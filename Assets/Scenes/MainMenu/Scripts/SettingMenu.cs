using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] GameObject MouseSliderObj;
    [SerializeField] GameObject VolumeSliderObj;

    Slider MouseSlider;
    Slider VolumeSlider;

    public AudioMixer audioMixer;

    private void Start()
    {
        MouseSlider = MouseSliderObj.GetComponent<Slider>();
        VolumeSlider = VolumeSliderObj.GetComponent<Slider>();
        setVisualMouseSensitivity(Main_Menu.game.MouseSensitivity);
        setVisualVolume(Main_Menu.game.Volume);
    }

    public void setMouseSensitivity(float value)
    {
        Main_Menu.game.MouseSensitivity = value;
    }

    public void setVolume(float value)
    {
        Main_Menu.game.Volume = value;
        AudioListener.volume = value;
    }

    public void setVisualVolume(float value)
    {
        VolumeSlider.value = value;
    }
    public void setVisualMouseSensitivity(float value)
    {
        MouseSlider.value = value;
    }
}
