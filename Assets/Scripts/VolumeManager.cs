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
    bool masterMute;

    private void Start()
    {
        try
        {
            masterMute = PlayerPrefs.GetInt("MasterMute", 1) == 0 ? true : false;

            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);


            UpdateMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
            UpdateMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));

            masterSlider.onValueChanged.AddListener(val => UpdateMasterVolume(val));
            musicSlider.onValueChanged.AddListener(val => UpdateMusicVolume(val));

            masterMuteButton.onClick.AddListener(() => ToggleMasterMute());
            musicMuteButton.onClick.AddListener(() => ToggleMusicMute());

            SetupMusicMute(PlayerPrefs.GetInt("MusicMute", 1) == 0 ? true : false);
            SetupMasterMute(PlayerPrefs.GetInt("MasterMute", 1) == 0 ? true : false);
        } 
        catch
        {
            //Nothing to do
        }
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
        masterMute = !masterMute;        

        if (masterMute)
        {
            masterImage.sprite = masterOff;
            AudioListener.volume = 0;
            masterSlider.onValueChanged.RemoveAllListeners();
        }
        else
        {
            masterImage.sprite = masterOn;
            masterSlider.onValueChanged.AddListener(val => UpdateMasterVolume(val));
            UpdateMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        }

        PlayerPrefs.SetInt("MasterMute", masterMute ? 0 : 1);

    }
    void ToggleMusicMute()
    {
        music.mute = !music.mute;
        if (music.mute)
        {
            musicImage.sprite = musicOff;
        }
        else
        {
            musicImage.sprite = musicOn;
        }

        PlayerPrefs.SetInt("MusicMute", music.mute ? 0 : 1);

    }

    void SetupMusicMute(bool isMuted)
    {
        if(isMuted)
        {
            musicImage.sprite = musicOff;
        }
        else
        {
            musicImage.sprite = musicOn;
        }
        music.mute = isMuted;
    }

    void SetupMasterMute(bool isMuted)
    {
        if (isMuted)
        {
            masterImage.sprite = masterOff;
            AudioListener.volume = 0;
            masterSlider.onValueChanged.RemoveAllListeners();
        }
        else
        {
            masterImage.sprite = masterOn;
            masterSlider.onValueChanged.AddListener(val => UpdateMasterVolume(val));
            UpdateMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        }

    }
}
