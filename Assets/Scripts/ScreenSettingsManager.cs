using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class ScreenSettingsManager : MonoBehaviour
{

    public bool fullScreen = false;
    public GameObject fullScreenTickmark;
    [SerializeField]
    Button fullScreenTickbox;
    [SerializeField]
    TMP_Dropdown resolutionsDropdown;
    List<Resolution> resolutions = new List<Resolution>();

    private void Start()
    {
        SetupResolutions();

        fullScreenTickbox.onClick.AddListener(() => ToggleFullScreen());
        resolutionsDropdown.onValueChanged.AddListener((Int) => SetResolution(Int));
        fullScreen = Screen.fullScreen;
        fullScreenTickmark.SetActive(fullScreen);
    }
    
    private void OnDestroy()
    {
        fullScreenTickbox.onClick.RemoveAllListeners();
        resolutionsDropdown.onValueChanged.RemoveAllListeners();

    }
    public void ToggleFullScreen()
    {
        fullScreen = !fullScreen;
        fullScreenTickmark.SetActive(fullScreen);
        Screen.fullScreen = fullScreen;
        
    }

    private void SetupResolutions()
    {
        List<string> resolutionStrings = new List<string>();

        foreach (Resolution resolution in Screen.resolutions)
        {
            resolutions.Add(resolution);
        }
        resolutions.Reverse();

        foreach(Resolution resolution in resolutions)
        {
            resolutionStrings.Add(resolution.ToString());
        }
        //resolutionStrings.Reverse();
        resolutionsDropdown.AddOptions(resolutionStrings);
        resolutionsDropdown.captionText.text = Screen.currentResolution.ToString();
        resolutionsDropdown.RefreshShownValue();

    }

    void SetResolution(Int32 index)
    {
        
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, fullScreen);
    }

}
