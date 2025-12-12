using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TrashSucker.Models.ShredderModels
{
    public class Shredder: UnityModelBaseClass
    {
        public event EventHandler<ItemShreddedEventArgs> ObjectShredded;

        public Queue<GameObject> StoredObjects = new Queue<GameObject>();
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

        public void ShredObject()
        {
            if (StoredObjects.Count == 0) return;
            GameObject obj = StoredObjects.Dequeue();
            OnShredObject(obj);
        }

        public void OnShredObject(GameObject obj)
        {
            ObjectShredded?.Invoke(this, new ItemShreddedEventArgs(obj));
        }
    }

    public class ItemShreddedEventArgs : EventArgs
    {
        public GameObject Object { get; private set; }

        public ItemShreddedEventArgs(GameObject obj)
        {
            Object = obj;
        }
    }

}
