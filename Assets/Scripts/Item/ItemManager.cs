using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemManager : MonoBehaviour
{

    //Game Attr
    [SerializeField] int itemSpawnFreq;
    [SerializeField] int itemDespawnTime;

    [SerializeField] int minSpawnRange;
    [SerializeField] int maxSpawnRange;

    [SerializeField] GameObject sprayItem;
    [SerializeField] GameObject pillItem;
    [SerializeField] GameObject maskItem;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateItem());
    }

    IEnumerator GenerateItem()
    {
        while (true)
        {
            while (isPaused)
            {
                yield return new WaitForEndOfFrame();
            }
            
            SpawnItem();
            yield return new WaitForSeconds(itemSpawnFreq);
        }
    }

    void SpawnItem()
    {
        Vector3 spawnLocation = GetSpawnLocation();
        GameObject item;

        int itemType = Mathf.RoundToInt(Random.Range(-0.49f, 2.49f));
        switch (itemType)
        {
            case 0:
                item = Instantiate(sprayItem, spawnLocation, sprayItem.transform.rotation);
                break;
            case 1:
                item = Instantiate(pillItem, spawnLocation, sprayItem.transform.rotation);
                break;
            case 2:
                item = Instantiate(maskItem, spawnLocation, sprayItem.transform.rotation);
                break;
            default:
                item = new GameObject();
                break;
        }

        StartCoroutine(DelayedDestroy(item, itemDespawnTime));
        Debug.Log("Spawned item of type " + itemType.ToString() + " with position: " + spawnLocation.ToString());
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

        return Adjust(spawnHit.position);
    }

    Vector3 Adjust(Vector3 p)
    {
        p.y = 1;
        return p;
    }


    IEnumerator DelayedDestroy(GameObject item, float time = 0)
    {
        if (isPaused) yield return new WaitForEndOfFrame();
        while (time > 0.001f)
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("item despawned");
        Destroy(item);
    }
}
