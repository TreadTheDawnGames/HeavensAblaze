using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuScreen : MonoBehaviour
{
    private bool menuUp = false;

    public GameObject menu;


    private void Awake()
    {
        menu = this.gameObject;
        menuUp = menu.activeInHierarchy;
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menuUp);
    }

}
