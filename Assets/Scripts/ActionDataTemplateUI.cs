using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionDataTemplateUI : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(InputActionReferenceSO actionReferenceSO, string value)
    {
        this.text.text = actionReferenceSO.inputActionName + " action value: " + value;
    }
}
