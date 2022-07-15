using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScript : BuildingScript
{
    int orderIncome;



    private new void Awake()
    {
        base.Awake();
        orderType = -1;
        SkipNum = 1;
    }

    public void SetOrder(int type, int income, int timer, Color colorPair)
    {
        orderType = type;
        orderIncome = income;
        StartOrder(timer, colorPair);
        Debug.Log("building ordered");
    }

    public int DeliverOrder(List<int> ordertypes)
    {
        int index = ordertypes.IndexOf(orderType);
        if (isActive && index != -1)
        {
            ordertypes.Remove(orderType);
            CashInOrder();
            ResetState();
            Debug.Log("delivered order");
        }
        else
        {
            Debug.Log("wrong delivery");
        }

        return index;
    }

    protected override void ResetState()
    {
        base.ResetState();

        ApplicationModel.GM.playerScript.RemoveOrder(orderType);
        orderType = -1;
        orderIncome = 0;
        OrderManager.EndOrder();
    }


    public void CashInOrder()
    {
        //may add inversely prop. to how much time left
        //float startTime = Time.time;
        ApplicationModel.GM.ChangeCoin(orderIncome);
    }
}
