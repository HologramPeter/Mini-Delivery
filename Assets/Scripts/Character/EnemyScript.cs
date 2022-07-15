using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] float timeToDespawn = 2f;
    [SerializeField] GameObject model;
    [SerializeField] GameObject circle;

    float outOfFrameTime;

    NavMeshAgent agent;
    Renderer _renderer;
    SphereCollider sphereCollider;
    Animator animator;
    AudioSource effect_audio;

    AudioClip walkSound;
    AudioClip coughSound;

    AIManager aIManager;
    float speed;
    float range;
    float damage;

    bool disinfected;

    public void Initialise(AIManager s, float speed, float range, float damage, Color ringColor, AudioClip walkSound, AudioClip coughSound)
    {
        aIManager = s;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        this.walkSound = walkSound;
        this.coughSound = coughSound;
        local_Start();
        UpdateRange();
        SetColor(ringColor);
        enabled = true;
    }

    // Start is called before the first frame update
    void local_Start()
    {
        Debug.Log("adding components");
        animator = model.GetComponent<Animator>();
        _renderer = circle.GetComponent<Renderer>();
        agent = gameObject.AddComponent<NavMeshAgent>();
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        effect_audio = gameObject.AddComponent<AudioSource>();
        effect_audio.volume = 0.8f;
        effect_audio.loop = true;
        effect_audio.clip = walkSound;
        effect_audio.Play();

        agent.speed = speed;
        agent.SetDestination(GetTargetLocation());

        StartCoroutine(AIManager.GameLoop(RandomAction, 5, false));
    }

    // Update is called once per frame
    void Update()
    {
        if (AIManager.isPaused && agent.speed != 0)
            Pause();
        else if(!AIManager.isPaused && agent.speed == 0)
            Resume();

        if (!_renderer.isVisible)
        {
            outOfFrameTime += Time.deltaTime;
            if (outOfFrameTime >= timeToDespawn)
            {
                Debug.Log("Despawned with location: " + transform.position.ToString());
                aIManager.DecrementSpawnNum();
                Destroy(gameObject);
            }
        }
        else
        {
            outOfFrameTime = 0;
        }

        Vector3 distanceToDestination = agent.destination-transform.position;
        //Debug.Log(distanceToDestination.magnitude);
        if (distanceToDestination.magnitude < 2f)
            agent.SetDestination(GetTargetLocation());
    }

    Vector3 GetTargetLocation()
    {
        Vector3 position;
        NavMeshHit spawnHit;
        do
        {
            position = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        } while (!NavMesh.SamplePosition(transform.position + position, out spawnHit, 10f, NavMesh.AllAreas));
        return spawnHit.position;
    }

    public void Pause()
    {
        agent.speed = 0;
        effect_audio.Pause();
    }

    public void Resume()
    {
        agent.speed = speed;
        if (!effect_audio.isPlaying)
        {
            effect_audio.Play();
        }
    }

    private Coroutine damageCoroutine;
    //add collision damage with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Player"))
        {
            Debug.Log("start damage");
            damageCoroutine = StartCoroutine(AIManager.GameLoop(() => { ApplicationModel.GM.ChangeHealth(-damage); }, 0.5f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Player"))
        {
            Debug.Log("End damage");
            if (damageCoroutine!=null)
            {
                StopCoroutine(damageCoroutine);
            }
        }
    }

    public void Disinfect()
    {
        disinfected = true;
        UpdateRange();
    }

    public void UpdateRange()
    {
        float radius = GetRange();

        Debug.Log("Range Changed to " + radius.ToString());
        sphereCollider.radius = radius;
        circle.transform.localScale = new Vector3(radius, radius, 1);
    }

    void SetColor(Color color)
    {
        circle.GetComponent<SpriteRenderer>().color = color;
    }

    float GetRange()
    {
        if (disinfected)
        {
            return 0;
        }
        else
        {
            Debug.Log("Range mult" + aIManager.rangeMult.ToString());
            return range * aIManager.rangeMult;
        }
    }

    void RandomAction()
    {
        switch(Mathf.RoundToInt(Random.Range(-0.49f, 5.49f)))
        {
            case 0:
                animator.SetTrigger("Cough");
                effect_audio.PlayOneShot(coughSound);
                break;
            default:
                animator.SetTrigger("Phone");
                break;
        }
    }

}
