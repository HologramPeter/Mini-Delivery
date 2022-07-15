using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static class ArrayExtensions
{
    public static T GetRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

}

//manage all  attributes methods, player state and game state
// UI related methods are also called here
public class GameManager : MonoBehaviour
{
    [HideInInspector] public  GameState game;
    [HideInInspector] public  LevelSummary levelSummary;
    [HideInInspector] public  PlayerScript playerScript;

    [HideInInspector] public  AIManager aIManager;
    [HideInInspector] public  OrderManager orderManager;
    [HideInInspector] public  ItemManager itemManager;

    [HideInInspector] public  EventManager eventManager;

    [HideInInspector] public  GameUI UI;

    [Header("LevelSetting")]
    [SerializeField] public int levelIndex = 0;
    [SerializeField] int targetCoin = 100;
    [SerializeField] int targetDeath = 1;
    [SerializeField] int targetItemCount = 1;

    [Header("Prefab")]
    [SerializeField] GameObject UIObj;
    [SerializeField] GameObject ambulancePrefab;

    [Header("PlayerAttribute")]
    [SerializeField] GameObject[] players;
    [SerializeField] int maxHealth;
    [SerializeField] int maxEnergy;
    [SerializeField] float deathTimePenalty = 30;
    [SerializeField] int deathCoinPenalty = 30;

    [Header("Music and sound")]
    [SerializeField] AudioClip BackgroundMusic;
    [SerializeField] AudioClip HurryUpMusic;
    [SerializeField] AudioClip EndGameSound;




    public  GameObject player;

    public  GameObject respawnLocation;

     int health;
     int energy;
    bool isUsingEnergy = false;

    public const string PLAYERTAG = "Player";
    public const string ENEMYTAG = "Enemy";
    public const string HOUSETAG = "House";
    public const string RESTAURANTTAG = "Restaurant";
    public const string RESPAWNTAG = "Respawn";
    public const string LOCKDOWNTAG = "LockDown";


    AudioSource music_audio;


    private void Awake()
    {
        ApplicationModel.GM = this;

        game = new GameState();
        levelSummary = new LevelSummary(targetCoin,targetDeath,targetItemCount);
        respawnLocation = GameObject.FindGameObjectWithTag(RESPAWNTAG);
    }

    // Start is called before the first frame update
    async void Start()
    {
        aIManager = GetComponent<AIManager>();
        orderManager = GetComponent<OrderManager>();
        itemManager = GetComponent<ItemManager>();
        eventManager = GetComponent<EventManager>();
        UI = UIObj.GetComponent<GameUI>();
        music_audio = gameObject.AddComponent<AudioSource>();

        player = Instantiate(players[game.SelectedCharacter], respawnLocation.transform.position, Quaternion.identity);
        playerScript = player.GetComponent<PlayerScript>();

        
        await StartGame();
        Resume();
    }

    async Task StartGame()
    {
        playerScript.SetColor(game.CharacterColor);
        if (game.UseNFT)
        {
            if (ApplicationModel.NFTTexture == null)
            {
                await game.LoadNFTTexture();
            }
            if (ApplicationModel.NFTTexture != null) playerScript.SetTexture(ApplicationModel.NFTTexture);
            else Debug.Log("NFT Loading failed");
        }

        maxHealth = Mathf.RoundToInt(maxHealth * game.GetHealthMult());
        playerScript.speed *= game.GetSpeedMult();
        aIManager.damageMult = game.GetDamageMult();

        health = maxHealth;
        energy = maxEnergy;

        UI.drawCoinTarget(targetCoin);
        UI.drawCoin(levelSummary.CoinEarned);
        UI.drawEnergy((float)energy / maxEnergy);
        UI.drawHealth((float)health / maxHealth);
        DrawItems();

        Debug.Log(game.MouseSensitivity.ToString());
        UI.setVisualMouseSensitivity(game.MouseSensitivity);
        UI.setVisualVolume(game.Volume);
        playerScript.mouseSensitivity = game.MouseSensitivity;
        AudioListener.volume = game.Volume;

        StartCoroutine(EnergyActivity());
        StartCoroutine(GameCountdown());

        UI.FadeIn();

        music_audio.clip = BackgroundMusic;
        music_audio.loop = true;
        music_audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (onEvent) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSpray();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UsePill();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseMask();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            toggleEnergy();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UI.ToggleMiniMap();
        }

