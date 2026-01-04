using UnityEngine;
using UnityEngine.UIElements;

namespace TrashSucker.Presenters
{
    public class ObjectivePopUpTriggerZone: MonoBehaviour
    {
        [SerializeField]
        private ObjectivePopUpPresenter _popUpPresenter;

        [SerializeField]
        private string _text;

        [SerializeField]
        private BoxCollider _collider;

        [SerializeField]
        private bool _timed;

        private void Awake()
        {
            if (_popUpPresenter == null)
                _popUpPresenter = GameObject.FindGameObjectWithTag("ObjectivePopUp").GetComponent<ObjectivePopUpPresenter>();
            if (_collider == null)
                _collider = this.GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent<PlayerMovementPresenter>(out PlayerMovementPresenter player))
            {
                _popUpPresenter.SetText(_text);
                _popUpPresenter.HasEnteredTrigger = true;
                if (_timed)
                    StartCoroutine(_popUpPresenter.AnimatePopupOverTime());
                else
                    StartCoroutine(_popUpPresenter.AnimatePopup());
            }           
        }

        private void OnTriggerExit(Collider other)
        {
            _popUpPresenter.HasEnteredTrigger = false;
        }

        private void OnDrawGizmos()
        {
            if (_collider == null)
                return;

            // Draw wire cube at collider center with collider size
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_collider.center, _collider.size);
        }
    }
}
