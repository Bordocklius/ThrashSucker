using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrashSucker.Models.ShredderModels;
using UnityEngine;

namespace ThrashSucker.Presenters
{
    public class ShredderPresenter: PresenterBaseClass<Shredder>
    {     
        public float ShredDuration {  get; set; }
        public LayerMask HitableLayer {  get; set; }

        private Coroutine _processingRoutine;

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

        private IEnumerator ProcessObjects()
        {
            while(Model.HasObjects)
            {
                yield return new WaitForSeconds(ShredDuration);
                GameObject obj = Model.ShredObject();
                Destroy(obj);
            }

            _processingRoutine = null;
        }

        public void AddObject(GameObject obj)
        {
            Model.AddObject(obj);

            if(_processingRoutine == null)
                _processingRoutine = StartCoroutine(ProcessObjects());
        }

        protected virtual void Model_OnObjectShredded(object sender, EventArgs e)
        {
            Debug.Log("Obj shredded, shredding next..");
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
