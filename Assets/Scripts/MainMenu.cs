using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private bool anotherMenuUp = false;

    public GameObject settingsHud;
    public GameObject controlsHud;
    public GameObject personalizationHud;


    public PredictionMotor ship;
    public InputManager inputManager;

    [SerializeField]
    public NetworkUIV2 networkUIV2;

    GameObject currentMenu;

    [SerializeField]
    AnimateLogo logo;

    

#if UNITY_EDITOR

    KeyCode escapeKey = KeyCode.M;

#else

    KeyCode escapeKey = KeyCode.Escape;
#endif
    

    private void Start()
    {
        logo.animationComplete += ToggleMenus;
    }

    private void OnDestroy()
    {
        logo.animationComplete -= ToggleMenus;        
    }
    void ToggleMenus()
    {
        StartCoroutine(ToggleMenusCoroutine());
    }
    
    IEnumerator ToggleMenusCoroutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(escapeKey))
            {
                if (!anotherMenuUp)
                    ToggleMainMenu();
                else
                    ToggleMenu(currentMenu);
            }
            yield return null;
        }
    }

    

    public void ToggleMainMenu()
    {
        if (!anotherMenuUp)
        {
            ToggleMenu(settingsHud);
            inputManager.menuUp = !inputManager.menuUp;
        }
    }

    [SerializeField]
    public bool cockpitDestroyed  = false;
    [SerializeField]
    public bool mainBodyDestroyed  = false;

    public void ResetShipDestroyed()
    {
        cockpitDestroyed = mainBodyDestroyed = false;
    }
   

    public void ToggleMenu(GameObject menu)
    {
        currentMenu = menu;
        bool changeTo = !menu.activeInHierarchy;
        if (networkUIV2.clientStarted)
        {
            networkUIV2.SetNetUIVisability(changeTo);
        }
       
        if (ship != null)
        {

            if (Cursor.visible)
            {
                ship.playerShip.Disable();
            }
            else
                ship.inputManager.ChangeInputTypeAndActivateShip(PlayerPrefs.GetInt("inputType", 0));
        }

        menu.SetActive(!menu.activeInHierarchy);

        if (menu != settingsHud)
        {
            anotherMenuUp = !anotherMenuUp;

            settingsHud.SetActive(!settingsHud.activeInHierarchy);
        }

    }


}