using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class RebindUI : MonoBehaviour
{
    [SerializeField]
    InputManager inputManager;

    [SerializeField]
    private InputActionReference inputActionReference; //this is on the Scriptable Object

    [SerializeField]
    public static bool excludeMouse = true;

    [Range(0, 10)]
    [SerializeField]
    private int selectedBinding;
    [SerializeField]
    InputBinding.DisplayStringOptions displayStringOptions;
    [Header("Binding Info - DO NOT EDIT")]
    [SerializeField]
    private InputBinding inputBinding;
    private int bindingIndex;

    private string actionName;

    [Header("UI Fields")]
    [SerializeField]
    private TMP_Text actionText;
    [SerializeField]
    private Button rebindButton;
    [SerializeField]
    private TMP_Text rebindText;
    [SerializeField]
    private Button resetButton;

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(()=>ResetBinding());

        inputManager.ChangeInputType(PlayerPrefs.GetInt("inputType", 0));

        if (inputActionReference != null)
        {
            GetBindingInfo();
            inputManager.LoadBindingOverride(actionName);
            UpdateUI();
        }

        InputManager.rebindComplete += UpdateUI;
        InputManager.rebindCanceled += UpdateUI;
    }

    private void OnDisable()
    {

        InputManager.rebindComplete -= UpdateUI;        
        InputManager.rebindCanceled -= UpdateUI;
    }


    private void OnValidate()
    {
        if (inputActionReference == null)
            return;

        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if (inputActionReference.action != null)
            actionName = inputActionReference.action.name;

        if(inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    public void UpdateUI()
    {
        if (actionText != null)
            actionText.text = actionName;


        if (rebindText != null)
        {
            if (Application.isPlaying)
            {
                rebindText.text = inputManager.GetBindingName(actionName, bindingIndex);
            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);

            }


        }


    }

    private void DoRebind()
    {
        inputManager.StartRebind(actionName, bindingIndex, rebindText, excludeMouse);

    }

    public void ResetBinding()
    {
        inputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }
}
