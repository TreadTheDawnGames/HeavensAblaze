using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class VolumeManager : MonoBehaviour
{
    [SerializeField]
    Slider masterSlider, musicSlider;

    [SerializeField]
    Button masterMuteButton;
    [SerializeField]
    Button musicMuteButton;

    [SerializeField]
    Image masterImage;
    [SerializeField]
    Image musicImage;

    [SerializeField]
    Sprite masterOn, masterOff, musicOn, musicOff;

    [SerializeField]
    AudioSource music;

    public bool isIngame = false;

    private void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);

        UpdateMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        UpdateMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));

        masterSlider.onValueChanged.AddListener(val => UpdateMasterVolume(val));
        musicSlider.onValueChanged.AddListener(val => UpdateMusicVolume(val));

        masterMuteButton.onClick.AddListener(() => ToggleMasterMute());
        musicMuteButton.onClick.AddListener(() => ToggleMusicMute());
    }


    void UpdateMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    void UpdateMusicVolume(float volume)
    {
        if(isIngame)
            volume *= 0.5f;

        music.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    void ToggleMasterMute()
    {
        if (masterImage.sprite == masterOn)
            masterImage.sprite = masterOff;
        else
            masterImage.sprite = masterOn;
       
    }
    void ToggleMusicMute()
    {
        if (musicImage.sprite == musicOn)
            musicImage.sprite = musicOff;
        else
            musicImage.sprite = musicOn;
       
    }
}
