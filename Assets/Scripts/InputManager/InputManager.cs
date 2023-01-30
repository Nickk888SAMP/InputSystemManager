using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour 
{
    private static InputManager instance = null;
    public static InputManager Instance { get { return instance; } }

    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private InputActionReference[] inputReferences;
    [SerializeField] private Dictionary<InputActionReference, InputAction> inputActions = new Dictionary<InputActionReference, InputAction>();

    public InputActionReference[] InputReferences { get { return inputReferences; } }

    #region Callbacks

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(var reference in inputReferences)
        {
            InitializeAction(reference.inputActionName, out InputAction action);
            inputActions.Add(reference, action);
        }
    }

    private void Update() 
    {
        GetMouseScreenPosition();
    }

    #endregion

    #region Public Methods

    
    public bool GetInputActionValue(InputActionReference reference, out Vector2 value)
    {
        if(GetAction(reference, out InputAction inputAction))
        {
            value = inputAction.ReadValue<Vector2>();
            return true;
        }
        value = Vector2.zero;
        return false;
    }
    public bool GetInputActionValue(string inputActionName, out Vector2 value)
    {
        return GetInputActionValue(GetInputReferenceByName(inputActionName), out value);
    }

    public bool GetInputActionValue(InputActionReference reference, InputActionState inputActionState)
    {
        if(GetAction(reference, out InputAction inputAction))
        {
            switch(inputActionState)
            {
                case InputActionState.WasPerformedThisFrame: return inputAction.WasPerformedThisFrame();
                case InputActionState.WasPressedThisFrame: return inputAction.WasPressedThisFrame();
                case InputActionState.WasReleasedThisFrame: return inputAction.WasReleasedThisFrame();
                case InputActionState.IsPressed: return inputAction.IsPressed();
                default: return inputAction.WasPerformedThisFrame();
            }
        }
        return inputAction.WasPerformedThisFrame();
    }
    public bool GetInputActionValue(string inputActionName, InputActionState inputActionState)
    {
        return GetInputActionValue(GetInputReferenceByName(inputActionName), inputActionState);
    }

    public InputAction GetInputAction(InputActionReference reference)
    {
        GetAction(reference, out InputAction inputAction);
        return inputAction;
    }

    public InputAction GetInputAction(string inputActionName)
    {
        GetAction(GetInputReferenceByName(inputActionName), out InputAction inputAction);
        return inputAction;
    }

    public Vector2 GetMouseScreenPosition()
    {
        return InputSystem.GetDevice<Mouse>().position.ReadValue();
    }

    #endregion

    #region Local Methods

    private void InitializeAction(string actionName, out InputAction inputAction)
    {
        inputAction = inputActionAsset.FindAction(actionName, true);
        inputAction.Enable();
    }
    
    private InputActionReference GetInputReferenceByName(string inputActionName)
    {
        return inputActions.Where(x => x.Key.inputActionName == inputActionName).Select(x => x.Key).FirstOrDefault();
    }

    private InputAction GetInputActionByName(string inputActionName)
    {
        return inputActions.Where(x => x.Key.inputActionName == inputActionName).Select(x => x.Value).FirstOrDefault();
    }
    
    private bool GetAction(InputActionReference reference, out InputAction inputAction)
    {
        if(inputActions.TryGetValue(reference, out InputAction action))
        {
            inputAction = action;
            return true;
        }
        inputAction = null;
        return false;
    }


    #endregion
}
