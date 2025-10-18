using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ThrashSucker.Presenters
{
    public class SuckingTrigger : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private SuckingCannonPresenter _canon;

        private void OnTriggerEnter(Collider other)
        {
            GameObject obj = other.gameObject;
            if (obj != null && _layerMask == (_layerMask | (1 << obj.layer)) && _canon.IsCannonSucking)
            {
                if (_canon.AmmoList.Count < _canon.MaxAmmoCount)
                {
                    _canon.AmmoList.Add(obj);
                    obj.SetActive(false);
                    _canon.UpdateCapacityText();
                }
            }
        }
    }
}
