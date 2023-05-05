using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindingButtonUI : MonoBehaviour 
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private InputActionReference reference;
    [SerializeField] private int bindingIndex;

    private void Awake() {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void Start() {
        InputManager.Instance.OnBindingRebind += InputManager_OnBindingRebind;
        InputManager.Instance.OnBindingRebindCancelled += InputManager_OnBindingRebindCancelled;
        InputManager.Instance.OnBindingRebindStart += InputManager_OnBindingRebindStart;
        UpdateBindingText();
    }

    private void InputManager_OnBindingRebindStart(object sender, InputManager.OnBindingRebindStartEventArgs e)
    {
        button.interactable = false;
        if(e.inputActionReference != reference || e.bindingIndex != bindingIndex)
            return;

        text.color = Color.red;
    }

    private void InputManager_OnBindingRebindCancelled(object sender, InputManager.OnBindingRebindCancelledEventArgs e)
    {
        button.interactable = true;
        text.color = Color.black;
        if(e.inputActionReference != reference || e.bindingIndex != bindingIndex)
            return;
    }

    private void InputManager_OnBindingRebind(object sender, InputManager.OnBindingRebindEventArgs e)
    {
        button.interactable = true;
        text.color = Color.black;
        if(e.inputActionReference != reference || e.bindingIndex != bindingIndex)
            return;

        UpdateBindingText();
    }

    private void OnButtonClicked() {
        InputManager.Instance.PerformInputActionRebind(reference, bindingIndex);
    }

    private void UpdateBindingText() {
        text.text = InputManager.Instance.GetBindingDisplayString(reference, bindingIndex);
    }
}
