using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] int rotationSpeed = 60;

    protected abstract void CollectItem();

    private void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.transform.tag == GameManager.PLAYERTAG)
        {
            CollectItem();
            Destroy(gameObject);
        }
    }
}
