using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TrashSucker.Presenters
{
    public class PlayerSprintBarPresenter: MonoBehaviour
    {
        [SerializeField]
        private PlayerMovementPresenter _playerPresenter;

        [SerializeField]
        private Image _sprintBar;

        private void Update()
        {
            _sprintBar.fillAmount = _playerPresenter.SprintTimer / _playerPresenter.MaxSprintTimer;
        }
    }
}