        //test
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    Test();
        //}
    }

    void Test()
    {
        GameEnd();
        //ChangeCoin(Random.Range(20, 30));
    }

    #region Items

    [SerializeField] float maskEffectMult;
    [SerializeField] public float maskDuration;
    [SerializeField] float pillHealth;

     int maskCount = 1;
     int sprayCount = 1;
     int pillCount = 1;

     void DrawItems()
    {
        UI.drawMask(maskCount);
        UI.drawSpray(sprayCount);
        UI.drawPill(pillCount);
    }

    public  void AddMask()
    {
        maskCount += 1;
        //Debug.Log(MaskCount);
        UI.drawMask(maskCount);
    }

    public  void AddSpray()
    {
        sprayCount += 1;
        //Debug.Log(SprayCount);
        UI.drawSpray(sprayCount);
    }

    public  void AddPill()
    {
        pillCount += 1;
        //Debug.Log(PillCount);
        UI.drawPill(pillCount);
    }

    bool UseMask()
    {
        if (maskCount < 1) return false;
        maskCount -= 1;
        levelSummary.MaskCount += 1;

        UI.drawMask(maskCount);

        //do sth to the player
        playerScript.wearMask();
        //reduce radius
        aIManager.SetRangeMult(maskEffectMult, maskDuration);
        
        return true;
    }

    bool UseSpray()
    {
        if (sprayCount < 1) return false;
        sprayCount -= 1;
        levelSummary.SprayCount += 1;

        UI.drawSpray(sprayCount);

        //do sth to the player
        playerScript.spray();
        //disinfact on screen characters
        aIManager.Disinfect();
        return true;
    }

    bool UsePill()
    {
        if (pillCount < 1) return false;
        pillCount -= 1;
        levelSummary.PillCount += 1;

        UI.drawPill(pillCount);

        //do sth to the player
        playerScript.pill();

        ChangeHealth(pillHealth);
        return true;
    }

    #endregion

    #region health energy

    public  void ChangeCoin(int amount)
    {
        levelSummary.CoinEarned += amount;
        UI.drawCoin(levelSummary.CoinEarned);
    }
    public  void ChangeHealth(float amount)
    {
        int roundedAmount = Mathf.RoundToInt(amount);
        health = Mathf.Clamp(health + roundedAmount, 0, maxHealth);

        Debug.Log(amount.ToString()+" Health: "+health.ToString());

        UI.drawHealth((float)health / maxHealth);
        UI.ShowDamageOverlay(amount > 0);
        if (health == 0)
        {
            PlayerDead();
        }
    }
    void toggleEnergy()
    {
        if (!isUsingEnergy && (energy < 1)) return;

        isUsingEnergy = !isUsingEnergy;
        playerScript.rideBike(isUsingEnergy);
    }

    IEnumerator EnergyActivity()
    {
        while (true)
        {
            if (!isPaused)
            {
                if (isUsingEnergy)
                    energy = Mathf.Max(0, energy - 1);
                else
                    energy = Mathf.Min(maxEnergy, energy + 1);

                //Debug.Log(energy.ToString());
                //Debug.Log(maxEnergy.ToString());
                UI.drawEnergy((float)energy / maxEnergy);


                if (energy < 1 || onEvent)
                    isUsingEnergy = false;

                playerScript.rideBike(isUsingEnergy);
            }
            yield return new WaitForSeconds(1);
        }
    }

    #endregion


    #region game
    [SerializeField] float gameTimeInSeconds = 300;


     float currGameSecondLeft;
     bool isHurryUp = false;
    public IEnumerator GameCountdown()
    {
        currGameSecondLeft = gameTimeInSeconds;
        UI.drawTime(Mathf.FloorToInt(currGameSecondLeft));

        while (currGameSecondLeft > 0)
        {
            if (!isPaused & !onEvent)
            {
                currGameSecondLeft -= Time.deltaTime;
                if (currGameSecondLeft < 60 && !isHurryUp)
                {
                    music_audio.clip = HurryUpMusic;
                    music_audio.Play();
                    isHurryUp = true;
                }
            }
            yield return new WaitForEndOfFrame();

            UI.drawTime(Mathf.Max(0, Mathf.FloorToInt(currGameSecondLeft)));
        }

        GameEnd();
    }

    public  bool isPaused = false;
    public  void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = true;
        UI.Pause();
        PauseManagers();
    }

     void PauseManagers()
    {
        itemManager.Pause();
        orderManager.Pause();
        aIManager.Pause();
        eventManager.Pause();
        playerScript.Pause();
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        UI.Resume();
        ResumeManagers();
    }

     void ResumeManagers()
    {
        itemManager.Resume();
        orderManager.Resume();
        aIManager.Resume();
        eventManager.Resume();
        playerScript.Resume();
    }

    public delegate void MyDelegate();
    public  IEnumerator GameInvoke(MyDelegate d, float timeLeft)
    {
        while (timeLeft > 0)
        {
            if (!isPaused)
                timeLeft -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        d();
    }


    public  bool onEvent = false;
    public  void PlayerDead()
    {
        if (onEvent) return;
        onEvent = true;
        levelSummary.DeathCount += 1;
        AIManager.doDamage = false;

        PauseManagers();
        
        ChangeCoin(deathCoinPenalty);
        currGameSecondLeft -= deathTimePenalty;

        playerScript.Die();
        if (currGameSecondLeft > 0) Instantiate(ambulancePrefab, GetAmbulanceSpawnLocation(), Quaternion.identity);
    }
    
    public  void Respawn()
    {
        //reset states
        health = maxHealth;
        energy = 0;
        UI.drawHealth((float)health / maxHealth);
        UI.drawEnergy((float)energy / maxEnergy);

        maskCount = 0;
        sprayCount = 0;
        pillCount = 1;
        DrawItems();

        //destroy ambulance
        playerScript.Respawn(respawnLocation);

        ResumeManagers();
        UI.FadeIn();

        AIManager.doDamage = true;
        onEvent = false;
    }


    public void GameEnd()
    {
        Pause();
        onEvent = true;
        int c = levelSummary.GameCoins();
        game.Coins += c;
        if (c > 0) game.Level += 1;
        game.StarCount[levelIndex] = c;
        game.Save();

        UI.ShowSummary(c, game.Coins, levelSummary.GetSummary());

        music_audio.Stop();
        music_audio.PlayOneShot(EndGameSound);
    }

     Vector3 GetAmbulanceSpawnLocation()
    {
        Vector3 position;
        NavMeshHit spawnHit;
        do
        {
            position = new Vector3(5, 0, 5);
            if ((Random.value > 0.5f))
                position.x = -position.x;
            if ((Random.value > 0.5f))
                position.z = -position.z;
        } while (!NavMesh.SamplePosition(player.transform.position + position, out spawnHit, 10f, NavMesh.AllAreas));
        return spawnHit.position;
    }


    private void OnApplicationQuit()
    {
        game.Save();
    }
    #endregion
}
