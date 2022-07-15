using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] float lockDownDuration;
    [SerializeField] float lockDownFreq;

    GameObject[] lockDownObjs;

    bool isLocked = false;
    public bool isPaused = false;

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
        lockDownObjs = GameObject.FindGameObjectsWithTag(GameManager.LOCKDOWNTAG);
        DisableAllLockDown();
        StartCoroutine(GenerateEvent());
    }

    private void DisableAllLockDown()
    {
        foreach (GameObject lockDown in lockDownObjs)
        {
            lockDown.SetActive(false);
        }
    }

    IEnumerator GenerateEvent() {
        while (true)
        {
            while (isPaused)
            {
                yield return new WaitForEndOfFrame();
            }

            if (!isLocked)
            {
                Debug.Log("lock down!");
                yield return new WaitForSeconds(lockDownFreq);
                LockDown();
            }
            
            yield return new WaitForSeconds(lockDownFreq);
        }
    }

    void LockDown()
    {
        GameObject lockDown = lockDownObjs.GetRandom();

        isLocked = true;
        lockDown.SetActive(true);
        StartCoroutine(DelayedDisable(lockDown, lockDownDuration));
    }

    IEnumerator DelayedDisable(GameObject item, float time = 0)
    {
        while (time > 0.001f)
        {
            if (!isPaused) time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        item.SetActive(false);
        isLocked = false;
    }
}
