using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    string houseTag = GameManager.HOUSETAG;
    string restaurantTag = GameManager.RESTAURANTTAG;
    [SerializeField] int orderFreq;
    [SerializeField] int maxOrder;
    [SerializeField] int orderTimer;

    [Header("Income")]
    [SerializeField] float pricePercentage;
    [SerializeField] float distanceMultiplier;
    


    GameObject[] houses;
    GameObject[] restaruants;
    static int activeOrder;

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

    private void Start()
    {
        houses = GameObject.FindGameObjectsWithTag(houseTag);
        restaruants = GameObject.FindGameObjectsWithTag(restaurantTag);
        maxOrder = MaxOrder();


        StartCoroutine(GenerateOrder());
    }

    IEnumerator GenerateOrder()
    {
        yield return new WaitForSeconds(orderFreq);
        while (true)
        {
            while (isPaused)
            {
                yield return new WaitForEndOfFrame();
            }

            if (activeOrder < maxOrder)
            {
                Debug.Log("Trying to place order");
                order();
                activeOrder += 1;
            }
            yield return new WaitForSeconds(orderFreq);
        }
    }

    void order()
    {
        GameObject house = houses.GetRandom();
        HouseScript houseScript = house.GetComponent<HouseScript>();
        while (houseScript.isActive)
        {
            Debug.Log("Finding house");
            house = houses.GetRandom();
            houseScript = house.GetComponent<HouseScript>();
        }

        GameObject restaruant = restaruants.GetRandom();
        RestaurantScript restaruantScript = restaruant.GetComponent<RestaurantScript>();
        while (restaruantScript.isActive)
        {
            Debug.Log("Finding restaruant");
            restaruant = restaruants.GetRandom();
            restaruantScript = restaruant.GetComponent<RestaurantScript>();
        }


        float income = restaruantScript.orderPrice * pricePercentage
            + (restaruant.transform.position - house.transform.position).magnitude * distanceMultiplier;

        Color colorPair = Random.ColorHSV(0,1,1,1,0,1,1,1);

        restaruantScript.SetOrder(orderTimer, colorPair, house);
        houseScript.SetOrder(restaruantScript.GetInstanceID(), Mathf.RoundToInt(income), orderTimer, colorPair);
    }

    public static void EndOrder()
    {
        Debug.Log("Order ended");
        activeOrder -= 1;
    }

    int MaxOrder()
    {
        return Mathf.Min(restaruants.Length, houses.Length, maxOrder);
    }
}
