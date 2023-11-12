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

            Cursor.visible = !Cursor.visible;


            if (Cursor.visible)
                Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;

        } 
        

        menu.SetActive(!menu.activeInHierarchy);
            if (menu != settingsHud)
            {
                anotherMenuUp = !anotherMenuUp;
            



                settingsHud.SetActive(!settingsHud.activeInHierarchy);
            }
        
    }

    
}
