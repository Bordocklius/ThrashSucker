using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThrashSucker.Presenters
{
    public class SuckingCannon : MonoBehaviour
    {
        public List<GameObject> AmmoList = new List<GameObject>();
        public int MaxAmmoCount;

        [SerializeField]
        private Transform _barrelPoint;
        [SerializeField]
        private float _suctionRange;
        
        [SerializeField]
        private float _maxSuctionForce;
        private float _suctionForce;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
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
            if (_suctionForce < _maxSuctionForce)
                _suctionForce += Time.deltaTime;
            else if(_suctionForce > _maxSuctionForce)
                _suctionForce = _maxSuctionForce;
            Debug.Log(_suctionForce);

                List<Collider> colliders = Physics.OverlapSphere(_barrelPoint.position, _suctionRange, _layerMask).ToList();
            foreach (Collider collider in colliders)
            {
                GameObject obj = collider.gameObject;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 direction = (_barrelPoint.position - rb.position).normalized;
                    rb.AddForce(direction * _suctionForce, ForceMode.Acceleration);
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
            GetAimDirection();
            //GameObject obj = AmmoList[AmmoList.Count - 1].gameObject;
            //Rigidbody rb = obj.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    Vector3 direction = GetAimDirection();
            //    BallpitBall ball = obj.GetComponent<BallpitBall>();
            //    if (ball != null)
            //    {
            //        ball.IsShot = true;
            //        ball.Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            //    }
            //    obj.transform.position = _barrelPoint.position;
            //    obj.SetActive(true);
            //    rb.AddForce(direction * _shootForce, ForceMode.Impulse);
            //    AmmoList.Remove(obj);
            //}
            //Text.text = AmmoList.Count.ToString();

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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_barrelPoint.position, _suctionRange);
        }
    }

}
