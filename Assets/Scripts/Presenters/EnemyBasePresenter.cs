using System.Collections.Generic;
using System.ComponentModel;
using ThrashSucker.Models;
using ThrashSucker.Models.Enemies;
using ThrashSucker.Presenters;
using UnityEngine;

public class EnemyBasePresenter : PresenterBaseClass<Enemybase>
{
    [SerializeField]
    private Transform _transform;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private float _movementSpeed;

    public float StartingHP;
    public float Damage;

    public List<MaterialType> EnemyWeaknesses;
    public List<MaterialType> EnemyResistances;

    protected override void Model_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(Enemybase.Health)))
        {
            if (Model.Health == 0)
                OnEnemyDeath();
        }
    }

    protected override void ModelSetInitialisation(Enemybase previousModel)
    {
        //base.ModelSetInitialisation(previousModel);
        //if (previousModel != null)
        //{
        //    previousModel.TTLExpired -= OnTTLExpired;
        //}
        //Model.TTLExpired += OnTTLExpired;
    }

    private void Awake()
    {
        Model = new Enemybase(EnemyWeaknesses, EnemyResistances, StartingHP, Damage);
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.Find("Player").transform;
        }
    }


    // Update is called once per frame
    protected override void Update()
    {
        _transform.position = Vector3.MoveTowards(_transform.position, _playerTransform.position, _movementSpeed * Time.deltaTime);
    }

    public void DamageEnemy(SuckableObject suckableObject)
    {
        Model.OnEnemyShot(suckableObject);
    }

    public virtual void OnEnemyDeath()
    {
        Destroy(this.gameObject);
    }
}
