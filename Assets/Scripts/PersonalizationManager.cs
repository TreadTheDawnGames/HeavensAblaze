using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PersonalizationManager : MonoBehaviour
{
    public bool aimpointActive = true;
    public float distance = 28f;


    [SerializeField]
    Slider aimpointSlider;

    private void Start()
    {
        aimpointSlider.onValueChanged.AddListener((v) =>
        {
            distance = v;
        });
    }

    public void ResetDistance()
    {
        aimpointSlider.value = 28;
        distance = 28;
    }

    public void ChangeAimpointViewability(TMP_Text tickmark)
    {
        aimpointActive = !aimpointActive;
        tickmark.gameObject.SetActive(aimpointActive);
        
    }
}
