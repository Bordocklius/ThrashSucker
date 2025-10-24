using NUnit.Framework;
using System;
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

        [SerializeField]
        private AudioSource _audioSource;

        private void Awake()
        {
            _canon.CannonShot += ToggleActive;
        }

        private void ToggleActive(object sender, EventArgs e)
        {
            if(this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }
            Debug.Log(this.gameObject.activeSelf);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleSuckingLogic(other);
        }

        private void OnTriggerStay(Collider other)
        {
            HandleSuckingLogic(other);   
        }

        private void HandleSuckingLogic(Collider other)
        {
            GameObject obj = other.gameObject;
            if (obj != null && _layerMask == (_layerMask | (1 << obj.layer)) && _canon.IsCannonSucking)
            {
                if (_canon.AmmoList.Count < _canon.MaxAmmoCount)
                {
                    if (_canon.AmmoList.Contains(obj))
                        return;
                    _canon.UpdateAmmoList(true, obj);
                    //_canon.AmmoList.Add(obj);
                    obj.SetActive(false);
                    _canon.UpdateCapacityText();
                    _audioSource.Play();
                }
            }
        }
    }
}
