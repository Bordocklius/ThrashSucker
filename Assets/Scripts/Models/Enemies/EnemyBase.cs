using System.Collections.Generic;
using UnityEngine;

namespace TrashSucker.Models.Enemies
{
    public partial class Enemybase: UnityModelBaseClass
    {
		private float _health;
		public float Health
		{
			get { return _health; }
			set
			{
				if (_health == value)
					return;

				_health = value;
				OnPropertyChanged();
			}
		}

		public List<MaterialType> EnemyWeaknesses;
        public List<MaterialType> EnemyResistances;

		public float Damage = 2;

		// Navmesh stuff
		public float RandomRadius = 2f;
		public int MaxPickAttempts = 2;
		public float WanderTimeout = 5f;
		public float StuckTimeout = 5f;
		public float DetectionEnterRadius = 10f;
		public float DetectionExitRadius = 20f;
		public float MinTargetDistance = 2f;
		private EnemyMovementFSM _movementFSM;

        public Enemybase(List<MaterialType> weaknesses, List<MaterialType> resistances, float startingHP, float damage, IEnemyMovement adapter, float randomRadius)
        {
			EnemyWeaknesses = weaknesses;
			EnemyResistances = resistances;
			Health = startingHP;
			Damage = damage;

			RandomRadius = randomRadius;
			_movementFSM = new EnemyMovementFSM(this, adapter);
        }

        public override void Update(float deltaTime)
        {
            _movementFSM.Update(deltaTime);
        }

		public virtual void OnEnemyShot(SuckableObject suckableObject)
		{
			if (suckableObject == null) 
				return;

			float damageReceived = suckableObject.Damage;

			if(EnemyWeaknesses.Contains(suckableObject.MaterialType))
			{
				damageReceived *= 2;
			}
			else if(EnemyResistances.Contains(suckableObject.MaterialType))
			{
                damageReceived *= 0.5f;
			}

			Health -= damageReceived;
		}

    }
}