using System;
using UnityEngine;

namespace ThrashSucker.Models
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

        [SerializeField]
        private float _shotLifetime;
        private float _shotTimer;

        public SuckableObject(int objHealth ,float timeToLive)
        {
            ObjectHealth = objHealth;
            _shotLifetime = timeToLive;
        }

        public override void FixedUpdate(float fixedDeltaTime)
        {
            if(IsShot)
            {
                _shotTimer += fixedDeltaTime;
                if(_shotTimer >= _shotLifetime)
                {
                    IsShot = false;
                    ObjectHealth--;
                    TTLExpired?.Invoke(this, EventArgs.Empty);
                }
            }
        }

    }

}
