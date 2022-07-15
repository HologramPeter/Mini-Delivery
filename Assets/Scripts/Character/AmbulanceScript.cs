using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmbulanceScript : MonoBehaviour
{
    [SerializeField] AudioClip alarm;

    NavMeshAgent agent;
    Vector3 respawnPosition;

    AudioSource alarm_audio;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        alarm_audio = gameObject.AddComponent<AudioSource>();
        alarm_audio.clip = alarm;
        alarm_audio.loop = true;
        alarm_audio.Play();

        //spawn abulance with nav agent carry player (on enter trigger hide model) and set destination to hospital
        agent.SetDestination(ApplicationModel.GM.playerScript.gameObject.transform.position);
        StartCoroutine(CarryPlayerToHospital());
    }

    IEnumerator CarryPlayerToHospital()
    {
        while ((transform.position - agent.destination).magnitude > 1)
        {
            yield return new WaitForEndOfFrame();
        }
        ApplicationModel.GM.playerScript.HideModel();
        agent.SetDestination(ApplicationModel.GM.respawnLocation.transform.position);
        yield return new WaitForSeconds(2);

        ApplicationModel.GM.UI.FadeOut();
        yield return new WaitForSeconds(2);

        ApplicationModel.GM.Respawn();
        Destroy(gameObject);
    }
}
