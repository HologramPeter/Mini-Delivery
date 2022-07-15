using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

public class UpgradeScreenManager : MonoBehaviour
{
    [SerializeField] GameObject SpawnPosition;
    [SerializeField] GameObject[] characters;
    [SerializeField] int poseFreq = 5;
    //[SerializeField] float transitionDuration;

    //[SerializeField] int ObjNumber;

    [SerializeField] GameObject coinObj;
    [SerializeField] GameObject UpgradePanel;
    [SerializeField] GameObject VaccineStatObj;

    [SerializeField] GameObject LockedMessageObj;

    [SerializeField] GameObject colorPicker;
    [SerializeField] GameObject upgradeBar;

    [SerializeField] string onCloseChangeScene;

    [Header("Audio")]
    [SerializeField] AudioClip backgroundMusic;
    //[SerializeField] AudioClip clickSoundEffect;
    [SerializeField] AudioClip updateSoundEffect;

    [Header("NFT")]
    [SerializeField] GameObject NFTLoginButton;
    [SerializeField] GameObject NFTControlGroup;

    public static GameState game;
    bool pickColor = false;
    GameObject previewAddon;
    GameObject activeCharacter;

    AudioSource bg_audio;
    AudioSource effect_audio;

    int currentCharacter = 0;

    void Awake()
    {
        game = new GameState();
    }

    void Start()
    {
        bg_audio = gameObject.AddComponent<AudioSource>();
        effect_audio = gameObject.AddComponent<AudioSource>();

        bg_audio.clip = backgroundMusic;
        bg_audio.loop = true;
        bg_audio.Play();

        LoadCharacterModel();
        LoadUI();
        SetColor();

        if (game.Account == "")
        {
            NFTLoginButton.SetActive(true);
            NFTControlGroup.SetActive(false);
        }
        else
        {
            NFTLoginButton.SetActive(false);
            NFTControlGroup.SetActive(true);
            ApplyNFT();
        }


        StartCoroutine(RandomPose());
    }

    private void LoadCharacterModel()
    {
        SwapTo(game.SelectedCharacter);

        ////character child ObjNumber is base model
        //for (int tree = 0; tree < game.CharacterUpgrade.Length; tree++)
        //{
        //    for (int i = 0; i < game.CharacterUpgrade[tree]; i++)
        //    {
        //        GameObject addon = character.transform.GetChild(ObjNumber+1+tree).GetChild(i).gameObject;
        //        Debug.Log(addon);
        //        addon.GetComponent<Outline>().enabled = false;
        //        addon.SetActive(true);
        //    }
        //}
    }

    public void SelectCharacter()
    {
        if (MissingStars(currentCharacter)) return;
        game.SelectedCharacter = currentCharacter;
        DisplayStat();
    }

    public void SwapLeft()
    {
        SwapTo((currentCharacter - 1 + characters.Length) % characters.Length);
    }

    public void SwapRight()
    {
        SwapTo((currentCharacter + 1) % characters.Length);
    }

    void SwapTo(int characterIndex)
    {
        Debug.Log(characterIndex);
        if (activeCharacter!=null) Destroy(activeCharacter);
        //todo fix ordering
        activeCharacter = Instantiate(characters[characterIndex],
            SpawnPosition.transform.position,
            SpawnPosition.transform.rotation);
        activeCharacter.transform.localScale = SpawnPosition.transform.localScale;

        animator = activeCharacter.GetComponent<Animator>();

        currentCharacter = characterIndex;

        SetColor();
        ApplyNFT();

        ShowLockMessage(currentCharacter);

    }

    bool MissingStars(int index)
    {
        return (index * 3 - game.GetTotalStars()) > 0;
    }

    void ShowLockMessage(int index)
    {
        LockedMessageObj.GetComponent<TextMeshProUGUI>().text = MissingStars(index) ?
            "Stars required: "+game.GetTotalStars().ToString() + "/" + (index * 3).ToString() :
            "";
    }

    private void LoadUI()
    {
        displayCoins();
        LoadUpgradeUI();
        DisplayStat();
    }

    private void LoadUpgradeUI()
    {
        // position
        SetUpBar(game.HealthUpgrade, "Health", 10, 1);
        SetUpBar(game.SpeedUpgrade, "Speed", 20, 2);
        SetUpBar(game.DamageUpgrade, "Resistance", 30, 3);
    }

