using UnityEngine;
using System.ComponentModel;
using ThrashSucker.Models;


namespace ThrashSucker.Presenters
{
    public abstract class PresenterBaseClass<T> : MonoBehaviour where T : UnityModelBaseClass
    {
        private T _model;
        public T Model
        {
            get { return _model; }
            set
            {
                T previousModel = null;
                if (_model == value)
                    return;
                if (_model != null)
                {
                    previousModel = _model;
                    _model.PropertyChanged -= Model_OnPropertyChanged;
                }
                _model = value;
                _model.PropertyChanged += Model_OnPropertyChanged;
                ModelSetInitialisation(previousModel);
            }
        }
        protected abstract void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e);

        protected virtual void ModelSetInitialisation(T previousModel)
        {

        }

        protected virtual void Update()
        {
            Model?.Update(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            Model?.FixedUpdate(Time.fixedDeltaTime);
        }
    }

}
