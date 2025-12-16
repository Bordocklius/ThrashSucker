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
