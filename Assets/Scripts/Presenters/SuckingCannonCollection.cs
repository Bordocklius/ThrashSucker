using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrashSucker.Presenters
{
    public class SuckingTrigger : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _layerMask;

        [SerializeField]
        private SuckingCannonPresenter _canon;

        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _suckClip;
        

        [SerializeField]
        private float _suckTime;
        [SerializeField]
        private Vector3 _scaleTo = new Vector3(0.1f, 0.1f, 0.1f);

        [SerializeField]
        private Transform _transform;

        private bool _isSucking;

        private void Awake()
        {
            _canon.CannonShot += ToggleActive;
            if(_transform == null)
                _transform = this.transform;
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
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleSuckingLogic(other);
        }

        //private void OnTriggerStay(Collider other)
        //{ 
        //    HandleSuckingLogic(other);   
        //}

        private void HandleSuckingLogic(Collider other)
        {          
            GameObject obj = other.gameObject;
            if (obj != null && _layerMask == (_layerMask | (1 << obj.layer)) && _canon.IsCannonSucking)
            {
                // avoid starting duplicate sucking work
                if (_canon.AmmoList.Contains(obj))
                    return;

                // try to reserve a slot; this prevents race between multiple triggers/coroutines
                if (!_canon.TryReserveAmmoSlot(obj))
                    return;

                StartCoroutine(SuckUpObject(obj));    
            }
        }

        private IEnumerator SuckUpObject(GameObject obj)
        {
            _isSucking = true;
            Transform objTransform = obj.transform;
            Vector3 startPosition = objTransform.position;
            Vector3 startScale = objTransform.localScale;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _suckTime;
                objTransform.position = Vector3.Lerp(startPosition, _transform.position, t);
                objTransform.localScale = Vector3.Lerp(startScale, _scaleTo, t);
                yield return null;
            }
            objTransform.position = _transform.position;
            objTransform.localScale = _scaleTo;

            obj.SetActive(false);
            objTransform.localScale = startScale;

            // Confirm the reserved slot and move the object into the real ammo list
            _canon.ConfirmReservedAmmo(obj);
            _canon.UpdateCapacityText();
            _audioSource.PlayOneShot(_suckClip);
            _isSucking = false;
        }
    }
}