    private void SetUpBar(int level, string label, int cost, int num)
    {
        GameObject obj = Instantiate(upgradeBar, UpgradePanel.transform);
        RectTransform t = obj.GetComponent<RectTransform>();
        //t.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        t.anchoredPosition = new Vector3(80, 60f*num, 0f);

        UpgradeBar bar = obj.GetComponent<UpgradeBar>();
        bar.SetUp(level, label, cost);
        bar.UpgradeBtn.onClick.AddListener(delegate
        {
            if (bar.Upgradable() && changeCoins(-bar.Cost))
            {
                bar.Upgrade();
                game.Upgrade(label,true);
                DisplayStat();
            }
        });
        bar.DowngradeBtn.onClick.AddListener(delegate
        {
            if (bar.Downgradable() && changeCoins(bar.Cost))
            {
                bar.Downgrade();
                game.Upgrade(label,false);
                DisplayStat();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (pickColor) ChangeColor();
    }

    bool changeCoins(int amount)
    {
        if (game.Coins + amount < 0) return false;
        game.Coins += amount;
        displayCoins();
        return true;
    }

    void displayCoins()
    {
        coinObj.GetComponent<TextMeshProUGUI>().text = game.Coins.ToString();
    }

    #region preview
    //public void resetPreviewTree()
    //{
    //    if (previewAddon != null)
    //    {
    //        previewAddon.SetActive(false);
    //        previewAddon = null;
    //    }
    //}
    //public void previewTree(int tree)
    //{
    //    if (previewAddon != null) previewAddon.SetActive(false);
    //    int currentUpgradeIndex = game.VaccineUpgrade[tree - 1];
    //    try
    //    {
    //        previewAddon = character.transform.GetChild(tree + ObjNumber).GetChild(currentUpgradeIndex).gameObject;
    //        previewAddon.GetComponent<Outline>().enabled = true;
    //        previewAddon.SetActive(true);
    //    }
    //    catch (Exception)
    //    {
    //        return;
    //    }
    //}
    public void TakeVaccine(int type)
    {
        if (game.EligibleVaccineDose < 1) return;

        DisplayStat();
        game.VaccineUpgrade[type] += 1;
    }

    public void DisplayStat()
    {
        VaccineStatObj.GetComponent<TextMeshProUGUI>().text = game.GetStat();
        UpdateSound();
    }

    public void PreviewVaccineStat(int vaccineIndex)
    {
        //Debug.Log(game.PreviewVaccineEffect(vaccineIndex));
        VaccineStatObj.GetComponent<TextMeshProUGUI>().text = game.PreviewVaccineStat(vaccineIndex);
        UpdateSound();
    }

    public void PreviewCharacterStat()
    {
        VaccineStatObj.GetComponent<TextMeshProUGUI>().text = game.PreviewCharacterStat(currentCharacter);
        UpdateSound();
    }

    void UpdateSound()
    {
        effect_audio.Stop();
        effect_audio.PlayOneShot(updateSoundEffect);
    }
    #endregion

    public void startPickColor()
    {
        Debug.Log("start");
        pickColor = true;
    }

    public void stopPickColor()
    {
        Debug.Log("stop");
        pickColor = false;
    }

    public void ChangeColor()
    {
        game.CharacterColor = colorPicker.GetComponent<ColorPickerUnityUI>().value;
        SetColor();
    }

    private void SetColor()
    {
        //Debug.Log(ColorUtility.ToHtmlStringRGB(game.CharacterColor));
        activeCharacter.GetComponent<ModelHandler>().SetColor(game.CharacterColor);
    }

    Animator animator;
    IEnumerator RandomPose()
    {
        while (true)
        {
            yield return new WaitForSeconds(poseFreq);

            if (animator == null) continue;
                animator.SetBool("IsWalking", false);

            switch (Mathf.RoundToInt(Random.Range(-0.49f, 3.49f)))
            {
                case 0:
                    animator.SetBool("IsWalking", false); 
                    break;
                case 1:
                    animator.SetTrigger("Spray");
                    break;
                case 2:
                    animator.SetTrigger("Pill");
                    break;
                case 3:
                    animator.SetTrigger("Mask");
                    break;
                default:
                    break;
            }
        }
    }

    public void SetContract(string contract)
    {
        game.Contract = contract;
    }

    public void SetToken(string token)
    {
        game.Token = token;
    }

    public void UseNFT(bool use)
    {
        game.UseNFT = use;
    }

    public async void ApplyNFT()
    {
        if (game.UseNFT)
        {
            if (ApplicationModel.NFTTexture == null)
            {
                await game.LoadNFTTexture();
            }
            if (ApplicationModel.NFTTexture != null) activeCharacter.GetComponent<ModelHandler>().SetTexture(ApplicationModel.NFTTexture);
            else Debug.Log("NFT Loading failed");
        }
        else
        {
            activeCharacter.GetComponent<ModelHandler>().SetTexture(null);
        }
    }

    public void WebLogin()
    {
        game.Save();
        SceneManager.LoadScene("WebLogin");
    }

    public void SampleShop()
    {
        game.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitScreen()
    {
        game.Save();
        ApplicationModel.ExitToLevelSelection = true;
        SceneManager.LoadScene(onCloseChangeScene);
    }

    void OnApplicationQuit()
    {
        game.Save();
    }
}
