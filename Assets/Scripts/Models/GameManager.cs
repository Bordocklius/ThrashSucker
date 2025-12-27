using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashSucker.Presenters;
using UnityEngine;

namespace TrashSucker.Models
{
    public class GameManager: UnityModelBaseClass
    {
        public event EventHandler TrashEvent;

        public List<GameObject> TrashObjects = new List<GameObject>();

        public List<GameObject> Enemies;

        public int ObjectsToShredForHPResore;
        public int HealthRestore;

        private int _shreddedObjectsCount;
        public int ShreddedObjectsCount
        {
            get { return _shreddedObjectsCount; }
            set
            {
                if (_shreddedObjectsCount == value)
                    return;

                _shreddedObjectsCount = value;
                OnPropertyChanged();
                if (_shreddedObjectsCount <= 0)
                {
                    _shreddedObjectsCount = ObjectsToShredForHPResore;
                }                
            }
        }

        public GameManager()
        {
        }


        public void AddThrashObject(GameObject obj)
        {
            if(obj.TryGetComponent<SuckableObjectPresenter>(out SuckableObjectPresenter thrashObj))
            {
                TrashObjects.Add(obj);
                TrashEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ObjectShredded()
        {
            ShreddedObjectsCount--;
        }

        public void RemoveThrashObject(GameObject obj)
        {
            TrashObjects.Remove(obj);
            TrashEvent?.Invoke(this, EventArgs.Empty);
            Debug.Log($"Obj removed, {TrashObjects.Count} left");
            if(TrashObjects.Count <= 0 )
            {
                Debug.Log("You won lol");
            }
        }

    }
}
