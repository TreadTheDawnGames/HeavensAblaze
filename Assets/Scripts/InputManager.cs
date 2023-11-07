using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;
using TMPro.Examples;
using Unity.VisualScripting;

public class InputManager : MonoBehaviour
{
    public PlayerShip inputActions;

    public static event Action rebindComplete;
    public static event Action rebindCanceled;
    public static event Action<InputAction, int> rebindStarted;

    public TMP_Dropdown myDrop;
    public enum ManagedInputType { Joystick, Keyboard, Gamepad, Mouse, Disabled }
    public ManagedInputType inputType;
    public bool blastersUsesAimpoint = true;

    public bool invertThrust, invertLift, invertLateral, invertPitch, invertRoll, invertYaw = false;

    public GameObject invThrTick;
    public GameObject invLatTick;
    public GameObject invLifTick;
    public GameObject invRollTick;
    public GameObject invPitTick;
    public GameObject invYawTick;

    public void ChangeShipUseAimpoint(GameObject text)
    {
        blastersUsesAimpoint = !blastersUsesAimpoint;
        text.SetActive(!text.activeInHierarchy);
    }

    #region inversions
    public void InvThrust(GameObject text)
    {
        invertThrust = !invertThrust;
        text.SetActive(!text.activeInHierarchy);
        PlayerPrefs.SetInt("invertThrust", invertThrust == true ? 1 : 0);
    }
    public void InvLift(GameObject text)
    {
    
        invertLift = !invertLift;
        text.SetActive(!text.activeInHierarchy);
        PlayerPrefs.SetInt("invertLift", invertLift == true ? 1 : 0);

    }
    public void InvLateral(GameObject text)
    {
        invertLateral = !invertLateral;
        text.SetActive(!text.activeInHierarchy);
        PlayerPrefs.SetInt("invertLateral", invertLateral == true ? 1 : 0);

    }
    public void InvRoll(GameObject text)
    {
        invertRoll = !invertRoll;
        text.SetActive(!text.activeInHierarchy);
        PlayerPrefs.SetInt("invertRoll", invertRoll == true ? 1 : 0);

    }
    public void InvPitch(GameObject text)
    {
        invertPitch = !invertPitch;
        text.SetActive(!text.activeInHierarchy);
        PlayerPrefs.SetInt("invertPitch", invertPitch == true ? 1 : 0);
    }
    public void InvYaw(GameObject text)
    {
        invertYaw = !invertYaw;
        text.SetActive(!text.activeInHierarchy);
        PlayerPrefs.SetInt("invertYaw", invertYaw == true ? 1 : 0);
    }
    #endregion






    public void ChangeInputType(Int32 num)
    {
        if (num == 0)
        {
            JoystickControls();
        }
        if (num == 1)
        {
            KeyboardControls();
        }
        if (num == 2)
        {
            GamepadControls();
        }
        if (num == 3)
        {
            MouseControls();
        }

    }

    public void JoystickControls()
    {
        inputType = ManagedInputType.Joystick;
        RebindUI.excludeMouse = true;
    }
    public void KeyboardControls()
    {
        inputType = ManagedInputType.Keyboard;
        RebindUI.excludeMouse = true;

    }
    public void MouseControls()
    {
        inputType = ManagedInputType.Mouse;
        RebindUI.excludeMouse = false;
        
    }
    public void GamepadControls()
    {
        inputType = ManagedInputType.Gamepad;
        RebindUI.excludeMouse = true;

    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("invertThrust") == 1)
        {
            invertThrust = true;
            invThrTick.SetActive(invertThrust);
        }
        else
        {
            invertThrust = false;
            invThrTick.SetActive(invertThrust);

        }
        if (PlayerPrefs.GetInt("invertLift") == 1)
        {
            invertLift = true;
            invLifTick.SetActive(invertLift);
        }
        else
        {
            invertLift = false;
            invLifTick.SetActive(invertLift);

        }
        if (PlayerPrefs.GetInt("invertLateral") == 1)
        {
            invertLateral = true;
            invLatTick.SetActive(invertLateral);
        }
        else
        {
            invertLateral = false;
            invLatTick.SetActive(invertLateral);

        }
        if (PlayerPrefs.GetInt("invertRoll") == 1)
        {
            invertRoll = true;
            invRollTick.SetActive(invertRoll);
        }
        else
        {
            invertRoll = false;
            invRollTick.SetActive(invertRoll);

        }
        if (PlayerPrefs.GetInt("invertPitch") == 1)
        {
            invertPitch = true;
            invPitTick.SetActive(invertPitch);
        }
        else
        {
            invertPitch = false;
            invPitTick.SetActive(invertPitch);

        }
        if (PlayerPrefs.GetInt("invertYaw") == 1)
        {
            invertYaw = true;
            invYawTick.SetActive(invertYaw);
        }
        else
        {
            invertYaw = false;
            invYawTick.SetActive(invertYaw);

        }







        if (inputActions == null)
        {
            inputActions = new PlayerShip();
        }

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
        if (inputActions == null)
            inputActions = new PlayerShip();

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
            Debug.Log("Could not find action or binding");
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                Debug.Log(action.GetBindingDisplayString(bindingIndex));
                action.RemoveBindingOverride(i);
                Debug.Log(action.GetBindingDisplayString(bindingIndex));
                
            }
        }
        else
            action.RemoveBindingOverride(bindingIndex);

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

    


}