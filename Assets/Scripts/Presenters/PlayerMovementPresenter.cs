using UnityEngine;
using UnityEngine.InputSystem;

namespace ThrashSucker.Presenters
{
    public class PlayerMovementPresenter : MonoBehaviour
    {
        [SerializeField]
        private CharacterController _characterController;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private float _movementSpeed;

        private Vector2 _movementInput;
        private Vector3 _verticalVelocity;

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
            Vector3 move = (horizontalMove + _verticalVelocity) * Time.deltaTime; // verticalVelocity is m/s in Y

            _characterController.Move(move);
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
                    Health-= enemy.Damage;
                }
            }
        }


    }

}
