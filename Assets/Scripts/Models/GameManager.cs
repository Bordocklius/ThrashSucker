using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrashSucker.Presenters;
using UnityEngine;

namespace ThrashSucker.Models
{
    public class GameManager: UnityModelBaseClass
    {
        public List<GameObject> ThrashObjects = new List<GameObject>();

        public GameManager()
        {
        }

        public void AddThrashObject(GameObject obj)
        {
            if(obj.TryGetComponent<SuckableObjectPresenter>(out SuckableObjectPresenter thrashObj))
            {
                ThrashObjects.Add(obj);
            }
        }

        public void RemoveThrashObject(GameObject obj)
        {
            ThrashObjects.Remove(obj);
            Debug.Log($"Obj removed, {ThrashObjects.Count} left");
            if(ThrashObjects.Count <= 0 )
            {
                Debug.Log("You won lol");
            }
        }

    }
}
