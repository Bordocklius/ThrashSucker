using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TrashSucker.Presenters
{
    public class RenderTextureCam: MonoBehaviour
    {
        [SerializeField]
        private SuckingCannonPresenter _cannon;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Transform _renderPoint;

        private GameObject _currentObj;

        public RenderTexture ObjRenderTexture;

        private void Awake()
        {
            _cannon.AmmoChanged += RenderOutObj;
        }

        private void RenderOutObj(object sender, AmmoChangedEventArgs e)
        {
            if(_currentObj != null)
            {
                Destroy(_currentObj);
            }
            if(e.AmmoObject == null)
            {
                return;
            }

            _currentObj = Instantiate(e.AmmoObject, _renderPoint);
            _currentObj.transform.position = _renderPoint.position;
            _currentObj.transform.rotation = Quaternion.identity;
            _currentObj.SetActive(true);
            _currentObj.GetComponent<Rigidbody>().useGravity = false;
        }
    }
}
