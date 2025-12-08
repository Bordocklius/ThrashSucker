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

        private Vector3 _movementVelocity;
        private Vector2 _movementInput;

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
            if(_movementInput == null)
                return;

            //Make charactermovement follow camera orientation
            Vector3 cameraForward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_mainCamera.transform.right.x, 0, _mainCamera.transform.right.z).normalized;

            _movementVelocity = (cameraRight * _movementInput.x + cameraForward * _movementInput.y).normalized;
            float movementspeed = _movementSpeed;

            if(!_characterController.isGrounded)
            {
                _movementVelocity += Physics.gravity;
            }

            _movementVelocity *= _movementSpeed * Time.deltaTime;
            _characterController.Move(_movementVelocity);
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
