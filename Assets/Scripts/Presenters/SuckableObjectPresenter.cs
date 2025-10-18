using UnityEngine;
using ThrashSucker.Models;
using System.ComponentModel;
using System;

namespace ThrashSucker.Presenters 
{ 
    public class SuckableObjectPresenter : PresenterBaseClass<SuckableObject>
    {
        public Rigidbody Rb;
        public float TTL;

        protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(SuckableObject.ObjectHealth))) return;
        }

        protected override void ModelSetInitialisation(SuckableObject previousModel)
        {
            base.ModelSetInitialisation(previousModel);
            if(previousModel != null)
            {
                previousModel.TTLExpired -= OnTTLExpired;
                previousModel.HealthDepleted -= OnHealthDepleted;
            }
            Model.TTLExpired += OnTTLExpired;
            Model.HealthDepleted += OnHealthDepleted;
        }

        protected void Awake()
        {
            Model = new SuckableObject(TTL);
            Rb = GetComponent<Rigidbody>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void OnHealthDepleted(object sender, EventArgs e)
        {
            Destroy(this.gameObject);
        }

        private void OnTTLExpired(object sender, EventArgs e)
        {
            Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
    }
}
