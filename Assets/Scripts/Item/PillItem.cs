using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillItem : Item
{
    protected override void CollectItem()
    {
        ApplicationModel.GM.AddPill();
    }
}
