using UnityEngine;
using ThrashSucker.Models;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace ThrashSucker.Presenters 
{ 
    public class SuckableObjectPresenter : PresenterBaseClass<SuckableObject>
    {
        public Rigidbody Rb;
        public float TTL;
        public List<GameObject> SubObjects = new List<GameObject>();
        public int ObjectHealth;
        public LayerMask HitableLayer;

        private LayerMask _currentLayerMask;
        [SerializeField]
        private float _layermaskActivateTimer;

        [SerializeField]
        private float _objectSpread = 0.2f;
        private Vector3 _contactNormal;

        [SerializeField]
        private ParticleSystem _deathParticles;

        protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(SuckableObject.ObjectHealth)))
            {
                if (Model.ObjectHealth == 0)
                    OnHealthDepleted();
            }            
        }

        protected override void ModelSetInitialisation(SuckableObject previousModel)
        {
            base.ModelSetInitialisation(previousModel);
            if(previousModel != null)
            {
                previousModel.TTLExpired -= OnTTLExpired;
            }
            Model.TTLExpired += OnTTLExpired;
        }

        private void Awake()
        {
            Model = new SuckableObject(ObjectHealth ,TTL);
            Rb = GetComponent<Rigidbody>();            
        }

        private void OnEnable()
        {
            _currentLayerMask = LayerMask.GetMask("Default");
            DelayLayerMask();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private IEnumerator DelayLayerMask()
        {
            yield return new WaitForSeconds(_layermaskActivateTimer);
            _currentLayerMask = HitableLayer;
        }

        private void OnHealthDepleted()
        {
            if(SubObjects.Count > 0)
            {
                foreach (GameObject obj in SubObjects)
                {
                    Instantiate(obj, transform.position, transform.rotation);
                    Rigidbody rb = obj.GetComponent<Rigidbody>();
                    
                    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
                    Vector3 finalDirection = Vector3.Slerp(_contactNormal, randomDirection, _objectSpread);

                    rb.AddForce(randomDirection * UnityEngine.Random.Range(0, 5), ForceMode.Impulse);
                }
            }
            // Detatch particle system to ensure playback of particles
            if(_deathParticles != null)
            {
                _deathParticles.transform.parent = null;
                _deathParticles.Play();
                Destroy(_deathParticles.gameObject, _deathParticles.main.duration);
            }
            Destroy(this.gameObject);
        }



        private void OnTTLExpired(object sender, EventArgs e)
        {
            Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }

        private void OnCollisionEnter(Collision collision)
        {            
            if (collision != null && ((1 << collision.gameObject.layer) & HitableLayer) != 0 && Model.IsShot)
            {
                Model.ObjectHealth--;
                if(Model.ObjectHealth <= 0)
                {
                    // Save surface normal of contact point to throw new spawned objects away from contact point
                    _contactNormal = collision.contacts[0].normal;
                }
                
                if(collision.collider.TryGetComponent<EnemyBasePresenter>(out EnemyBasePresenter enemy)) {
                    if(enemy != null)
                    {
                        enemy.DamageEnemy();
                    }
                }
            }
        }


    }
}
