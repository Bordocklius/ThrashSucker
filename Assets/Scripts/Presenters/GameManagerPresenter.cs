using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TrashSucker.Models;
using TrashSucker.Models.Enemies;
using TrashSucker.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrashSucker.Presenters
{
    public class GameManagerPresenter: PresenterBaseClass<GameManager>
    {
        public static bool IsPaused;

        [SerializeField]
        private TextMeshProUGUI _objectiveText;
        [SerializeField]
        private PlayerMovementPresenter _player;
        [SerializeField]
        private int _objectsToShredForHPRestore;
        [SerializeField]
        private int _healthRestore;

        protected override void ModelSetInitialisation(GameManager previousModel)
        {
            base.ModelSetInitialisation(previousModel);
        }

        protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(GameManager.ShreddedObjectsCount)))
            {
                Model_OnObjShredded();
            }
        }

        private void Awake()
        {
            Model = Singleton<GameManager>.Instance;
            Model.TrashEvent += Model_OnTrashEvent;
            Model.ObjectsToShredForHPResore = _objectsToShredForHPRestore;
            Model.ShreddedObjectsCount = _objectsToShredForHPRestore;
            Model.HealthRestore = _healthRestore;
        }

        private void Start()
        {
            SetObjectiveText(Model.TrashObjects.Count.ToString());
        }

        protected override void Update()
        {
            if(_player.Health <= 0)
            {                
                SceneLoader.LoadSceneByName("StartScreen");
            }
        }

        protected void Model_OnTrashEvent(object sender, EventArgs e)
        {
            SetObjectiveText(Model.TrashObjects.Count.ToString());
        }

        private void SetObjectiveText(string amount)
        {
            _objectiveText.text = $"Trash left: {amount}";
        } 

        private void Model_OnObjShredded()
        {
            if(Model.ShreddedObjectsCount <= 0)
            {
                _player.Health += Model.HealthRestore;
            }
        }
        
    }
}
