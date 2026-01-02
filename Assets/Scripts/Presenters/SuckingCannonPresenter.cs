using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TrashSucker.Presenters
{
    public class SuckingCannonPresenter : MonoBehaviour
    {
        public event EventHandler CannonShot;
        public event EventHandler<AmmoChangedEventArgs> AmmoChanged;

        public List<GameObject> AmmoList = new List<GameObject>();
        public int MaxAmmoCount;
        private GameObject _previousAmmoObject;

        // Reservations to avoid race between multiple concurrent sucking coroutines
        private readonly HashSet<GameObject> _reservedAmmo = new HashSet<GameObject>();

        [Space(10)]
        [Header("Suction parameters")]
        [SerializeField]
        private Transform _barrelPoint;
        [SerializeField]
        private float _suctionRange;
        
        [SerializeField]
        private float _maxSuctionForce;
        [SerializeField]
        private float _minSuctionForce;
        [SerializeField]
        private float _suctionIncreaseMultiplier = 1f;
        [SerializeField]
        private Transform _suctionAreaMax;
        [SerializeField]
        private Transform _suctionAreaCurrent;
        [SerializeField]
        private float _suctionAreaIncreaseMultiplier = 1f;
        [SerializeField]
        private ParticleSystem _suckingParticles;
        [SerializeField]
        private float _shakeIntensity;

        private float _suctionForce;

        [Space(10)]
        [Header("Shooting parameters")]
        [SerializeField]
        private float _maxShootForce;
        [SerializeField]
        private float _shootForceIncreaseMultiplier;

        private float _shootForce;
        private float ShootForce
        {
            get { return _shootForce; }
            set
            {
                if (_shootForce == value)
                    return;

                _shootForce = value;
                UpdateShootForceText();
            }
        }

        private float _shootForceProgress => ShootForce / _maxShootForce;  

        [SerializeField]
        private float _range;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Transform _crosshairTransform;
        [SerializeField]
        private ParticleSystem _shootingParticles;
        [SerializeField]
        private LayerMask _layerMask;
        [SerializeField]
        private GameObject _collectionPoint;

        [SerializeField]
        public bool IsCannonSucking;        
        [SerializeField]
        private Transform _gunTransform;
        private bool _isCannonShooting;
        private bool _isShotBuildingUp;
        private Quaternion _originalGunRotation;

        [Space(10)]
        [Header("UI elements")]
        public TextMeshProUGUI AmmoCapacityText;
        public TextMeshProUGUI ShootForceText;
        public Image ShootForceBar;
        private Color _originalShootForceBarColor;
        public Color ShootForceLerpColor;

        public List<Image> AmmoCapacityImages = new List<Image>();
        [SerializeField]
        private Color _filledColor;
        [SerializeField]
        private Color _emptyColor;
        [SerializeField]
        private Color _reservedColor;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UpdateShootForceText();
            UpdateCapacityText();
            _originalGunRotation = _gunTransform.localRotation;
            _originalShootForceBarColor = ShootForceBar.color;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsCannonSucking)
            {
                ActivateCannonSuction();
            }
            if(!IsCannonSucking && _gunTransform.localRotation != _originalGunRotation)
            {
                _gunTransform.localRotation = _originalGunRotation;
            }

            if (AmmoList.Count > 0)
            {
                if (_isShotBuildingUp)
                {
                    BuildUpShootForce();
                }
                if (!_isShotBuildingUp && _gunTransform.localRotation != _originalGunRotation)
                {
                    _gunTransform.localRotation = _originalGunRotation;
                }
                if (_isCannonShooting)
                {
                    ShootCannon();
                }
                
            }

            if(_suctionForce > _minSuctionForce || ShootForce >= 0.1f)
            {
                ApplyGunShake();
            }

        }        

        private void ActivateCannonSuction()
        {
            // Progressively increase suction power
            if (_suctionForce < _maxSuctionForce)
                _suctionForce += Time.deltaTime * _suctionIncreaseMultiplier;
            else if(_suctionForce > _maxSuctionForce)
                _suctionForce = _maxSuctionForce;

            // Lerp suction area towards max position
            if(_suctionAreaCurrent.position != _suctionAreaMax.position)
            {
                _suctionAreaCurrent.position = Vector3.Lerp(_suctionAreaCurrent.position, _suctionAreaMax.position, Time.deltaTime * _suctionAreaIncreaseMultiplier);
                if (Vector3.Distance(_suctionAreaCurrent.position, _suctionAreaMax.position) <= 0.01f)
                {
                    _suctionAreaCurrent.position = _suctionAreaMax.position;
                }
            }           


            //List<Collider> colliders = Physics.OverlapSphere(_barrelPoint.position, _suctionRange, _layerMask).ToList();

            List<Collider> colliders = Physics.OverlapCapsule(_barrelPoint.position, _suctionAreaMax.position, _suctionRange, _layerMask).ToList();
            //Debug.Log(_suctionForce);


            Vector3 shakeOffset = UnityEngine.Random.insideUnitSphere * _shakeIntensity;

            ApplyGunShake();

            // Add force towards barrelpoint for all colliders in the overlap area
            foreach (Collider collider in colliders)
            {
                collider.attachedRigidbody.WakeUp();
                GameObject obj = collider.gameObject;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (_barrelPoint.position - rb.position).normalized;
                    rb.AddForce(direction * _suctionForce, ForceMode.Force);
                    Transform modeltransform = obj.transform.GetChild(0);

                    if (modeltransform != null)
                    {
                        modeltransform.localRotation = Quaternion.Euler(
                            UnityEngine.Random.Range(0, 1f),
                            UnityEngine.Random.Range(0, 1f),
                            UnityEngine.Random.Range(0, 1f)
                        );
                    }

                    // Add fallof of force based on distance
                    //float distance = Vector3.Distance(rb.position, _barrelPoint.position);
                    //float falloff = Mathf.Clamp01(1f - (distance / _suctionRange));
                    //float appliedForce = _suctionForce * falloff;
                    //rb.AddForce(direction * appliedForce, ForceMode.Force);

                }
            }
        }

        private void OnActivateSuck(InputValue inputValue)
        {
            IsCannonSucking = !IsCannonSucking;
            _suckingParticles.Play();
            _suctionForce = _minSuctionForce;
            if (!IsCannonSucking) 
            {
                _suctionAreaCurrent.position = _barrelPoint.position;
                _suctionForce = _minSuctionForce;
                _suckingParticles.Stop();
                _suckingParticles.Clear();
            }
        }

        private void BuildUpShootForce()
        {
            if (ShootForce == _maxShootForce)
                return;

            if(ShootForce <= _maxShootForce)
            {
                ShootForce += Time.deltaTime * _shootForceIncreaseMultiplier;
            }
            else if(ShootForce >= _maxShootForce)
            {
                ShootForce = _maxShootForce;
            }

            ApplyGunShake();
        }

        private void ApplyGunShake()
        {
            Quaternion shake = Quaternion.Euler(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                );
            _gunTransform.localRotation = _originalGunRotation * shake;
        }

        private void OnCannonShoot(InputValue inputValue)
        {
            if (AmmoList.Count == 0)
                return;

            if (!_isShotBuildingUp && !_isCannonShooting)
            {
                _isShotBuildingUp = true;
                _shootingParticles.Play();
            }      
            else if(_isShotBuildingUp && !_isCannonShooting)
            {
                _isShotBuildingUp = false;
                _isCannonShooting = true;
                _shootingParticles.Stop();
                _shootingParticles.Clear();
            }            

        }

        private Vector3 GetAimDirection()
        {
            // Shoot ray to crosshair
            Ray ray = _mainCamera.ScreenPointToRay(_crosshairTransform.position);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * _range, Color.red, 2f, true);
            Physics.Raycast(ray, out hit, _range);

            if (hit.collider == null)
            {
                hit.point = ray.origin + ray.direction * _range;
            }


            Debug.DrawLine(ray.origin, hit.point, Color.blue, 2f, true);

            _barrelPoint.rotation = Quaternion.LookRotation(hit.point);

            // Get direction from shootpoint to hitpoint
            Vector3 direction = (hit.point - _barrelPoint.position).normalized;
            //float randomStrength = Random.Range(0, _offsetStrengthMax);
            //direction = (direction + Random.insideUnitSphere * randomStrength).normalized;
            Debug.DrawLine(_barrelPoint.position, direction * _range, Color.green, 2f);

            return direction;

        }

        private void ShootCannon()
        {
            CannonShot?.Invoke(this, EventArgs.Empty);
            GameObject obj = AmmoList[AmmoList.Count - 1].gameObject;
            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = GetAimDirection();
                SuckableObjectPresenter skObject = obj.GetComponent<SuckableObjectPresenter>();
                if (skObject != null)
                {
                    skObject.Model.IsShot = true;
                    skObject.Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    skObject.Model.ResetTTL();
                }

                // Get object slightly out of the way of barrel
                obj.transform.position = _barrelPoint.position + _barrelPoint.forward * 0.1f;

                // Allign object with shoot direction
                obj.transform.rotation = Quaternion.LookRotation(direction);
                obj.SetActive(true);

                // Ensure obj doesnt have leftover velocity from somewhere, set collisiondetection and shoot
                rb.linearVelocity = Vector3.zero;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.AddForce(direction * ShootForce, ForceMode.Impulse);
                UpdateAmmoList(false, obj);
                //AmmoList.Remove(obj);
            }

            UpdateCapacityText();
            ShootForce = 0f;
            _isCannonShooting = false;
            CannonShot?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateAmmoList(bool add, GameObject ammoObject)
        {
            if(add)
            {
                AmmoList.Add(ammoObject);
            }
            else
            {
                AmmoList.Remove(ammoObject);
            }
            HandleCurrentAmmoObject();
        }

        /// <summary>
        /// Try to reserve an ammo slot for an object that is about to be sucked.
        /// Returns true when reservation succeeded and the caller can proceed with sucking coroutine.
        /// </summary>
        public bool TryReserveAmmoSlot(GameObject obj)
        {
            if (obj == null)
                return false;

            // Already in list or reserved -> cannot reserve
            if (AmmoList.Contains(obj) || _reservedAmmo.Contains(obj))
                return false;

            // If current real ammo + reserved slots already fill capacity, reject
            int totalTaken = AmmoList.Count + _reservedAmmo.Count;
            if (totalTaken >= MaxAmmoCount)
                return false;

            _reservedAmmo.Add(obj);
            UpdateCapacityText();
            return true;
        }

        /// <summary>
        /// Confirm a previously reserved object and move it into the actual ammo list.
        /// </summary>
        public void ConfirmReservedAmmo(GameObject obj)
        {
            if (obj == null)
                return;

            if (_reservedAmmo.Remove(obj))
            {
                AmmoList.Add(obj);
                HandleCurrentAmmoObject();
            }
            UpdateCapacityText();
        }

        /// <summary>
        /// Release a reservation if the sucking attempt failed or was cancelled.
        /// </summary>
        public void ReleaseReservedAmmo(GameObject obj)
        {
            if (obj == null)
                return;

            if (_reservedAmmo.Remove(obj))
            {
                UpdateCapacityText();
            }
        }

        private void HandleCurrentAmmoObject()
        {
            if (AmmoList.Count > 0)
            {
                GameObject currentobj = AmmoList.Last();
                if (_previousAmmoObject == null || currentobj != _previousAmmoObject)
                {
                    _previousAmmoObject = currentobj;
                    AmmoChanged?.Invoke(this, new AmmoChangedEventArgs(currentobj));
                }
            }
            else
                AmmoChanged?.Invoke(this, new AmmoChangedEventArgs(null));
        }

        public void UpdateCapacityText()
        {
            // Show reserved slots as part of the "taken" count to reflect immediate reservations
            int realCount = AmmoList.Count;
            int reservedCount = _reservedAmmo.Count;
            int taken = realCount + reservedCount;

            if(AmmoCapacityText != null && AmmoCapacityText.enabled)
                AmmoCapacityText.text = $"{taken} / {MaxAmmoCount}";

            for(int i = 0; i < AmmoCapacityImages.Count; i++)
            {
                Image img = AmmoCapacityImages[i];
                if(i < MaxAmmoCount)
                {
                    if (!img.gameObject.activeSelf)
                        img.gameObject.SetActive(true);

                    // Decide color: filled (owned), reserved, or empty
                    if (i < realCount)
                        img.color = _filledColor;
                    else if (i < realCount + reservedCount)
                        img.color = _reservedColor;
                    else
                        img.color = _emptyColor;
                }
                else
                {
                    if (img.gameObject.activeSelf)
                        img.gameObject.SetActive(false);
                }
            }            
        }

        private void UpdateShootForceText()
        {
            if(ShootForceText != null && ShootForceText.enabled)
                ShootForceText.text = $"Shootforce: {ShootForce}/{_maxShootForce}";

            ShootForceBar.fillAmount = _shootForceProgress;
            //ShootForceBar.color = Color.Lerp(_originalShootForceBarColor, ShootForceLerpColor, _shootForceProgress);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_barrelPoint.position, _suctionRange);
            Gizmos.DrawLine(_barrelPoint.position, _suctionAreaCurrent.position);
        }
    }


    public class AmmoChangedEventArgs : EventArgs
    {
        public GameObject AmmoObject { get; private set; }

        public AmmoChangedEventArgs(GameObject ammoObject)
        {
            AmmoObject = ammoObject;
        }
    }


}
