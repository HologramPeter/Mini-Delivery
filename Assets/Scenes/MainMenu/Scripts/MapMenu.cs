using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MapMenu : MonoBehaviour
{

    int mapNo = 0;
    
    public void TungChung()
    {
        mapNo = 1; 
    }

    public void WanChai()
    {
        mapNo = 2;
    }
    public void MongKok()
    {
        mapNo = 3; 
    }

    public void NewGame()
    {
        Main_Menu.game.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + mapNo);
    }

    public void Shop()
    {
        Main_Menu.game.Save();
        SceneManager.LoadScene("UpgradeScreen");
    }
}
