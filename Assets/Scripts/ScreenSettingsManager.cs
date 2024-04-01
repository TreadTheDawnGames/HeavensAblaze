using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;

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

        Resolution res = new Resolution();
        res.width = PlayerPrefs.GetInt("ScreenWidth", Screen.width);
        res.height = PlayerPrefs.GetInt("ScreenHeight", Screen.height);
        print("width "+PlayerPrefs.GetInt("ScreenWidth", 0));
        print("height "+PlayerPrefs.GetInt("ScreenHeight", 0));
        print(resolutions.IndexOf(res));

        foreach (Resolution resolution in resolutions)
        {
            if(resolution.width == res.width && resolution.height == res.height)
            {
                resolutionsDropdown.SetValueWithoutNotify(resolutions.IndexOf(resolution));
                break;
            }
        }

//            StartCoroutine(ChangeValue(resolutions.IndexOf(res)));
    }

    IEnumerator ChangeValue(int newValue)
    {
        resolutionsDropdown.Select();
        yield return new WaitForEndOfFrame();
        //resolutionsDropdown.RefreshShownValue();
    }

    void SetResolution(Int32 index)
    {

        print(resolutions[index]);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, fullScreen);
        PlayerPrefs.SetInt("ScreenWidth", resolutions[index].width);
        PlayerPrefs.SetInt("ScreenHeight", resolutions[index].height);
        print("width " + PlayerPrefs.GetInt("ScreenWidth", 0));
        print("height " + PlayerPrefs.GetInt("ScreenHeight", 0));

    }

}
