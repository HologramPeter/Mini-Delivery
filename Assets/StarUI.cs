using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{
    Image[] stars;

    public void SetStars(int star)
    {
        if (stars == null) stars = GetComponentsInChildren<Image>();
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = (i < star) ? Color.yellow : Color.black;
        }
    }
}
