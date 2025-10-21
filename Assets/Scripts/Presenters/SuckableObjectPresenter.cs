using UnityEngine;
using ThrashSucker.Models;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ThrashSucker.Presenters 
{ 
    public class SuckableObjectPresenter : PresenterBaseClass<SuckableObject>
    {
        public Rigidbody Rb;
        public float TTL;
        public List<GameObject> SubObjects = new List<GameObject>();
        public int ObjectHealth;

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

        protected void Awake()
        {
            Model = new SuckableObject(ObjectHealth ,TTL);
            Rb = GetComponent<Rigidbody>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
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
                    rb.AddForce(randomDirection * 2, ForceMode.Impulse);
                }
            }            
            Destroy(this.gameObject);
        }

        private void OnTTLExpired(object sender, EventArgs e)
        {
            Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
    }
}
