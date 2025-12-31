using System;
using TrashSucker.Singleton;
using TMPro;
using UnityEngine;

namespace TrashSucker.Models
{
    public class SuckableObject: UnityModelBaseClass
    {
        public event EventHandler TTLExpired;

        private bool _isShot;
        public bool IsShot
        {
            get { return _isShot; }
            set
            {
                if (_isShot == value)
                    return;

                _isShot = value;
                OnPropertyChanged();
            }
        }

        private int _objectHealth;
        public int ObjectHealth
        {
            get { return _objectHealth; }
            set
            {
                if (_objectHealth == value)
                    return;

                _objectHealth = value;
                OnPropertyChanged();
            }
        }

        public int Damage;

        public MaterialType MaterialType { get; set; }

        [SerializeField]
        private float _shotLifetime;
        private float _shotTimer;

        public SuckableObject(int objHealth ,float timeToLive, int damage, MaterialType materialType)
        {
            ObjectHealth = objHealth;
            _shotLifetime = timeToLive;
            Damage = damage;
            MaterialType = materialType;
        }

        public override void FixedUpdate(float fixedDeltaTime)
        {
            if(_shotLifetime == 0)
                return;

            if(IsShot)
            {
                _shotTimer += fixedDeltaTime;
                if(_shotTimer >= _shotLifetime)
                {
                    IsShot = false;
                    TTLExpired?.Invoke(this, EventArgs.Empty);
                }
            }
        }

    }

    public enum MaterialType
    {
        None,
        Wood,
        Metal
    }

}
