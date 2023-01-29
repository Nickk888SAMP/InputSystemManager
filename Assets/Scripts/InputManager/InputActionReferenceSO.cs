using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Input Manager/InputActionReferenceSO")]
public class InputActionReferenceSO : ScriptableObject 
{
    public string inputActionName;
    public InputActionType inputActionType;

    public enum InputActionType
    {
        Button,
        Vector2
    }
}
