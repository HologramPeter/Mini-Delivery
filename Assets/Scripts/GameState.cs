using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class GameState
{
    int level;
    int coins;
    int healthUpgrade;
    int damageUpgrade;
    int speedUpgrade;



    Color characterColor;

    //getter and setter
    public int Level { get => level; set => level = value; }
    public int Coins { get => coins; set => coins = value; }
    public int EligibleVaccineDose { get => GetClearedLevels() - VaccineTaken();}
    public int HealthUpgrade { get => healthUpgrade; set => healthUpgrade = value; }
    public int DamageUpgrade { get => damageUpgrade; set => damageUpgrade = value; }
    public int SpeedUpgrade { get => speedUpgrade; set => speedUpgrade = value; }
    public Color CharacterColor { get => characterColor; set => characterColor = value; }
    public float MouseSensitivity { get; set; }
    public float Volume { get; set; }
    public int[] VaccineUpgrade { get; set; } = new int[2];
    public int[] StarCount { get; set; } = new int[3];//level count

    public int SelectedCharacter { get; set; }

    public static int UpgradeMax = 10;

    public string Account { get; set; }
    public string Contract { get; set; }
    public string Token { get; set; }
    public bool UseNFT { get; set; }

    public GameState()
    {
        Load();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("coins", coins);
        //PlayerPrefs.SetInt("eligibleVaccineDose", eligibleVaccineDose);
        PlayerPrefs.SetInt("healthUpgrade", healthUpgrade);
        PlayerPrefs.SetInt("damageUpgrade", damageUpgrade);
        PlayerPrefs.SetInt("speedUpgrade", speedUpgrade);

        string hexColor = ColorUtility.ToHtmlStringRGB(characterColor);
        PlayerPrefs.SetString("characterColor", hexColor);
        //Debug.Log(PlayerPrefs.GetString("characterColor", "000000"));

        PlayerPrefs.SetInt("vaccineUpgrade1", VaccineUpgrade[0]);
        PlayerPrefs.SetInt("vaccineUpgrade2", VaccineUpgrade[1]);

        PlayerPrefs.SetInt("starCount1", StarCount[0]);
        PlayerPrefs.SetInt("starCount2", StarCount[1]);
        PlayerPrefs.SetInt("starCount3", StarCount[2]);

        PlayerPrefs.SetFloat("mouseSensitivity", MouseSensitivity);
        PlayerPrefs.SetFloat("volume", Volume);

        PlayerPrefs.SetInt("selectedCharacter", SelectedCharacter);

        //NFT
        PlayerPrefs.SetString("Account", Account);
        PlayerPrefs.SetString("Contract", Contract);
        PlayerPrefs.SetString("Token", Token);
        PlayerPrefs.SetInt("UseNFT", UseNFT? 1:0);
    }

    public void Load()
    {
        level = PlayerPrefs.GetInt("level", 0);
        coins = PlayerPrefs.GetInt("coins", 100);
        //eligibleVaccineDose = PlayerPrefs.GetInt("eligibleVaccineDose", 0);
        healthUpgrade = PlayerPrefs.GetInt("healthUpgrade", 0);
        damageUpgrade = PlayerPrefs.GetInt("damageUpgrade", 0);
        speedUpgrade = PlayerPrefs.GetInt("speedUpgrade", 0);
        
        string hexColor= PlayerPrefs.GetString("characterColor", "000000");
        ColorUtility.TryParseHtmlString("#" + hexColor, out characterColor);
        //Debug.Log(hexColor);

        VaccineUpgrade[0] = PlayerPrefs.GetInt("vaccineUpgrade1", 0);
        VaccineUpgrade[1] = PlayerPrefs.GetInt("vaccineUpgrade2", 0);

        StarCount[0] = PlayerPrefs.GetInt("starCount1", 0);
        StarCount[1] = PlayerPrefs.GetInt("starCount2", 0);
        StarCount[2] = PlayerPrefs.GetInt("starCount3", 0);

        MouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 200);
        Volume = PlayerPrefs.GetFloat("volume", 1);

        SelectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0);

        //NFT
        Account = PlayerPrefs.GetString("Account", "0x88B48F654c30e99bc2e4A1559b4Dcf1aD93FA656");
        Contract = PlayerPrefs.GetString("Contract", "0x446C17a97e2b7461FBce7a1B5D2293BF832A380C");
        Token = PlayerPrefs.GetString("Token", "30948256496342367792501964843211623706276934530243934524698709116451362439169");
        UseNFT = PlayerPrefs.GetInt("UseNFT", 0) > 0;

        //TestState();
    }

    void TestState()
    {
        //healthUpgrade = 1;
        //damageUpgrade = 2;
        //speedUpgrade = 3;

        //VaccineUpgrade[0] = 0;
        //VaccineUpgrade[1] = 0;

        ////CHANGE HERE
        //coins = 100;
        //StarCount[0] = 3;
        //StarCount[1] = 2;
        //StarCount[2] = 1;

        //Contract = "0x88B48F654c30e99bc2e4A1559b4Dcf1aD93FA656";
        //Account = "0x446C17a97e2b7461FBce7a1B5D2293BF832A380C";
        //Token = "30948256496342367792501964843211623706276934530243934524698709116451362439169";
    }

    public int GetTotalStars()
    {
        int sum = 0;
        foreach(int i in StarCount) sum += i;
        return sum;
    }
    //+
    public float GetHealthMult(int v0 = 0, int v1 = 0, int characterIndex = -1)
    {
        float mult = healthUpgrade * 0.1f + (VaccineUpgrade[0] + v0) * -0.15f + (VaccineUpgrade[1] + v1) * -0.1f;
        int character = characterIndex > -1 ? characterIndex : SelectedCharacter;
        if (character == 1) mult += 0.3f;
        if (character == 2) mult -= 0.1f;
        if (character == 3) mult += 0.2f;
        return 1 + mult;
    }
    //+
    public float GetSpeedMult(int v0 = 0, int v1 = 0, int characterIndex = -1)
    {
        float mult = speedUpgrade * 0.1f + (VaccineUpgrade[0] + v0) * -0.1f + (VaccineUpgrade[1] + v1) * -0.1f;
        int character = characterIndex > -1 ? characterIndex : SelectedCharacter;
        if (character == 1) mult -= 0.1f;
        if (character == 2) mult += 0.3f;
        if (character == 3) mult += 0.1f;
        return 1+mult;
    }
    //-
    public float GetDamageMult(int v0 = 0, int v1 = 0, int characterIndex = -1)
    {
        float mult = damageUpgrade * 0.02f + (VaccineUpgrade[0]+ v0) * 0.3f + (VaccineUpgrade[1] + v1) * 0.15f;
        int character = characterIndex > -1 ? characterIndex : SelectedCharacter;
        if (character == 2) mult += 0.1f;
        if (character == 3) mult += 0.2f;
        return 1-mult;
    }

    public int GetClearedLevels()
    {
        int n = 0;
        foreach (int i in StarCount) if (i > 0) n += 1;
        return n;
    }

    public int VaccineTaken()
    {
        int sum = 0;
        foreach (int i in VaccineUpgrade) sum += i;
        return sum;
    }

    string VaccineEffect(int v0 = 0, int v1 = 0, int characterIndex = -1)
    {
        Debug.Log(GetDamageMult(v0, v1, characterIndex) - 1);
        return
            "Available Doses: " + EligibleVaccineDose.ToString() + "\n" +
            string.Format("Health: {0:P0}\n", GetHealthMult(v0,v1, characterIndex)) +
            string.Format("Speed: {0:P0}\n", GetSpeedMult(v0, v1, characterIndex)) +
            string.Format("Dmg: {0:P0}\n", GetDamageMult(v0, v1, characterIndex)-1)
            ;

    }

    public string GetStat()
    {
        return VaccineEffect();
    }

    public string PreviewVaccineStat(int vaccineIndex)
    {
        if (vaccineIndex == 0)
            return VaccineEffect(VaccineUpgrade[0] + 1, VaccineUpgrade[1]);
        else
            return VaccineEffect(VaccineUpgrade[0], VaccineUpgrade[1] + 1);
    }

    public string PreviewCharacterStat(int characterIndex)
    {
        return VaccineEffect(0,0, characterIndex);
    }

    public void Upgrade(string label, bool upgrade)
    {
        switch (label)
        {
            case "Health":
                healthUpgrade += upgrade ? 1 : -1;
                break;
            case "Speed":
                speedUpgrade += upgrade ? 1 : -1;
                break;
            case "Damage":
                damageUpgrade += upgrade ? 1 : -1;
                break;
            case "Resistance":
                damageUpgrade += upgrade ? 1 : -1;
                break;
            default:
                break;
        }
    }


    public async Task LoadNFTTexture()
    {
        string chain = "ethereum";
        string network = "rinkeby";

        BigInteger balanceOf = await ERC1155.BalanceOf(chain, network, Contract, Account, Token);
        Debug.Log(balanceOf);

        if (balanceOf > 0)
        {
            Debug.Log("balanceOf > 0");
            //Sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        }

        // fetch uri from chain
        string uri = await ERC1155.URI(chain, network, Contract, Token);
        Debug.Log("uri: " + uri);
        uri = uri.Replace("0x{id}", Token);

        // fetch json from uri
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        await webRequest.SendWebRequest();
        Response data = JsonUtility.FromJson<Response>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
        Debug.Log(data);

        // parse json to get image uri
        string imageUri = data.image;
        Debug.Log("imageUri: " + imageUri);

        // fetch image and display in game
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUri);
        await textureRequest.SendWebRequest();

        ApplicationModel.NFTTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
        
    }

    public class Response
    {
        public string image;
    }
}
