using UnityEngine;

namespace Controller
{
    public class FreeCameraController: MonoBehaviour
    {
        [Header("Camera Settings")]
        [Range(10.0f, 100.0f)]
        public float movementSpeed = 10f;
        [Range(10.0f, 100.0f)]
        public float fastMovementSpeed = 100f;
        [Range(1.0f, 10.0f)]
        public float freeLookSensitivity = 3f;
        [Range(1.0f, 20.0f)]
        public float zoomSensitivity = 10f;
        [Range(10.0f, 100.0f)]
        public float fastZoomSensitivity = 50f;
        
        private bool _looking;

        private void Update()
        {
            var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");

            var moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            var currentMovementSpeed = fastMode ? fastMovementSpeed : movementSpeed;
            transform.Translate(moveDirection * currentMovementSpeed * Time.deltaTime);

            if (_looking)
            {
                var localEulerAngles = transform.localEulerAngles;
                var newRotationX = localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
                var newRotationY = localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
                localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
                transform.localEulerAngles = localEulerAngles;
            }

            var scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                var currentZoomSensitivity = fastMode ? fastZoomSensitivity : zoomSensitivity;
                transform.Translate(Vector3.forward * scrollInput * currentZoomSensitivity * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartLooking();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                StopLooking();
            }
        }

        private void OnDisable()
        {
            StopLooking();
        }

        private void StartLooking()
        {
            _looking = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void StopLooking()
        {
            _looking = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}