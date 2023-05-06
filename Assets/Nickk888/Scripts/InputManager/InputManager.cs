using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour 
{
    
    private static InputManager instance = null;
    public static InputManager Instance { get { return instance; } }
    public InputActionReference[] InputReferences { get { return inputReferences; } }

    public event EventHandler<OnBindingRebindEventArgs> OnBindingRebind;
    public event EventHandler<OnBindingRebindStartEventArgs> OnBindingRebindStart;
    public event EventHandler<OnBindingRebindCancelledEventArgs> OnBindingRebindCancelled;

    public class OnBindingRebindEventArgs : EventArgs 
    {
        public InputAction inputAction;
        public InputActionReference inputActionReference;
        public int bindingIndex;
    }

    public class OnBindingRebindStartEventArgs : EventArgs 
    {
        public InputAction inputAction;
        public InputActionReference inputActionReference;
        public int bindingIndex;
    }

    public class OnBindingRebindCancelledEventArgs : EventArgs 
    {
        public InputAction inputAction;
        public InputActionReference inputActionReference;
        public int bindingIndex;
    }

    [Header("Input Action Asset"), Tooltip("The Input Action Asset with the actions that the references will be referenced.")]
    [SerializeField] private InputActionAsset inputActionAsset;

    [Header("Bindings Save/Load"), Tooltip("The name of the Key in the PlayerPrefs for the Input Bindings Save and Load")]
    [SerializeField] private string bindingsPlayerPrefsKey = "InputBindings";

    [Header("Input Action References"), Tooltip("The Input Action References that will be initialized on load to be accessable during gameplay.")]
    [SerializeField] private InputActionReference[] inputReferences;
    
    private Dictionary<InputActionReference, InputAction> inputActions = new Dictionary<InputActionReference, InputAction>();
    private Dictionary<InputAction, InputActionReference> inputActionReferences = new Dictionary<InputAction, InputActionReference>();
    private Dictionary<string, InputAction> inputActionStrings = new Dictionary<string, InputAction>();
    private Dictionary<string, InputActionReference> inputActionReferenceStrings = new Dictionary<string, InputActionReference>();

    #region Callbacks

    private void Awake() 
    {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        // Load bindings from PlayerPrefs
        if(PlayerPrefs.HasKey(bindingsPlayerPrefsKey))
        {
            inputActionAsset.LoadBindingOverridesFromJson(PlayerPrefs.GetString(bindingsPlayerPrefsKey));
        }

        // Load all references
        foreach(InputActionReference reference in inputReferences)
        {
            AddInputActionReference(reference);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Tries to add a Input Action Reference to the InputManager.
    /// Returns "false" if the Reference has been already added or Input Action has not been found in the Input Action Asset.
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <returns></returns>
    public bool AddInputActionReference(InputActionReference inputActionReference)
    {
        if(inputActions.ContainsKey(inputActionReference))
            return false;

        InitializeAction(inputActionReference.inputActionName, out InputAction inputAction);
        if(inputAction is null)
            return false;

        inputActions.Add(inputActionReference, inputAction);
        inputActionReferences.Add(inputAction, inputActionReference);
        inputActionReferenceStrings.Add(inputActionReference.inputActionName.ToLower(), inputActionReference);
        inputActionStrings.Add(inputActionReference.inputActionName.ToLower(), inputAction);
        return true;
    }

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
    /// Get InputActionReference by providing the InputAction
    /// Will return "null" if the InputActionReference wasn't found.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public InputActionReference GetInputReference(InputAction action)
    {
        GetActionReference(action, out InputActionReference inputActionReference);
        return inputActionReference;
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

    /// <summary>
    /// Performs a rebind process for the given Input Action and the Index if the binding.
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <param name="bindingIndex"></param>
    public void PerformInputActionRebind(InputActionReference inputActionReference, int bindingIndex)
    {
        PerformInputActionRebind(GetInputAction(inputActionReference), bindingIndex);
    }

    /// <summary>
    /// Performs a rebind process for the given Input Action and the Index if the binding.
    /// </summary>
    /// <param name="inputAction"></param>
    /// <param name="bindingIndex">The Binding Index is the index of the Actions Array in the Input Action Asset</param>
    public void PerformInputActionRebind(InputAction inputAction, int bindingIndex)
    {
        var inputActionReference = GetInputReference(inputAction);

        foreach(var map in inputActionAsset.actionMaps) {
            map.Disable();
        }
        inputAction.PerformInteractiveRebinding(bindingIndex)

        // On Complete
        .OnComplete(callback => 
        {
            callback.Dispose();
            foreach(var map in inputActionAsset.actionMaps) {
                map.Enable();
            }
            OnBindingRebind?.Invoke(this, new OnBindingRebindEventArgs { inputAction = inputAction, inputActionReference =inputActionReference, bindingIndex = bindingIndex });

            PlayerPrefs.SetString(bindingsPlayerPrefsKey, inputActionAsset.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        })

        // On Cancel
        .OnCancel(callback => 
        {
            callback.Dispose();
            OnBindingRebindCancelled?.Invoke(this, new OnBindingRebindCancelledEventArgs { inputAction = inputAction, inputActionReference = inputActionReference, bindingIndex = bindingIndex });
        })

        // Start
        .Start();
        OnBindingRebindStart?.Invoke(this, new OnBindingRebindStartEventArgs { inputAction = inputAction, inputActionReference = inputActionReference, bindingIndex = bindingIndex });
    }

    /// <summary>
    /// Returns the Display String of the Input Action's Binding. For short: It's the name of the Button like "E" or "Shift".
    /// </summary>
    /// <param name="inputAction"></param>
    /// <param name="bindingIndex"></param>
    /// <returns></returns>
    public string GetBindingDisplayString(InputAction inputAction, int bindingIndex)
    {
        return inputAction.bindings[bindingIndex].ToDisplayString();
    }

    /// <summary>
    /// Returns the Display String of the Input Action's Binding. For short: It's the name of the Button like "E" or "Shift".
    /// </summary>
    /// <param name="inputActionReference"></param>
    /// <param name="bindingIndex"></param>
    /// <returns></returns>
    public string GetBindingDisplayString(InputActionReference inputActionReference, int bindingIndex)
    {
        return GetBindingDisplayString(GetInputAction(inputActionReference), bindingIndex);
    }

    /// <summary>
    /// Returns the Display String of the Input Action's Binding. For short: It's the name of the Button like "E" or "Shift".
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <param name="bindingIndex"></param>
    /// <returns></returns>
    public string GetBindingDisplayString(string inputActionName, int bindingIndex)
    {
        return GetBindingDisplayString(GetInputAction(inputActionName), bindingIndex);
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
        inputActionReferenceStrings.TryGetValue(inputActionName.ToLower(), out InputActionReference actionReference);
        return actionReference;
    }

    /// <summary>
    /// Returns the InputAction providing the name of the InputAction
    /// Will return "null" if the reference wasn't found.
    /// </summary>
    /// <param name="inputActionName"></param>
    /// <returns></returns>
    private InputAction GetInputActionByName(string inputActionName)
    {
        inputActionStrings.TryGetValue(inputActionName.ToLower(), out InputAction action);
        return action;
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
        return inputActions.TryGetValue(reference, out inputAction);
    }

    /// <summary>
    /// Returns the InputActionReference in the "out" parameter by providing the action.
    /// Will return "null" if the action wasn't found.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="inputActionReference"></param>
    /// <returns></returns>
    private bool GetActionReference(InputAction action, out InputActionReference inputActionReference)
    {
        return inputActionReferences.TryGetValue(action, out inputActionReference);
    }


    #endregion
}

public enum InputActionState
{
    WasPerformedThisFrame,
    WasPressedThisFrame,
    WasReleasedThisFrame,
    IsPressed
}
