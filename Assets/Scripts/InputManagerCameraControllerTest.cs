using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerCameraControllerTest : MonoBehaviour 
{
    public InputActionReferenceSO lookActionReference;
    public InputActionReferenceSO moveActionReference;
    public Transform cameraTransform;
    private float yRot, xRot;

    private void Update() 
    {
        if(InputManager.Instance.GetInputActionValue(lookActionReference, out Vector2 lookInputValue))
        {
            yRot = Mathf.Clamp(yRot += lookInputValue.y, -90, 90);
            xRot += lookInputValue.x;
            cameraTransform.rotation = Quaternion.Euler(-yRot, xRot, 0);
        }
        if(InputManager.Instance.GetInputActionValue(moveActionReference, out Vector2 moveInputValue))
        {
            Vector3 translation = new Vector3(moveInputValue.x, 0, moveInputValue.y) * 100;
            cameraTransform.Translate(translation * Time.deltaTime, Space.Self);
        }
    }
}
