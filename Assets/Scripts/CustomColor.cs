using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CustomColor : MonoBehaviour
{
    public Color custom1;
    public bool hasColor = false;

    [SerializeField]
    ColorPicker picker;
   
    public GameObject selectingText;

    public void ResetColor()
    {
        custom1 = Color.white;
        GetComponent<Image>().color = custom1;
        hasColor = false;
        picker.laserColor = custom1;
    }

    private void Awake()
    {
        
        custom1 = new Color(PlayerPrefs.GetFloat("Custom1R",1), PlayerPrefs.GetFloat("Custom1G",1), PlayerPrefs.GetFloat("Custom1B",1), PlayerPrefs.GetFloat("Custom1A",1));
        if (custom1 == Color.white)
        {
            hasColor = false;
        }
        else hasColor = true;
        GetComponent<Image>().color = custom1;
        
        
    }

    public void ChangeColor()
    {
        custom1 = picker.laserColor;
        GetComponent<Image>().color = custom1;
        hasColor = true;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("Custom1R", custom1.r);
        PlayerPrefs.SetFloat("Custom1G", custom1.g);
        PlayerPrefs.SetFloat("Custom1B", custom1.b);
        PlayerPrefs.SetFloat("Custom1A", custom1.a);

    }
}
