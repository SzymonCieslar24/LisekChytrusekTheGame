using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float distance = 5f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float maxVerticalAngle = 45f;
    [SerializeField] float minVerticalAngle = 5f;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    [SerializeField] Vector2 framingOffset;

    float rotationX;
    float rotationY;

    float invertXVal;
    float invertYVal;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * rotationSpeed * invertXVal;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * rotationSpeed * invertYVal;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPosition = followTarget.position + new Vector3(framingOffset.x,framingOffset.y);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    //public Quaternion PlannarRotation => Quaternion.Euler(0, rotationY, 0);
}
