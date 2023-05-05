using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Input Manager/InputActionReference")]
public class InputActionReference : ScriptableObject 
{
    public string inputActionName;
    public InputActionType inputActionType;

    public enum InputActionType
    {
        Button,
        Vector2
    }
}
