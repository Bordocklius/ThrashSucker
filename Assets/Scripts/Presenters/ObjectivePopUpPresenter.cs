using System.Collections;
using TMPro;
using UnityEngine;

namespace TrashSucker.Presenters
{
    public class ObjectivePopUpPresenter : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _to;
        private Vector3 _from;
        private RectTransform _rectTransform;

        [SerializeField]
        private float _lerpSpeed;

        [SerializeField]
        private float _popUpDuration;

        [SerializeField]
        private TextMeshProUGUI _textField;

        public bool HasEnteredTrigger;

        private void Awake()
        {
            _rectTransform = this.transform as RectTransform;
            _from = _rectTransform.position;           
        }

        private void Start()
        {
            //StartCoroutine(AnimatePopupOverTime(_popUpDuration));
        }

        public IEnumerator AnimatePopupOverTime()
        {
            Vector3 startPosition = _from;
            Vector3 endPosition = _to.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _lerpSpeed;
                _rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }
            _rectTransform.position = _to.position;

            yield return new WaitForSeconds(_popUpDuration);

            startPosition = _to.position;
            endPosition = _from;

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _lerpSpeed;
                _rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }
            _rectTransform.position = _from;
        }

        public IEnumerator AnimatePopup()
        {
            Vector3 startPosition = _from;
            Vector3 endPosition = _to.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _lerpSpeed;
                _rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }
            _rectTransform.position = _to.position;

            yield return new WaitUntil(() => !HasEnteredTrigger);

            //while(HasEnteredTrigger)
            //{
            //    yield return new Wait;
            //}

            startPosition = _to.position;
            endPosition = _from;

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _lerpSpeed;
                _rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }
            _rectTransform.position = _from;
        }

        public void SetText(string text)
        {
            _textField.text = text;
        }
    }
}

