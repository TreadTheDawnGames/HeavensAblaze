using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour
{
    public PlayerShip inputActions;

    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    public TMP_Dropdown myDrop;
    public bool blastersUsesAimpoint = true;

    public bool invertThrust, invertLift, invertLateral, invertPitch, invertRoll, invertYaw = false;

    public GameObject invThrTick;
    public GameObject invLatTick;
    public GameObject invLifTick;
    public GameObject invRollTick;
    public GameObject invPitTick;
    public GameObject invYawTick;

    public PredictionMotor ship;
    public bool menuUp = false;

    List<RebindUI> rebindButtons = new List<RebindUI>();

    #region inversions
    public void InvThrust(bool toDefault = false)
    {
        invertThrust = toDefault ? false : !invertThrust;

        invThrTick.SetActive(invertThrust);

        if (ship != null)
        {
            ship.invertThrust = invertThrust ? -1 : 1;

        }

        PlayerPrefs.SetInt("invertThrust", invertThrust == true ? 1 : 0);
    }
    public void InvLift(bool toDefault = false)
    {
        invertLift = toDefault ? false : !invertLift;
        invLifTick.SetActive(invertLift);
        if (ship != null)
        {
            ship.invertLift = invertLift ? -1 : 1;

        }
        PlayerPrefs.SetInt("invertLift", invertLift == true ? 1 : 0);

    }
    public void InvLateral(bool toDefault = false)
    {
        invertLateral = toDefault ? false : !invertLateral;
        invLatTick.SetActive(invertLateral);
        if (ship != null)
        {
            ship.invertLateral = invertLateral ? -1 : 1;

        }
        PlayerPrefs.SetInt("invertLateral", invertLateral == true ? 1 : 0);

    }
    public void InvRoll(bool toDefault = false)
    {
        invertRoll = toDefault ? false : !invertRoll;
        invRollTick.SetActive(invertRoll);
        if (ship != null)
        {
            ship.invertRoll = invertRoll ? -1 : 1;

        }
        PlayerPrefs.SetInt("invertRoll", invertRoll == true ? 1 : 0);

    }
    public void InvPitch(bool toDefault = false)
    {
        invertPitch = toDefault ? false : !invertPitch;
        invPitTick.SetActive(invertPitch);
        if (ship != null)
        {
            ship.invertPitch = invertPitch ? -1 : 1;

        }
        PlayerPrefs.SetInt("invertPitch", invertPitch == true ? 1 : 0);
    }
    public void InvYaw(bool toDefault = false)
    {
        invertYaw = toDefault ? false : !invertYaw;
        invYawTick.SetActive(invertYaw);
        if (ship != null)
        {
            ship.invertYaw = invertYaw ? -1 : 1;

        }
        PlayerPrefs.SetInt("invertYaw", invertYaw == true ? 1 : 0);
    }
    #endregion





    public void ChangeInputType(int num)
    {

        if(ship != null)
        {
            ship.playerShip.Disable();
        }

        inputActions.Disable();

        switch (num)
        {
            case 0:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Joystick;
                }
                    inputActions.Joystick.Enable();
                PlayerPrefs.SetInt("inputType", 0);
                myDrop.value = 0;
                break;

            case 1:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Keyboard;
                }
                    inputActions.Keyboard.Enable();
                PlayerPrefs.SetInt("inputType", 1);
                myDrop.value = 1;
                break;

            case 2:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Gamepad;
                }
                    inputActions.Gamepad.Enable();
                PlayerPrefs.SetInt("inputType", 2);
                myDrop.value = 2;
                break;

            case 3:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Mouse;
                }
                    inputActions.Mouse.Enable();
                PlayerPrefs.SetInt("inputType", 3);
                myDrop.value = 3;
                break;

            case 4:
                if (ship != null)
                    ship.playerShip.Disable();
                break;

            default:
                Debug.LogError("Input type does not exist");
                break;

                //UpdateUI

        }



        foreach (RebindUI button in rebindButtons)
        {
            button.UpdateUI();
            Debug.Log("Updated " + button.name);
        }
        if (ship != null)
        {
            inputActions=ship.playerShip;

            //ship.playerShip.Disable();
        }
        //-inputActions.Disable();
    }
    public void ChangeInputTypeAndActivateShip(int num)
    {
        ship.playerShip.Disable();
        ship.playerShip = inputActions;

        switch (num)
        {
            case 0:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Joystick;
                    ship.playerShip.Joystick.Enable();
                }
                PlayerPrefs.SetInt("inputType", 0);
                myDrop.value = 0;
                break;

            case 1:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Keyboard;
                    ship.playerShip.Keyboard.Enable();
                }
                PlayerPrefs.SetInt("inputType", 1);
                myDrop.value = 1;
                break;

            case 2:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Gamepad;
                    ship.playerShip.Gamepad.Enable();
                }
                PlayerPrefs.SetInt("inputType", 2);
                myDrop.value = 2;
                break;

            case 3:
                if (ship != null)
                {
                    ship.inputType = PredictionMotor.InputType.Mouse;
                    ship.playerShip.Mouse.Enable();
                }
                PlayerPrefs.SetInt("inputType", 3);
                myDrop.value = 3;
                break;

            case 4:
                if (ship != null)
                    ship.playerShip.Disable();
                break;

            default:
                Debug.LogError("Input type does not exist");
                break;

                //UpdateUI

        }

        ChangeInputType(num);
       // if (ship != null)
            RebindUI.excludeMouse = inputActions.Mouse.enabled;

        foreach (RebindUI button in rebindButtons)
        {
            button.UpdateUI();
            Debug.Log("Updated " + button.name);
        }
    }



    private void Start()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerShip();
        }

        invertThrust = PlayerPrefs.GetInt("invertThrust") == 1 ? true : false;
        invThrTick.SetActive(invertThrust);
        invertLift = PlayerPrefs.GetInt("invertLift") == 1 ? true : false;
        invLifTick.SetActive(invertLift);
        invertLateral = PlayerPrefs.GetInt("invertLateral") == 1 ? true : false;
        invLatTick.SetActive(invertLateral);
        invertRoll = PlayerPrefs.GetInt("invertRoll") == 1 ? true : false;
        invRollTick.SetActive(invertRoll);
        invertPitch = PlayerPrefs.GetInt("invertPitch") == 1 ? true : false;
        invPitTick.SetActive(invertPitch);
        invertYaw = PlayerPrefs.GetInt("invertYaw") == 1 ? true : false;
        invYawTick.SetActive(invertYaw);


        rebindButtons = FindObjectsOfType<RebindUI>(true).ToList();

        

        ChangeInputType(PlayerPrefs.GetInt("inputType", 0));


        //Come back to this and make the control types save between sessions.

        //inputActions.Joystick.Enable();

        sensitivityValue = PlayerPrefs.GetInt("sensitivity", 100);

        sensitivityDisplay.text = sensitivityValue.ToString();
        //JoystickControls();
    }

   

    public void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
    {
        

        InputAction action = inputActions.asset.FindAction(actionName);
        if (action == null || action.bindings.Count <= 0)
        {
            Debug.Log("Couldn't find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                DoRebind(action, firstPartIndex, statusText, true, excludeMouse);
        }
        else
            DoRebind(action, bindingIndex, statusText, false, excludeMouse);

        
    }

    private void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
    {
        if (actionToRebind == null || bindingIndex < 0)
            return;

        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            if (allCompositeParts)
            {
                var nextBindingIndex = bindingIndex + 1;
                if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isPartOfComposite)
                    DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
            }
            SaveBindingOverride(actionToRebind);
            rebindComplete?.Invoke();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();

            rebindCanceled?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        if (excludeMouse)
            rebind.WithControlsExcluding("Mouse");

        rebindStarted?.Invoke(actionToRebind, bindingIndex);
        rebind.Start(); //actually starts the rebinding process


    }

    public string GetBindingName(string actionName, int bindingIndex)
    {
        if (inputActions == null)
            inputActions = new PlayerShip();

        InputAction action = inputActions.asset.FindAction(actionName);
        return action.GetBindingDisplayString(bindingIndex);
    }

    private static void SaveBindingOverride(InputAction action)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
        }
    }

    public void LoadBindingOverride(string actionName)
    {
     //   if (inputActions == null)
           // inputActions = new PlayerShip();

        InputAction action = inputActions.asset.FindAction(actionName);

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))           
                action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            
        }
    }


    
    public void ResetBinding(string actionName, int bindingIndex)
    {

        InputAction action = inputActions.asset.FindAction(actionName);

        if(action==null||action.bindings.Count<=bindingIndex)
        {
            Debug.LogError("Could not find action or binding");
            return;
        }



        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count; i++)
            {
                action.RemoveBindingOverride(i);
                //action.Disable();
            }
        }
        else
        {

            action.RemoveBindingOverride(bindingIndex);
            //action.Disable();
        }


        SaveBindingOverride(action);
    }



    public Slider sensitivitySlider;
    public int sensitivityValue = 100;
    public TMP_InputField sensitivityDisplay;

    public AnimationCurve curve;

    public void UpdateSensitivityValue(float value)
    {
        sensitivityValue = (int)value;
        sensitivityDisplay.text = sensitivityValue.ToString();
            PlayerPrefs.SetInt("sensitivity", sensitivityValue);
    }
    public void UpdateSensitivityValue(string value)
    {
        if (value == null||value=="")
            return;

        float floatValue = float.Parse(value);
        if (value != null)
        {
            sensitivitySlider.value = floatValue;
            sensitivityValue = (int)floatValue;
            PlayerPrefs.SetInt("sensitivity", sensitivityValue);

        }
        else
        {
            Debug.LogError("Invalid value");
        }
    }

    
    public void RestoreDefaultControls()
    {
       
        for(int i = 3; i>=0; i--)
        {

        ChangeInputType(i);
        foreach(RebindUI button in rebindButtons)
        {
            button.ResetBinding();
        }
        }
        
        UpdateSensitivityValue(100);
        InvThrust(true);
        InvLift(true);
        InvLateral(true);
        InvRoll(true);
        InvPitch(true);
        InvYaw(true);

        if(ship != null)
        {
       //     ship.playerShip.Disable();
        }
       // inputActions.Disable();
    }


    public bool debugUpdateUI = false;
    public bool debugRunOnce = false;
    private void Update()
    {
        if (debugUpdateUI)
        {
            debugUpdateUI = false;
           
                //inputActions.Mouse.Enable();
            
            foreach (RebindUI button in rebindButtons)
            {
                button.UpdateUI();
            }
        }
    }
    

    

}