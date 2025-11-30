using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrashSucker.Models.ShredderModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThrashSucker.Presenters
{
    public class ShredderPresenter: PresenterBaseClass<Shredder>
    {
        public float StartingShredDuration;
        public LayerMask HitableLayer;
        private Coroutine _processingRoutine;

        [SerializeField]
        private TextMeshProUGUI _objText;
        [SerializeField]
        private Image _progressBar;
        [SerializeField]
        private ParticleSystem _particleSystem;

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
        }

        private IEnumerator ProcessObjects()
        {
            _particleSystem.Play();
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
            _progressBar.fillAmount = Progress;
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
