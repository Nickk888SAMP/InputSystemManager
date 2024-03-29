using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerCameraControllerExample : MonoBehaviour 
{
    public InputActionReference lookActionReference;
    public InputActionReference moveActionReference;
    public InputActionReference sprintActionReference;
    public InputActionReference aimActionReference;
    public InputActionReference shootActionReference;

    public Transform bulletPrefab;
    public Transform cameraTransform;

    private float yRot, xRot;
    private float sprintMultiplier = 2.0f;

    private void Update() 
    {
        InputAction movementAction = InputManager.Instance.GetInputAction("Movement");
        InputAction fireAction = InputManager.Instance.GetInputAction("Fire");

        bool isSprinting = InputManager.Instance.GetInputActionValue(sprintActionReference, InputActionState.IsPressed);
        bool isAiming = InputManager.Instance.GetInputActionValue(aimActionReference, InputActionState.IsPressed);
        bool isShooting = InputManager.Instance.GetInputActionValue(shootActionReference, InputActionState.WasPressedThisFrame);
        
        if(InputManager.Instance.GetInputActionValue(lookActionReference, out Vector2 lookInputValue) && isAiming)
        {
            yRot = Mathf.Clamp(yRot += lookInputValue.y, -90, 90);
            xRot += lookInputValue.x;
            cameraTransform.rotation = Quaternion.Euler(-yRot, xRot, 0);
        }

        if(InputManager.Instance.GetInputActionValue(moveActionReference, out Vector2 moveInputValue))
        {
            Vector3 translation = new Vector3(moveInputValue.x, 0, moveInputValue.y) * 100 * (isSprinting ? 5 : 1);
            cameraTransform.Translate(translation * Time.deltaTime, Space.Self);
        }

        if(isShooting)
        {
            Transform bullet = Instantiate(bulletPrefab, cameraTransform.position, Quaternion.identity);
            if(bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(cameraTransform.forward * 2000f);
            }
        }
    }
}
