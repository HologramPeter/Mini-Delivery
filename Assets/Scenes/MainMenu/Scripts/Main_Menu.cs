using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    //public void NewGame()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}
    public static GameState game;

    [SerializeField] GameObject MainMenuObj;
    [SerializeField] GameObject MapMenuObj;
    [SerializeField] GameObject CharacterMenuObj;
    [SerializeField] GameObject SettingMenuObj;
    [SerializeField] GameObject HowToPlayObj;
    [SerializeField] AudioClip backgroundMusic;

    [SerializeField] GameObject TCStar;
    [SerializeField] GameObject WCStar;
    [SerializeField] GameObject MKStar;

    private void Awake()
    {
        game = new GameState();
        AudioListener.volume = game.Volume;
        Debug.Log(AudioListener.volume);
    }

    private void Start()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();

        if (ApplicationModel.ExitToLevelSelection)
        {
            MapMenu();
            ApplicationModel.ExitToLevelSelection = false;
        }

        TCStar.GetComponent<StarUI>().SetStars(game.StarCount[0]);
        WCStar.GetComponent<StarUI>().SetStars(game.StarCount[1]);
        MKStar.GetComponent<StarUI>().SetStars(game.StarCount[2]);
    }
    public void ExitGame()
    {
        game.Save();
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        game.Save();
    }


    public void MainMenu()
    {
        CharacterMenuObj.SetActive(false);
        SettingMenuObj.SetActive(false);
        MapMenuObj.SetActive(false);
        HowToPlayObj.SetActive(false);
        MainMenuObj.SetActive(true);
    }

    public void HowToPlay()
    {
        MainMenuObj.SetActive(false);
        HowToPlayObj.SetActive(true);
    }

    public void MapMenu(){
        MainMenuObj.SetActive(false);
        MapMenuObj.SetActive(true);
    }

    public void SettingMenu()
    {
        MainMenuObj.SetActive(false);
        SettingMenuObj.SetActive(true);
    }

    public void CharacterMenu()
    {
        MapMenuObj.SetActive(false);
        CharacterMenuObj.SetActive(true);
    }

}