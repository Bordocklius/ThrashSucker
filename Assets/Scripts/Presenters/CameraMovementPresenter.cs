using UnityEngine;
using UnityEngine.InputSystem;

namespace ThrashSucker.Presenters
{
    public class CameraMovementPresenter : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private Transform _playerTransform;
        [SerializeField]
        private float _rotationSpeed;
        [SerializeField]
        private Vector2 _yLimits;

        private Vector2 _cameraMovement;
        private float _pitch;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            // Apply yaw (horizontal rotation) (rotate player instead of camera)
            float yaw = _cameraMovement.x * _rotationSpeed * Time.deltaTime;
            _playerTransform.rotation *= Quaternion.AngleAxis(yaw, Vector3.up);

            // Apply pitch (Vertical rotation)
            float pitchdelta = -_cameraMovement.y * _rotationSpeed * Time.deltaTime;
            _pitch += pitchdelta;
            _pitch = Mathf.Clamp(_pitch, -90f, 90f);
            _cameraTransform.localRotation = Quaternion.AngleAxis(_pitch, Vector3.right);

            //if (_cameraTransform.rotation.eulerAngles.x < 360f + _yLimits.x && _cameraTransform.rotation.eulerAngles.x > 180f)
            //{
            //    _cameraTransform.rotation = Quaternion.AngleAxis(_yLimits.x, Vector3.right);
            //}
            //else if (_cameraTransform.rotation.eulerAngles.x > _yLimits.y && _cameraTransform.rotation.eulerAngles.x < 180f)
            //{
            //    _cameraTransform.rotation = Quaternion.AngleAxis(_yLimits.y, Vector3.right);
            //}
        }

        // OnLook called by inputsystem
        private void OnLook(InputValue inputValue)
        {
            _cameraMovement = inputValue.Get<Vector2>();
        }
    }

}
