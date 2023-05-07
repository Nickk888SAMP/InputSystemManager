# Input System Manager

This asset for Unity is a powerful Input System Manager that allows you to easily retrieve data from the Input System using Scriptable Objects as references to Input Actions. With this Singleton-based solution, you'll be able to speed up the process of game development and create a more intuitive user experience for your players. 

## Features
* Singleton/Instance based script for easy global access.
* Plug'n'Play, just add your Input Action Asset and references to the Manager and start reading inputs!
* Key rebinding support.
* 3 methods to read inputs: Scriptable Objects, Input Actions and Strings!
* Simple one-line code to read any inputs.
* Perfect for Single-Player and Networked Multiplayer.

## Limitations
* Local Multiplayer/Split-Screen is not supported.
* Only one Action Map is supported.

## Installation
1. Download the asset and import to your Project.
2. Create a new `Empty Game Object` and add the `InputManager.cs` component to it.
3. Create and configure a new `Input Actions` Asset. (Don't forget to install the `Input System package` from the `Package Manager`).
4. Drag and Drop or Select the `Input Actions` Asset in the `InputManager` component.
5. Create reference/s for the asset clicking `Create > Scriptable Objects > Input Manager > InputActionReference`, providing the `Name` and the `Type` of the Action.
6. Add the reference/s to the `Input Manager` component's array.
7. That's all. Now you can retrieve data from the actions by calling code from the InputManager's Instance.

## How to Use
You can retrieve data from the Input System Manager by using one of the 2 types as a reference.
### By Input Action References
```csharp
public InputActionReference moveActionReference; // Vector2
public InputActionReference shootActionReference; // Button

private void Update() {
    if(InputManager.Instance.GetInputActionValue(moveActionReference, out Vector2 moveInputValue)) {
        // Movement Code
    }

    if(InputManager.Instance.GetInputActionValue(shootActionReference, InputActionState.WasPressedThisFrame)) {
        // Shooting Code
    }
}
```
### By Strings
```csharp
private void Update() {
    if(InputManager.Instance.GetInputActionValue("Movement", out Vector2 moveInputValue)) {
        // Movement Code
    }

    if(InputManager.Instance.GetInputActionValue("Fire", InputActionState.WasPressedThisFrame)) {
        // Shooting Code
    }
}
```

## All Methods
```csharp
public bool AddInputActionReference(InputActionReference inputActionReference)
public bool GetInputActionValue(InputActionReference reference, out Vector2 value)
public bool GetInputActionValue(string inputActionName, out Vector2 value)
public bool GetInputActionValue(InputActionReference reference, InputActionState inputActionState)
public bool GetInputActionValue(string inputActionName, InputActionState inputActionState)
public InputActionReference GetInputReference(InputAction action)
public InputAction GetInputAction(InputActionReference reference)
public InputAction GetInputAction(string inputActionName)
public Vector2 GetMouseScreenPosition()
public void PerformInputActionRebind(InputActionReference inputActionReference, int bindingIndex)
public void PerformInputActionRebind(InputAction inputAction, int bindingIndex)
public string GetBindingDisplayString(InputAction inputAction, int bindingIndex)
public string GetBindingDisplayString(InputActionReference inputActionReference, int bindingIndex)
public string GetBindingDisplayString(string inputActionName, int bindingIndex)
```
