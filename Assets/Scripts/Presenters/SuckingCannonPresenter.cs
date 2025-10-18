using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThrashSucker.Presenters
{
    public class SuckingCannonPresenter : MonoBehaviour
    {
        public List<GameObject> AmmoList = new List<GameObject>();
        public int MaxAmmoCount;

        [SerializeField]
        private Transform _barrelPoint;
        [SerializeField]
        private float _suctionRange;
        
        [SerializeField]
        private float _maxSuctionForce;
        [SerializeField]
        private float _suctionIncreaseMultiplier;
        private float _suctionForce;

        [SerializeField]
        private float _shootForce;

        [SerializeField]
        private float _range;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Transform _crosshairTransform;

        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        public bool IsCannonSucking;

        public TextMeshProUGUI Text;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            UpdateCapacityText();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsCannonSucking)
            {
                ActivateCannonSuction();
            }
        }

        private void ActivateCannonSuction()
        {
            // Progressively increase suction power
            if (_suctionForce < _maxSuctionForce)
                _suctionForce += Time.deltaTime * _suctionIncreaseMultiplier;
            else if(_suctionForce > _maxSuctionForce)
                _suctionForce = _maxSuctionForce;

            List<Collider> colliders = Physics.OverlapSphere(_barrelPoint.position, _suctionRange, _layerMask).ToList();
            foreach (Collider collider in colliders)
            {
                GameObject obj = collider.gameObject;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (_barrelPoint.position - rb.position).normalized;
                    rb.AddForce(direction * _suctionForce, ForceMode.Acceleration);

                    //// Add fallof of force based on distance
                    //float distance = Vector3.Distance(_barrelPoint.position, rb.position);
                    //float fallof = 1 - (distance / _suctionRange);
                    //float appliedForce = _suctionForce * fallof;
                    //rb.AddForce(direction * appliedForce, ForceMode.Acceleration);
                }
            }
        }

        private void OnActivateSuck(InputValue inputValue)
        {
            IsCannonSucking = !IsCannonSucking;
            if (!IsCannonSucking)
                _suctionForce = 0f;
        }

        private void OnCannonShoot(InputValue inputValue)
        {
            if (AmmoList.Count == 0)
                return;

            GameObject obj = AmmoList[AmmoList.Count - 1].gameObject;
            if (obj.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = GetAimDirection();
                SuckableObjectPresenter skObject = obj.GetComponent<SuckableObjectPresenter>();
                if (skObject != null)
                {
                    skObject.Model.IsShot = true;
                    skObject.Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                }
                obj.transform.position = _barrelPoint.position;
                obj.SetActive(true);
                rb.AddForce(direction * _shootForce, ForceMode.Impulse);
                AmmoList.Remove(obj);
            }
            UpdateCapacityText();

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

            // Get direction from shootpoint to hitpoint
            Vector3 direction = (hit.point - _barrelPoint.position).normalized;
            //float randomStrength = Random.Range(0, _offsetStrengthMax);
            //direction = (direction + Random.insideUnitSphere * randomStrength).normalized;
            Debug.DrawLine(_barrelPoint.position, direction * _range, Color.green, 2f);

            return direction;

        }

        public void UpdateCapacityText()
        {
            Text.text = $"{AmmoList.Count} / {MaxAmmoCount}";
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_barrelPoint.position, _suctionRange);
        }
    }

}
