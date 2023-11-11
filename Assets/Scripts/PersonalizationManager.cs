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

    private void Start()
    {
        UpdateDistanceValue(PlayerPrefs.GetInt("distance"));
        UpdateAimpointViewability(true);
        UpdateUseAimpoint(true);
    }

    public void ResetDistance()
    {
        UpdateDistanceValue("75");
    }

    public void UpdateAimpointViewability(bool onStart)
    {
        
        if(!onStart)
            PlayerPrefs.SetInt("showAimpoint", PlayerPrefs.GetInt("showAimpoint", 1) == 1 ? 0 : 1);
        bool showAimpoint = PlayerPrefs.GetInt("showAimpoint", 1) == 1 ? true : false;

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


    public void UpdateUseAimpoint(bool onStart)
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



}
