using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerDisplay : MonoBehaviour
{
    ColorPicker picker;
    private void Awake()
    {
        picker = FindObjectOfType<ColorPicker>();
    }

    private void Update()
    {
        GetComponent<Image>().material.color = picker.laserColor;
    }

    

}
