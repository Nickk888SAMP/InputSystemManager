using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerTest : MonoBehaviour 
{
    [SerializeField] private Transform actionDataGridTransform;
    [SerializeField] private GameObject actionDataTemplateUIPrefab;
    private List<ActionDataTemplateUI> actionDataTemplates = new List<ActionDataTemplateUI>();

    private void Start() 
    {
        if(InputManager.Instance != null)
        {
            foreach(InputActionReferenceSO actionReferenceSO in InputManager.Instance.InputReferences)
            {
                var go = Instantiate(actionDataTemplateUIPrefab, actionDataGridTransform);
                if(go.TryGetComponent<ActionDataTemplateUI>(out ActionDataTemplateUI actionDataTemplateUI))
                {
                    actionDataTemplates.Add(actionDataTemplateUI);
                }
            }
        }
    }

    private void Update() 
    {
        for (int i = 0; i < InputManager.Instance.InputReferences.Length; i++)
        {
            var inputRef = InputManager.Instance.InputReferences[i];
            var action = InputManager.Instance.GetInputAction(inputRef);
            var template = actionDataTemplates[i];
            switch(inputRef.inputActionType)
            {
                case InputActionReferenceSO.InputActionType.Button:
                {
                    string buttonString = $"<color=red>Is Pressed: <color=green>{action.IsPressed()}</color>";
                    template.SetText(inputRef, buttonString);
                    break;
                }
                case InputActionReferenceSO.InputActionType.Vector2:
                {
                    string vector2String = $"<color=green>{action.ReadValue<Vector2>().ToString()}</color>";
                    template.SetText(inputRef, vector2String);
                    break;
                }
            }
        }

        
    }


}
