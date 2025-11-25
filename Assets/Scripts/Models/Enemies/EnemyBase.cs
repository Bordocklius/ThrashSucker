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

        public Enemybase(List<MaterialType> weaknesses, List<MaterialType> resistances, float startingHP)
        {
			EnemyWeaknesses = weaknesses;
			EnemyResistances = resistances;
			Health = startingHP;
        }

		public virtual void OnEnemyShot(SuckableObject suckableObject)
		{
			if (suckableObject == null) 
				return;

			if(EnemyWeaknesses.Contains(suckableObject.MaterialType))
			{
				Health -= suckableObject.Damage * 2;
			}
			else if(EnemyWeaknesses.Contains(suckableObject.MaterialType))
			{
                Health -= suckableObject.Damage * 0.5f;
			}
			else
			{
				Health -= suckableObject.Damage;
			}
		}

    }
}