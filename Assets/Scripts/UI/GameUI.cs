using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//this class serve as interface for the game manager to interact with UI elements
public class GameUI : MonoBehaviour
{
    [Header("UI Display")]
    [SerializeField] GameObject HealthFillObj;
    [SerializeField] GameObject EnergyFillObj;

    [SerializeField] GameObject CoinCountObj;
    [SerializeField] GameObject CoinTargetObj;
    [SerializeField] GameObject GameTimerObj;

    [SerializeField] GameObject MaskIconObj;
    [SerializeField] GameObject MaskCountObj;

    [SerializeField] GameObject SprayIconObj;
    [SerializeField] GameObject SprayCountObj;

    [SerializeField] GameObject PillIconObj;
    [SerializeField] GameObject PillCountObj;

    [SerializeField] GameObject MinimapObj;

    [Header("UI Color")]
    [SerializeField] Color ItemDisabledColor;
    [SerializeField] Color RestoreHealthColor;
    [SerializeField] Color DamageHealthColor;

    [Header("Overlays")]
    [SerializeField] GameObject PauseMenu;

    [SerializeField] GameObject damageOverlayObj;
    [SerializeField] GameObject fadeOutOverlayObj;
    [SerializeField] const float animationDuration = 0.5f;
    [SerializeField] const float fadeDuration = 1f;

    [Header("Level Summary")]
    [SerializeField] GameObject SummaryMenu;

    [SerializeField] GameObject SummaryTitleObj;

    [SerializeField] GameObject Star1Obj;
    [SerializeField] GameObject Star2Obj;
    [SerializeField] GameObject Star3Obj;

    [SerializeField] GameObject Summary1Obj;
    [SerializeField] GameObject Summary2Obj;
    [SerializeField] GameObject Summary3Obj;

    [SerializeField] GameObject SummaryCoinObj;

    [Header("Options")]
    [SerializeField] GameObject MouseSliderObj;
    [SerializeField] GameObject VolumeSliderObj;

    [Header("Music and sound")]
    [SerializeField] AudioClip CashSound;
    [SerializeField] AudioClip HealSound;
    [SerializeField] AudioClip DamageSound;

    Image HealthFill;
    Image EnergyFill;
    Image MaskIcon;
    Image SprayIcon;
    Image PillIcon;
    RawImage Minimap;

    Image damageOverlay;
    Image fadeOutOverlay;

    TextMeshProUGUI CoinCount;
    TextMeshProUGUI CoinTarget;
    TextMeshProUGUI GameTimer;

    TextMeshProUGUI MaskCount;
    TextMeshProUGUI SprayCount;
    TextMeshProUGUI PillCount;


    Image Star1;
    Image Star2;
    Image Star3;

    TextMeshProUGUI Summary1;
    TextMeshProUGUI Summary2;
    TextMeshProUGUI Summary3;
    TextMeshProUGUI SummaryTitle;
    TextMeshProUGUI SummaryCoin;

    Slider MouseSlider;
    Slider VolumeSlider;

    AudioSource effect_audio;

    // Start is called before the first frame update
    void Awake()
    {
        HealthFill = HealthFillObj.GetComponent<Image>();
        EnergyFill = EnergyFillObj.GetComponent<Image>();
        CoinCount  = CoinCountObj.GetComponent<TextMeshProUGUI>();
        CoinTarget = CoinTargetObj.GetComponent<TextMeshProUGUI>();
        MaskCount  = MaskCountObj.GetComponent<TextMeshProUGUI>();
        SprayCount = SprayCountObj.GetComponent<TextMeshProUGUI>();
        PillCount = PillCountObj.GetComponent<TextMeshProUGUI>();
        GameTimer = GameTimerObj.GetComponent<TextMeshProUGUI>();

        Minimap = MinimapObj.GetComponent<RawImage>();

        MaskIcon = MaskIconObj.GetComponent<Image>();
        SprayIcon = SprayIconObj.GetComponent<Image>();
        PillIcon = PillIconObj.GetComponent<Image>();

        damageOverlay = damageOverlayObj.GetComponent<Image>();
        fadeOutOverlay = fadeOutOverlayObj.GetComponent<Image>();

        Star1 = Star1Obj.GetComponent<Image>();
        Star2 = Star2Obj.GetComponent<Image>();
        Star3 = Star3Obj.GetComponent<Image>();

        SummaryTitle = SummaryTitleObj.GetComponent<TextMeshProUGUI>();

        Summary1 = Summary1Obj.GetComponent<TextMeshProUGUI>();
        Summary2 = Summary2Obj.GetComponent<TextMeshProUGUI>();
        Summary3 = Summary3Obj.GetComponent<TextMeshProUGUI>();
        SummaryCoin = SummaryCoinObj.GetComponent<TextMeshProUGUI>();

        MouseSlider = MouseSliderObj.GetComponent<Slider>();
        VolumeSlider = VolumeSliderObj.GetComponent<Slider>();

        effect_audio = gameObject.AddComponent<AudioSource>();
    }

    delegate void UIdelegate(float amount);
    IEnumerator Animate(UIdelegate UIAction, float originalAmount, float targetAmount, float duration = animationDuration)
    {
        //Debug.Log(duration);
        float progress = 0;
        while (progress < duration)
        {
            progress += Time.deltaTime;
            UIAction(Mathf.Lerp(originalAmount, targetAmount, progress/ duration));
            yield return new WaitForEndOfFrame();
        }
    }

