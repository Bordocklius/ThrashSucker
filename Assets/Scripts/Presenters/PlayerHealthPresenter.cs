using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TrashSucker.Presenters
{
    public class PlayerHealthPresenter: MonoBehaviour
    {
        [SerializeField]
        private PlayerMovementPresenter _playerPresenter;

        [SerializeField]
        private Image _healthBar;

        private float _startingHP;

        private void Start()
        {
            _startingHP = _playerPresenter.StartingHP;
        }

        private void Update()
        {
            _healthBar.fillAmount = _playerPresenter.Health / _playerPresenter.StartingHP;
        }
    }
}
