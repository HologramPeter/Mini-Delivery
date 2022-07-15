using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    float storedShadowDistance;
    private void Start()
    {
        Camera cam = GetComponent<Camera>();
    }
    void OnPreRender()
    {
        storedShadowDistance = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance = 0;
    }

    void OnPostRender()
    {
        QualitySettings.shadowDistance = storedShadowDistance;
    }
}
