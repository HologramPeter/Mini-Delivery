using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoad : MonoBehaviour
{
    public void GoBackToScene()
    {
        SceneManager.LoadScene("UpgradeScreen");
    }
}
