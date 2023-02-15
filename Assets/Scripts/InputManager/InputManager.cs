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

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a Vector2 value in the Out parameter providing the reference of the InputAction.
    /// Returns a boolean if the Action was found or not. 
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Returns a Vector2 value in the Out parameter providing the name of the InputAction.
    /// Returns a boolean if the Action was found or not.
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool GetInputActionValue(string inputActionName, out Vector2 value)
    {
        return GetInputActionValue(GetInputReferenceByName(inputActionName), out value);
    }

    /// <summary>
    /// Returns a boolean value of the InputAction state providing the reference of the InputAction.
    /// Will return false if the InputAction wasn't found.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="inputActionState"></param>
    /// <returns></returns>
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
        return false;
    }

    /// <summary>
    /// Returns a boolean value of the InputAction state providing the name of the InputAction.
    /// Will return false if the InputAction wasn't found.
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <param name="inputActionState"></param>
    /// <returns></returns>
    public bool GetInputActionValue(string inputActionName, InputActionState inputActionState)
    {
        return GetInputActionValue(GetInputReferenceByName(inputActionName), inputActionState);
    }

    /// <summary>
    /// Returns the InputAction by providing the reference of the InputAction.
    /// Will return "null" if the InputAction wasn't found.
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public InputAction GetInputAction(InputActionReference reference)
    {
        GetAction(reference, out InputAction inputAction);
        return inputAction;
    }

    /// <summary>
    /// Return the InputAction by providing the name of the InputAction.
    /// Will return "null" if the InputAction wasn't found.
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <returns></returns>
    public InputAction GetInputAction(string inputActionName)
    {
        GetAction(GetInputReferenceByName(inputActionName), out InputAction inputAction);
        return inputAction;
    }

    /// <summary>
    /// Returns a Vector2 with the X and Y position of the Mouse on the Screen.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMouseScreenPosition()
    {
        return InputSystem.GetDevice<Mouse>().position.ReadValue();
    }

    #endregion

    #region Local Methods

    /// <summary>
    /// Initializes the action by finding the InputAction by name in the Input Action Asset.
    /// </summary>
    /// <param name="actionName"></param>
    /// <param name="inputAction"></param>
    private void InitializeAction(string actionName, out InputAction inputAction)
    {
        inputAction = inputActionAsset.FindAction(actionName, true);
        inputAction.Enable();
    }
    
    /// <summary>
    /// Returns the InputActionReference providing the name of the InputAction.
    /// Will return "null" if the reference wasn't found.
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <returns></returns>
    private InputActionReference GetInputReferenceByName(string inputActionName)
    {
        return inputActions.Where(x => x.Key.inputActionName == inputActionName).Select(x => x.Key).FirstOrDefault();
    }

    /// <summary>
    /// Returns the InputAction providing the name of the InputAction
    /// Will return "null" if the reference wasn't found.
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <returns></returns>
    private InputAction GetInputActionByName(string inputActionName)
    {
        return inputActions.Where(x => x.Key.inputActionName == inputActionName).Select(x => x.Value).FirstOrDefault();
    }
    
    /// <summary>
    /// Returns the InputAction in the "out" parameter by providing the reference.
    /// Will return "null" if the reference wasn't found.
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="inputAction"></param>
    /// <returns></returns>
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
