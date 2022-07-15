using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeBar : MonoBehaviour
{
    [SerializeField] GameObject fillObj;
    [SerializeField] GameObject upgradeObj;
    [SerializeField] GameObject downgradeObj;
    [SerializeField] GameObject labelObj;

    //UpgradeScreenManager manager;
    Image fillImage;

    public Button UpgradeBtn { get; set; }
    public Button DowngradeBtn { get; set; }

    //public canvas
    public int Level { get; set; }
    public int Cost { get; set; }

    public void SetUp(int level, string label, int cost)
    {
        fillImage = fillObj.GetComponent<Image>();
        UpgradeBtn = upgradeObj.GetComponent<Button>();
        DowngradeBtn = downgradeObj.GetComponent<Button>();

        //manager = transform.parent.GetComponent<UpgradeScreenManager>();
        labelObj.GetComponent<TextMeshProUGUI>().text = label+":";
        Level = level;
        Cost = cost;
        Fill();
    }

    public bool Upgradable()
    {
        return Level < 10;
    }

    public bool Downgradable()
    {
        return Level > 0;
    }

    public void Upgrade()
    {
        Level += 1;
        Fill();
    }

    public void Downgrade()
    {
       Level -= 1;
       Fill();
    }

    public void Fill()
    {
        Debug.Log(Level.ToString());
        fillImage.fillAmount = Level / 10f;
    }

}
