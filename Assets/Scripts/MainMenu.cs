using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class MainMenu : MonoBehaviour
{
    private bool anotherMenuUp = false;

    public GameObject settingsHud;
    public GameObject controlsHud;
    public GameObject personalizationHud;


    public PredictionMotor ship;
    public InputManager inputManager;

    [SerializeField]
    NetworkUIV2 networkUIV2;

    GameObject currentMenu;

    [SerializeField]
    AnimateLogo logo;


   
    

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

#if UNITY_EDITOR
    IEnumerator ToggleMenusCoroutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (!anotherMenuUp)
                    ToggleMainMenu();
                else
                    ToggleMenu(currentMenu);
            }
            yield return null;
        }
    }


#else
IEnumerator ToggleMenusCoroutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!anotherMenuUp)
                    ToggleMainMenu();
                else
                    ToggleMenu(currentMenu);
            }
            yield return null;
        }
    }
#endif

    private void ToggleMenus(InputAction.CallbackContext context)
    {
        if (!anotherMenuUp)
            ToggleMainMenu();
        else
            ToggleMenu(currentMenu);
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
    public void SetShipPartDestroyed(ShipPart caller, bool value = true)
    {
        if(caller is Cockpit)
        {
            cockpitDestroyed = value;
        }
        else if(caller is MainBody)
        {
            mainBodyDestroyed = value;
        }
    }

    public void ToggleMenu(GameObject menu)
    {
        currentMenu = menu;
        if (networkUIV2.clientStarted && (!cockpitDestroyed || !mainBodyDestroyed))
        {
            networkUIV2.SetNetUIVisability(!menu.activeInHierarchy);
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