    Coroutine health_coroutine;
    /// <param name="fill">between 0 and 1</param>
    public void drawHealth(float fill)
    {
        if (health_coroutine != null) StopCoroutine(health_coroutine);
        health_coroutine = StartCoroutine(
            Animate((float _)=> {
                Debug.Log("healthfill:" + _.ToString());
                HealthFill.fillAmount = _;
            }, 
            HealthFill.fillAmount, fill)
            );
    }

    Coroutine energy_coroutine;
    /// <param name="fill">between 0 and 1</param>
    public void drawEnergy(float fill)
    {
        if (energy_coroutine != null) StopCoroutine(energy_coroutine);
        energy_coroutine = StartCoroutine(
            Animate((float _) => {
                EnergyFill.fillAmount = _;
            },
            EnergyFill.fillAmount, fill)
            );
    }

    public void drawCoinTarget(int count)
    {
        CoinTarget.text = "/"+count.ToString();
    }

    Coroutine coin_coroutine;
    /// <param name="count">number of coins</param>
    public void drawCoin(int count)
    {
        if (coin_coroutine != null) StopCoroutine(coin_coroutine);
        effect_audio.PlayOneShot(CashSound);

        coin_coroutine = StartCoroutine(
            Animate((float _) => {
                CoinCount.text = ((int)_).ToString();
            },
            float.Parse(CoinCount.text), count)
            );
    }

    /// <param name="count">number of masks</param>
    public void drawMask(int count)
    {
        drawItem(MaskIcon, MaskCount, count);
    }

    /// <param name="count">number of sprays</param>
    public void drawSpray(int count)
    {
        drawItem(SprayIcon, SprayCount, count);
    }

    /// <param name="count">number of pills</param>
    public void drawPill(int count)
    {
        drawItem(PillIcon, PillCount, count);
    }


    void drawItem(Image _icon, TextMeshProUGUI _text, int count)
    {
        if (count > 0)
        {
            _text.text = count.ToString();
            _icon.color = Color.white;
        }
        else
        {
            _text.text = "";
            _icon.color = ItemDisabledColor;
        }
    }

    public void drawTime(int secondsLeft)
    {
        GameTimer.text = (secondsLeft / 60).ToString() + ":" + (secondsLeft % 60).ToString("00");
    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    Coroutine damageOverlay_coroutine;
    public void ShowDamageOverlay(bool isPositive)
    {
        damageOverlay.color = isPositive ? RestoreHealthColor : DamageHealthColor;
        effect_audio.PlayOneShot(isPositive ? HealSound : DamageSound);

        if (damageOverlay_coroutine != null) StopCoroutine(damageOverlay_coroutine);
        damageOverlay_coroutine = StartCoroutine(
            Animate((float _) => {
                damageOverlay.color = ChangeAlpha(damageOverlay.color, Mathf.Sin(_));
            },
            0, Mathf.PI)
            );
    }

    Coroutine fadeoutOverlay_coroutine;
    public void FadeOut(float duration = fadeDuration)
    {
        StartCoroutine(Fade(0, 1, duration));
    }
    
    public void FadeIn(float duration = fadeDuration)
    {
        StartCoroutine(Fade(1, 0, duration));
    }

    IEnumerator Fade(float start, float finish, float duration)
    {
        if (fadeoutOverlay_coroutine != null) yield return fadeoutOverlay_coroutine;
        fadeoutOverlay_coroutine = StartCoroutine(
            Animate((float _) => {
                fadeOutOverlay.color = ChangeAlpha(fadeOutOverlay.color, _);
            },
            start, finish, duration)
            );
    }

    public static Color ChangeAlpha(Color color, float newAlpha)
    {
        color.a = newAlpha;
        return color;
    }

    public void ExitToLevelSelectionScreen()
    {
        ApplicationModel.ExitToLevelSelection = true;
        SceneManager.LoadScene("Main_Menu");
    }

    public void ShowSummary(int earnedCoins, int totalCoins, (bool, string)[] summary)
    {
        PauseMenu.SetActive(false);
        SummaryMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;

        SummaryTitle.text = summary[0].Item2;

        Summary1.text = summary[1].Item2;
        Summary2.text = summary[2].Item2;
        Summary3.text = summary[3].Item2;

        Star1.color = summary[1].Item1 ? Color.yellow: Color.grey;
        Star2.color = summary[2].Item1 ? Color.yellow : Color.grey;
        Star3.color = summary[3].Item1 ? Color.yellow : Color.grey;

        SummaryCoin.text = "You earned " + earnedCoins.ToString() + " shop coins. Total: " + totalCoins.ToString() + " shop coins.";
    }
    bool showMinimap = true;
    Coroutine minimap_coroutine;
    public void ToggleMiniMap()
    {
        showMinimap = !showMinimap;
        if (minimap_coroutine != null) StopCoroutine(minimap_coroutine);
        minimap_coroutine = StartCoroutine(
            Animate((float _) => {
                Minimap.color = ChangeAlpha(Color.white, _);
            },
            Minimap.color.a, showMinimap?0.2f:0.8f)
            );
    }

    public void setMouseSensitivity(float value)
    {
        ApplicationModel.GM.game.MouseSensitivity = value;
        ApplicationModel.GM.playerScript.mouseSensitivity = value;
    }

    public void setVolume(float value)
    {
        ApplicationModel.GM.game.Volume = value;
        AudioListener.volume = value;
    }

    public void setVisualMouseSensitivity(float value)
    {
        MouseSlider.value = value;
    }

    public void setVisualVolume(float value)
    {
        VolumeSlider.value = value;
    }
}
