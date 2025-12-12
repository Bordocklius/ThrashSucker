using UnityEngine;
using UnityEngine.InputSystem;

namespace TrashSucker.Presenters
{
    [SelectionBase]
    public class PlayerMovementPresenter : MonoBehaviour
    {
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

        private Vector2 _movementInput;
        private Vector3 _verticalVelocity;

        private Vector3 _knockbackVelocity;
        private float _knockbackTimer;

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
            if(_mainCamera == null)
                _mainCamera = Camera.main;
            if( _characterController == null)
                _characterController = GetComponent<CharacterController>();
            Health = StartingHP;
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
            //Make charactermovement follow camera orientation
            Vector3 cameraForward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_mainCamera.transform.right.x, 0, _mainCamera.transform.right.z).normalized;

            Vector3 movementDirection = (cameraRight * _movementInput.x + cameraForward * _movementInput.y).normalized;
            float movementspeed = _movementSpeed;

            if(_characterController.isGrounded)
            {
                if (_verticalVelocity.y < 0f)
                    _verticalVelocity.y = -2f;
            }          

            _verticalVelocity += Physics.gravity * Time.deltaTime;

            Vector3 horizontalMove = movementDirection * _movementSpeed; // m/s           
            Vector3 move = horizontalMove + _verticalVelocity; // verticalVelocity is m/s in Y

            if (_knockbackTimer <= _knockbackDuration)
            {
                _knockbackTimer += Time.deltaTime;

                move += _knockbackVelocity;

                _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _knockbackDamping * Time.deltaTime);
            }

            _characterController.Move(move * Time.deltaTime);
        }

        private void OnMove(InputValue value)
        {
            _movementInput = value.Get<Vector2>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Colide with hittable object
            if (collision != null && ((1 << collision.gameObject.layer) & HitableLayer) != 0)
            {
                if(collision.gameObject.TryGetComponent<EnemyBasePresenter>(out EnemyBasePresenter enemy))
                {
                    Health -= enemy.Damage;
                    ApplyEnemyKnockback(collision.transform.position, enemy.KnockbackStrength);
                }
            }
        }

        private void ApplyEnemyKnockback(Vector3 position, float strength)
        {
            if(strength <= 0f) 
                strength = 1f;

            _knockbackVelocity = (_characterController.transform.position - position).normalized * strength;
            _knockbackTimer = 0f;
            Debug.DrawRay(position, _knockbackVelocity, Color.black, 3f);
        }
    }

}
