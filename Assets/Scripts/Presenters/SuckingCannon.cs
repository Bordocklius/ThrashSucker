using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThrashSucker.Presenters
{
    public class SuckingCannon : MonoBehaviour
    {
        public List<GameObject> AmmoList = new List<GameObject>();

        [SerializeField]
        private Transform _barrelPoint;
        [SerializeField]
        private float _suctionRange;
        
        [SerializeField]
        private float _maxSuctionForce;
        private float _suctionForce;

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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_barrelPoint.position, _suctionRange);
        }
    }

}
