using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashSucker.Models.ShredderModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TrashSucker.Presenters
{
    public class ShredderPresenter: PresenterBaseClass<Shredder>
    {
        public float StartingShredDuration;
        public LayerMask HitableLayer;
        private Coroutine _processingRoutine;

        [SerializeField]
        private TextMeshProUGUI _objText;
        [SerializeField]
        private GameObject _progressBarObj;
        [SerializeField]
        private Image _progressBarImage;
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField]
        private Color _lerpColor;

        private Color _startColor;

        private float _progress;
        private float Progress
        {
            get { return _progress;}
            set
            {
                if(_progress == value) return;

                _progress = value;
                OnProgressChanged();
            }
        }


        protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        protected override void ModelSetInitialisation(Shredder previousModel)
        {
            if(previousModel != null)
            {
                Model.ObjectShredded -= Model_OnObjectShredded;
            }
            Model.ObjectShredded += Model_OnObjectShredded;
        }

        private void Awake()
        {
            Model = new Shredder();
            _startColor = _progressBarImage.color;
        }

        private IEnumerator ProcessObjects()
        {
            _particleSystem.Play();
            _progressBarObj.SetActive(true);
            while(Model.HasObjects)
            {
                float timer = 0f;
                while(timer <= StartingShredDuration)
                {
                    timer += Time.deltaTime;
                    Progress = Mathf.Clamp01(timer / StartingShredDuration);
                    yield return null;
                }
                
                Model.ShredObject();
                ChangeText();
            }

            _processingRoutine = null;
            _particleSystem.Stop();
            _progressBarObj.SetActive(false);
        }

        public void AddObject(GameObject obj)
        {
            Model.AddObject(obj);
            ChangeText();

            if(_processingRoutine == null)
                _processingRoutine = StartCoroutine(ProcessObjects());
        }

        protected virtual void Model_OnObjectShredded(object sender, ItemShreddedEventArgs e)
        {
            ChangeText();
            Destroy(e.Object);
        }

        private void ChangeText()
        {
            _objText.text = $"{Model.StoredObjects.Count} objects";
        }

        private void OnProgressChanged()
        {
            _progressBarImage.fillAmount = Progress;
            _progressBarImage.color = Color.Lerp(_startColor, _lerpColor, Progress);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other != null && ((1 << other.gameObject.layer) & HitableLayer) != 0)
            {
                AddObject(other.gameObject);
                other.gameObject.SetActive(false);
            }        
        }
    }
}
