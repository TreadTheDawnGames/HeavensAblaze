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

#if UNITY_EDITOR

    
    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!anotherMenuUp)
                ToggleMainMenu();
            else
                ToggleMenu(currentMenu);
        }
    }
#else
private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           if (!anotherMenuUp)
                ToggleMainMenu();
            else
                ToggleMenu(currentMenu);
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

    public void ToggleMenu(GameObject menu)
    {
        currentMenu = menu;
        if (menu == settingsHud)
        {
            if (networkUIV2.clientStarted)
            {

                Cursor.visible = !Cursor.visible;

                Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            
                
                networkUIV2.ToggleNetUIVisability(!menu.activeInHierarchy);
            }


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