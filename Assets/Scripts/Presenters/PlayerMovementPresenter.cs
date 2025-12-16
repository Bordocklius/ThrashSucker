using UnityEngine;
using UnityEngine.InputSystem;

namespace TrashSucker.Presenters
{
    [SelectionBase]
    public class PlayerMovementPresenter : MonoBehaviour
    {
        private TrashInputActions _inputActions;

        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private float _movementSpeed;
        [SerializeField]
        private float _knockbackDuration = 0.15f;
        [SerializeField]
        private float _knockbackDamping = 5f;
        [SerializeField]
        private float _sprintModifier;
        [SerializeField]
        private float _jumpVelocity;
        public float MaxSprintTimer;

        private Vector2 _movementInput;
        private Vector3 _verticalVelocity;
        private bool _isSprinting = false;

        private Vector3 _knockbackVelocity;
        private float _knockbackTimer;

        public float SprintTimer;

        public LayerMask HitableLayer;
        public float StartingHP;

        private float _health;
        public float Health
        {
            get { return _health; }
            set
            {
                if (_health == value)
                    return;

                _health = value;

                if (_health <= 0)
                    Debug.Log("ded");
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();
            Health = StartingHP;
            SprintTimer = MaxSprintTimer;
            _inputActions = new TrashInputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.PlayerInput.Move.performed += Move_Performed;
            _inputActions.PlayerInput.Move.canceled += Move_Performed;

            _inputActions.PlayerInput.Sprint.performed += Sprint_Performed;
            _inputActions.PlayerInput.Sprint.canceled += Sprint_Performed;

            _inputActions.PlayerInput.Jump.performed += Jump_Performed;
        }

        private void OnDisable()
        {
            _inputActions.Disable();
            _inputActions.PlayerInput.Move.performed -= Move_Performed;
            _inputActions.PlayerInput.Move.canceled -= Move_Performed;

            _inputActions.PlayerInput.Sprint.performed -= Sprint_Performed;
            _inputActions.PlayerInput.Sprint.canceled -= Sprint_Performed;

            _inputActions.PlayerInput.Jump.performed -= Jump_Performed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            Debug.Log(SprintTimer);
            if (!_isSprinting && SprintTimer <= MaxSprintTimer && SprintTimer != MaxSprintTimer)
            {
                SprintTimer += Time.deltaTime;
                if (SprintTimer >= MaxSprintTimer)
                    SprintTimer = MaxSprintTimer;
            }

            //Make charactermovement follow camera orientation
            Vector3 cameraForward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_mainCamera.transform.right.x, 0, _mainCamera.transform.right.z).normalized;

            Vector3 movementDirection = (cameraRight * _movementInput.x + cameraForward * _movementInput.y).normalized;
            float movementspeed = _movementSpeed;

            if (_isSprinting && SprintTimer >= 0f)
            {
                Debug.Log("Sprinting");
                movementspeed *= _sprintModifier;
                SprintTimer -= Time.deltaTime;
            }

            if (_characterController.isGrounded)
            {
                if (_verticalVelocity.y < 0f)
                    _verticalVelocity.y = -2f;
            }

            _verticalVelocity += Physics.gravity * Time.deltaTime;

            Vector3 horizontalMove = movementDirection * movementspeed; // m/s           
            Vector3 move = horizontalMove + _verticalVelocity; // verticalVelocity is m/s in Y

            if (_knockbackTimer <= _knockbackDuration)
            {
                _knockbackTimer += Time.deltaTime;

                move += _knockbackVelocity;

                _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _knockbackDamping * Time.deltaTime);
            }

            _characterController.Move(move * Time.deltaTime);
        }

        private void Move_Performed(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                _movementInput = ctx.ReadValue<Vector2>();
            else
                _movementInput = Vector2.zero;
        }

        private void Sprint_Performed(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
                _isSprinting = true;
            else if (ctx.canceled)
                _isSprinting = false;
        }

        private void Jump_Performed(InputAction.CallbackContext ctx)
        {
            if (_characterController.isGrounded)
                _verticalVelocity.y = _jumpVelocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Colide with hittable object
            if (collision != null && ((1 << collision.gameObject.layer) & HitableLayer) != 0)
            {
                if (collision.gameObject.TryGetComponent<EnemyBasePresenter>(out EnemyBasePresenter enemy))
                {
                    Health -= enemy.Damage;
                    ApplyEnemyKnockback(collision.transform.position, enemy.KnockbackStrength);
                }
            }
        }

        private void ApplyEnemyKnockback(Vector3 position, float strength)
        {
            if (strength <= 0f)
                strength = 1f;

            _knockbackVelocity = (_characterController.transform.position - position).normalized * strength;
            _knockbackTimer = 0f;
            Debug.DrawRay(position, _knockbackVelocity, Color.black, 3f);
        }
    }

}
