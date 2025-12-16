using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashSucker.Models;
using UnityEngine;
using TrashSucker.Singleton;
using TMPro;

namespace TrashSucker.Presenters
{
    public class GameManagerPresenter: PresenterBaseClass<GameManager>
    {
        [SerializeField]
        private TextMeshProUGUI _objectiveText;

        protected override void ModelSetInitialisation(GameManager previousModel)
        {
            base.ModelSetInitialisation(previousModel);
        }

        protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        private void Awake()
        {
            Model = Singleton<GameManager>.Instance;
            Model.TrashEvent += Model_OnTrashEvent;
        }

        private void Start()
        {
            SetObjectiveText(Model.TrashObjects.Count.ToString());
        }

        protected void Model_OnTrashEvent(object sender, EventArgs e)
        {
            SetObjectiveText(Model.TrashObjects.Count.ToString());
        }

        private void SetObjectiveText(string amount)
        {
            _objectiveText.text = $"Trash left: {amount}";
        } 
    }
}
