using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerDisplay : MonoBehaviour
{
    [SerializeField]
    ColorPicker picker;
    private void Start()
    {
        if (picker == null)
        {
            picker = FindObjectOfType<ColorPicker>();
        }
        picker.ColorChanged += UpdateColor;
    }

    private void OnDestroy()
    {
        picker.ColorChanged -= UpdateColor;        
    }

    public void UpdateColor()
    {
        GetComponent<Image>().material.color = picker.laserColor;
    }
    

}
