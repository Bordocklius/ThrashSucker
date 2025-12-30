using UnityEngine;
using UnityEngine.InputSystem;

namespace TrashSucker.Presenters
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

        // Head bob settings
        [Header("Head Bob")]
        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private float _bobAmplitude = 0.05f;
        [SerializeField]
        private float _bobFrequency = 8f;
        [SerializeField]
        private float _swayAmount = 0.02f;
        [SerializeField]
        private float _bobSmoothness = 8f;
        [SerializeField]
        private float _maxPlayerSpeed = 5f;
        [SerializeField]
        private float _movementThreshold = 0.1f;

        private Vector2 _cameraMovement;
        private float _pitch;

        // head bob runtime state
        private float _bobTimer;
        private Vector3 _originalLocalPosition;
        private Vector3 _currentBobOffset;

        private void Awake()
        {
            if (_cameraTransform == null && Camera.main != null)
                _cameraTransform = Camera.main.transform;

            if (_playerTransform != null && _characterController == null)
                _characterController = _playerTransform.GetComponent<CharacterController>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (_cameraTransform != null)
                _originalLocalPosition = _cameraTransform.localPosition;
            else
                _originalLocalPosition = Vector3.zero;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            // Apply yaw (horizontal rotation) (rotate player instead of camera)
            float yaw = _cameraMovement.x * _rotationSpeed * Time.deltaTime;
            _playerTransform.rotation *= Quaternion.AngleAxis(yaw, Vector3.up);

            // Apply pitch (Vertical rotation)
            float pitchdelta = -_cameraMovement.y * _rotationSpeed * Time.deltaTime;
            _pitch += pitchdelta;
            _pitch = Mathf.Clamp(_pitch, -90f, 90f);
            _cameraTransform.localRotation = Quaternion.AngleAxis(_pitch, Vector3.right);

            // Head bob update
            UpdateHeadBob();
        }

        private void UpdateHeadBob()
        {
            if (_cameraTransform == null)
                return;

            // Measure horizontal movement speed (CharacterController.velocity is in m/s)
            float speed = 0f;
            if (_characterController != null)
            {
                Vector3 horizontalVel = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
                speed = horizontalVel.magnitude;
            }

            // Normalized movement amount [0,1]
            float normalized = (_maxPlayerSpeed > 0f) ? Mathf.Clamp01(speed / _maxPlayerSpeed) : Mathf.Clamp01(speed);

            // If moving above threshold, advance bob timer; otherwise decay timer slightly
            if (normalized > _movementThreshold)
            {
                _bobTimer += Time.deltaTime * _bobFrequency * Mathf.Lerp(0.5f, 1.5f, normalized);
            }
            else
            {
                // gentle slow down of the bob phase so it doesn't jump when starting again
                _bobTimer = Mathf.Lerp(_bobTimer, 0f, Time.deltaTime * 2f);
            }

            // target offsets
            float bobY = Mathf.Sin(_bobTimer) * _bobAmplitude * normalized;
            float bobX = Mathf.Sin(_bobTimer * 0.5f) * _swayAmount * normalized;

            Vector3 targetOffset = new Vector3(bobX, bobY, 0f);

            // smooth the bob offset
            _currentBobOffset = Vector3.Lerp(_currentBobOffset, targetOffset, Time.deltaTime * _bobSmoothness);

            // apply to camera local position
            _cameraTransform.localPosition = _originalLocalPosition + _currentBobOffset;
        }

        // OnLook called by inputsystem
        private void OnLook(InputValue inputValue)
        {
            _cameraMovement = inputValue.Get<Vector2>();
        }
    }

}
