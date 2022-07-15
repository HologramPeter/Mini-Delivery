using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayItem : Item
{
    protected override void CollectItem()
    {
        ApplicationModel.GM.AddSpray();
    }
}

