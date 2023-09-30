using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float minYPosition = 3f;
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minRotationY = -60f;
    public float maxRotationY = 60f;
    public float panningSpeed = 10f;
    public float zoomSpeed = 140f;
    float rotationY;

    void Start()
    {
        rotationY = -transform.localEulerAngles.x;
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) mouseLeftButtonClicked();
        else if (Input.GetMouseButton(1)) mouseRightButtonClicked();
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) showAndUnlockCursor();
        handleMouseWheel();
    }

    void showAndUnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void hideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void mouseLeftButtonClicked()
    {
        Vector3 horizontalDirection = Quaternion.Euler(0, 90, 0) *
            new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        transform.position += horizontalDirection * Input.GetAxis("Mouse X") * panningSpeed;
        transform.position += Input.GetAxis("Mouse Y") * panningSpeed * Vector3.down;
        if (transform.position.y < minYPosition)
            transform.position = new Vector3(transform.position.x, minYPosition, transform.position.z);
    }

    void mouseRightButtonClicked()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minRotationY, maxRotationY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    void handleMouseWheel()
    {
        transform.position = transform.position + transform.forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (transform.position.y < minYPosition)
            transform.position = new Vector3(transform.position.x, minYPosition, transform.position.z);
    }
}