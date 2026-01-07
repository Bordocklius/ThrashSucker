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
using TrashSucker.Singleton;
using TrashSucker.Models;

namespace TrashSucker.Presenters
{
    public class ShredderPresenter: PresenterBaseClass<Shredder>
    {
        public float StartingShredDuration;
        public LayerMask HitableLayer;
        public LayerMask DamagableLayer;
        private Coroutine _processingRoutine;

        [SerializeField]
        private TextMeshProUGUI _objText;
        [SerializeField]
        private TextMeshProUGUI _objCountText;
        [SerializeField]
        private GameObject _progressBarObj;
        [SerializeField]
        private Image _progressBarImage;
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField]
        private Color _lerpColor;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _damageClip;

        [SerializeField]
        private float _damageRange;

        [SerializeField]
        private float _damage;

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

        private void Start()
        {
            ChangeObjectsText();
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
                Singleton<GameManager>.Instance.ObjectShredded();
                ChangeObjectsText();
            }

            _processingRoutine = null;
            _particleSystem.Stop();
            _progressBarObj.SetActive(false);
        }

        public void AddObject(GameObject obj)
        {
            Model.AddObject(obj);
            ChangeObjectsText();

            if(_processingRoutine == null)
                _processingRoutine = StartCoroutine(ProcessObjects());
        }

        protected virtual void Model_OnObjectShredded(object sender, ItemShreddedEventArgs e)
        {
            ChangeObjectsText();
            DamagePulse(e.Object);
            Destroy(e.Object);
        }

        private void ChangeObjectsText()
        {
            _objText.text = $"{Model.StoredObjects.Count} objects";
            _objCountText.text = $"{Singleton<GameManager>.Instance.ShreddedObjectsCount} until healing";
        }

        private void DamagePulse(GameObject obj)
        {
            bool didDamage = false;
            Collider[] objects = Physics.OverlapSphere(this.transform.position, _damageRange, DamagableLayer);
            foreach(Collider collider in objects)
            {
                if(collider.gameObject.TryGetComponent<EnemyBasePresenter>(out EnemyBasePresenter enemy))
                {
                    enemy.DamageEnemy(_damage, false);
                    didDamage = true;
                }
            }
            if (didDamage)
                _audioSource.PlayOneShot(_damageClip);
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(this.transform.position, _damageRange);
        }
    }
}
