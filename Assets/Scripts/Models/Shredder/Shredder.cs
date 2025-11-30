using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThrashSucker.Models.ShredderModels
{
    public class Shredder: UnityModelBaseClass
    {
        public event EventHandler ObjectShredded;

        public Queue<GameObject> StoredObjects = new Queue<GameObject>();
        public float ProcessingTime {  get; set; }
        public bool HasObjects
        {
            get
            {
                return StoredObjects.Count > 0;
            }
        }

        public Shredder() { }
        
        public void AddObject(GameObject obj)
        {
            StoredObjects.Enqueue(obj);
        }

        public GameObject ShredObject()
        {
            if (StoredObjects.Count == 0) return null;
            OnShredObject();
            return StoredObjects.Dequeue();
        }

        public void OnShredObject()
        {
            ObjectShredded?.Invoke(this, EventArgs.Empty);
        }
    }
}
