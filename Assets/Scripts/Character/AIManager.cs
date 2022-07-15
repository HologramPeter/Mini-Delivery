using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    //[SerializeField] Camera cam; //might switch later
    [SerializeField] int minSpawnRange;
    [SerializeField] int maxSpawnRange;

    [SerializeField] GameObject[] enemyPrefabs;

    [SerializeField] int maxSpawn;
    [SerializeField] int spawnFreq;

    [SerializeField] int enemySpeedRangeStart;
    [SerializeField] int enemySpeedRangeEnd;

    [SerializeField] float[] enemyRanges;
    [SerializeField] float[] enemyDamages;

    [SerializeField] Gradient damageRingGradient;
    [SerializeField] AudioClip WalkSound;
    [SerializeField] AudioClip CoughSound;

    int spawnNum = 0;
    float minEnemyDamage;
    float minMaxDamageDifference;

    public float damageMult;

    public float rangeMult = 1;

    public static bool isPaused = true;

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    private void Awake()
    {
        isPaused = true;
        minEnemyDamage = Mathf.Min(enemyDamages);
        minMaxDamageDifference = Mathf.Max(enemyDamages) - minEnemyDamage;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateEnermy());
    }

    IEnumerator GenerateEnermy()
    {
        while (true)
        {
            while (isPaused)
            {
                yield return new WaitForEndOfFrame();
            }

            if (spawnNum < maxSpawn)
            {
                SpawnEnemy();
                spawnNum += 1;
            }

            yield return new WaitForSeconds(spawnFreq);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnLocation = GetSpawnLocation();

        int enemyType = Mathf.RoundToInt(Random.Range(-0.49f, enemyRanges.Length - 0.51f));
        float enemySpeed = Random.Range(enemySpeedRangeStart, enemySpeedRangeEnd);
        float enemyRange = enemyRanges[enemyType];
        float enemyDamage = enemyDamages[enemyType];
        int enemyModelType = Mathf.RoundToInt(Random.Range(-0.49f, enemyPrefabs.Length - 0.51f));

        Color ringColor = damageRingGradient.Evaluate((enemyDamage-minEnemyDamage)/minMaxDamageDifference);


        GameObject e = Instantiate(enemyPrefabs[enemyModelType], spawnLocation, Quaternion.identity);
        EnemyScript es = e.GetComponent<EnemyScript>();
        es.Initialise(this, enemySpeed, enemyRange, enemyDamage*damageMult, ringColor, WalkSound, CoughSound);


        Debug.Log("spawnEnemy with position:" +spawnLocation.ToString()
            + ", speed:" + enemySpeed.ToString()
            + ", range:" + enemyRange.ToString()
            + ", damage:" + enemyDamage.ToString());
    }

    Vector3 GetSpawnLocation()
    {
        Vector3 position;
        NavMeshHit spawnHit;
        do
        {
            position = new Vector3(Random.Range(minSpawnRange, maxSpawnRange), 0, Random.Range(minSpawnRange, maxSpawnRange));
            if ((Random.value > 0.5f))
                position.x = -position.x;
            if ((Random.value > 0.5f))
                position.z = -position.z;
        } while (!NavMesh.SamplePosition(ApplicationModel.GM.player.transform.position + position, out spawnHit, 10f, NavMesh.AllAreas));
        return spawnHit.position;
    }

    public void DecrementSpawnNum()
    {
        spawnNum -= 1;
    }

    public void SetRangeMult(float rangeMult, float duration)
    {
        this.rangeMult = rangeMult;
        Debug.Log("Range mult" + this.rangeMult.ToString());
        UpdateRange();
        StartCoroutine(ApplicationModel.GM.GameInvoke(() => { this.rangeMult = 1; Debug.Log("reset"); UpdateRange(); }, duration));
    }

    public void Disinfect()
    {
        foreach (GameObject e in GameObject.FindGameObjectsWithTag(GameManager.ENEMYTAG))
        {
            e.GetComponent<EnemyScript>().Disinfect();
        }
    }

    void UpdateRange()
    {
        //change existing enemies
        foreach (GameObject e in GameObject.FindGameObjectsWithTag(GameManager.ENEMYTAG))
        {
            e.GetComponent<EnemyScript>().UpdateRange();
        }
    }

    public static bool doDamage = true;

    public delegate void MyDelegate();
    public static IEnumerator GameLoop(MyDelegate d, float time, bool initial = true)
    {
        if (initial) d();

        float countdown = time;
        while (doDamage)
        {
            while (countdown > 0)
            {
                if (!isPaused)
                    countdown -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
            d();
            countdown = time;
        }
    }
}
