using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantScript : BuildingScript
{
    [Header("Game Values")]
    public int orderPrice;
    [HideInInspector] public GameObject targetHouse;
    [SerializeField] GameObject minimapIcon2;
    private new void Awake()
    {
        base.Awake();
        SkipNum = 2;
    }

    private new void Start()
    {
        base.Start();
        if (minimapIcon2!=null)
        {
            minimapIcon2.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
            minimapIcon2.transform.Rotate(Vector3.forward, 180);
        }
        
    }

    public void SetOrder(int timer, Color colorPair, GameObject targetHouse)
    {
        this.targetHouse = targetHouse;
        StartOrder(timer, colorPair);
        Debug.Log("order placed");
    }
    public (int,GameObject) CollectOrder()
    {
        if (isActive)
        {
            ResetState();
            Debug.Log("collected order");
            return (GetInstanceID(),targetHouse);
        }
        Debug.Log("no order to collect");
        return (-1,null);
    }
}
