using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashSucker.Models;
using UnityEngine;
using TrashSucker.Singleton;

namespace TrashSucker.Presenters
{
    public class GameManagerPresenter: PresenterBaseClass<GameManager>
    {
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
        }

    }
}
