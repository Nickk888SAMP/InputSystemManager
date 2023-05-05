using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerReferencesCameraControllerTest : MonoBehaviour 
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
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        bool isSprinting = InputManager.Instance.GetInputActionValue("Sprint", InputActionState.IsPressed);
        bool isAiming = InputManager.Instance.GetInputActionValue("Aim", InputActionState.IsPressed);
        bool isShooting = InputManager.Instance.GetInputActionValue("Fire", InputActionState.WasPressedThisFrame);
        if(InputManager.Instance.GetInputActionValue("Look", out Vector2 lookInputValue) && isAiming)
        {
            yRot = Mathf.Clamp(yRot += lookInputValue.y, -90, 90);
            xRot += lookInputValue.x;
            cameraTransform.rotation = Quaternion.Euler(-yRot, xRot, 0);
        }
        if(InputManager.Instance.GetInputActionValue("Move", out Vector2 moveInputValue))
        {
            Vector3 translation = new Vector3(moveInputValue.x, 0, moveInputValue.y) * 100 * (isSprinting ? 5 : 1);
            cameraTransform.Translate(translation * Time.deltaTime, Space.Self);
        }

        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);

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
