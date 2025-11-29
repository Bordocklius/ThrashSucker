using System.Collections.Generic;

namespace ThrashSucker.Models.Enemies
{
    public class Enemybase: UnityModelBaseClass
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

        public Enemybase(List<MaterialType> weaknesses, List<MaterialType> resistances, float startingHP, float damage)
        {
			EnemyWeaknesses = weaknesses;
			EnemyResistances = resistances;
			Health = startingHP;
			Damage = damage;
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