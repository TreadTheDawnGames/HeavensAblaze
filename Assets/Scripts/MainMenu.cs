using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    private bool anotherMenuUp = false;

    public GameObject settingsHud;
    public GameObject controlsHud;
    public GameObject personalizationHud;

    public PredictionMotor ship;

    [SerializeField]
    NetworkUIV2 networkUIV2;

#if UNITY_EDITOR



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {


            if (!anotherMenuUp)
            {
                ToggleMenu(settingsHud);
            }

        }


    }
#else
private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            

            if (!anotherMenuUp)
            {
                ToggleMenu(settingsHud);
            }
            
        }
        

    }
#endif
    public void ToggleMenu(GameObject menu)
    {

        if (menu == settingsHud)
        {
            if (networkUIV2.clientStarted)
            {

                Cursor.visible = !Cursor.visible;

                Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
            }



        }

        if (ship != null)
        {
            if (Cursor.visible)
                ship.playerShip.Disable();
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