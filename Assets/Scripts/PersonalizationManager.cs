using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PersonalizationManager : MonoBehaviour
{

    public AimPoint aimpoint;
    public PredictionMotor ship;

    [SerializeField]
    Slider distanceSlider;

    [SerializeField]
    TMP_InputField distanceDisplay;

    [SerializeField]
    TMP_Text showAimpointTickbox;
    [SerializeField]
    TMP_Text useAimpointTickbox;
    [SerializeField]
    TMP_Text useFOVTickbox;

    [SerializeField]
    InputManager inputManager;
    [SerializeField]
    VolumeManager volumeManager;
    public bool useFOVEffects { get; private set; } = true;

    private void Start()
    {
        ToggleUseFOVEffects(true);
        UpdateDistanceValue(PlayerPrefs.GetInt("distance",75));
        UpdateAimpointViewability(true);
        UpdateUseAimpoint(true);
    }

    public void ToggleUseFOVEffects(bool onStart = false)
    {
        useFOVEffects = !useFOVEffects;
        if (onStart)
        {
            useFOVEffects = PlayerPrefs.GetFloat("useFOV", 1) == 1 ? true : false;
        }
        useFOVTickbox.gameObject.SetActive(useFOVEffects);
        PlayerPrefs.SetFloat("useFOV", useFOVEffects ? 1 : 0);
    }

    public void ResetDistance()
    {
        UpdateDistanceValue("75");
    }

    public void UpdateAimpointViewability(bool onStart)
    {
        
        if(!onStart)
            PlayerPrefs.SetInt("showAimpoint", PlayerPrefs.GetInt("showAimpoint", 0) == 1 ? 0 : 1);
        bool showAimpoint = PlayerPrefs.GetInt("showAimpoint", 0) == 1 ? true : false;

        if (aimpoint != null)
        {
            aimpoint.UpdateViewability(showAimpoint);
        }

        useAimpointTickbox.gameObject.SetActive(showAimpoint);


    }


    public void UpdateDistanceValue(float value)
    {
        if (value < distanceSlider.minValue)
        return;
        


        distanceDisplay.text = value.ToString();
        if (aimpoint != null)
        {

            aimpoint.UpdatePosition((int)value);
        }
        PlayerPrefs.SetInt("distance", (int)value);

    }
    public void UpdateDistanceValue(string value)
    {

        if (value == null || value == "" || float.Parse(value) < distanceSlider.minValue)
            return;

        float floatValue = float.Parse(value);
        if (value != null)
        {
            distanceSlider.value = floatValue;

            UpdateDistanceValue(floatValue);

        }
        else
        {
            Debug.LogError("Invalid value");
        }
    }


    public void UpdateUseAimpoint(bool onStart = false)
    {
        if (!onStart)
            PlayerPrefs.SetInt("useAimpoint", PlayerPrefs.GetInt("useAimpoint", 1) == 1 ? 0 : 1);
        bool useAimpoint = PlayerPrefs.GetInt("useAimpoint", 1) == 1 ? true : false;

        if (ship != null)
        {
            ship.ChangeBlastersUseAimpoint(useAimpoint);
        }
        showAimpointTickbox.gameObject.SetActive(useAimpoint);
    }

    [SerializeField]
    GameObject areYouSurePanel;
    [SerializeField]
    ColorPicker colorPicker;
    [SerializeField]
    CustomColor[] customColors;
    public void SetAYSPanelVisible(bool setTo)
    {
        areYouSurePanel.SetActive(setTo);
    }

    public void ResetAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        inputManager.RestoreDefaultControls();
        volumeManager.RemoveAllListeners();
        colorPicker.OnClickPickRed();
        foreach(CustomColor color in customColors)
        {
            color.ResetColor();
        }

        colorPicker.Awake();
        volumeManager.Start();
        inputManager.Start();
        Start();
        SetAYSPanelVisible(false);
    }

}
