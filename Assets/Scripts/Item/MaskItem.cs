using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskItem : Item
{
    protected override void CollectItem()
    {
        ApplicationModel.GM.AddMask();
    }

}

