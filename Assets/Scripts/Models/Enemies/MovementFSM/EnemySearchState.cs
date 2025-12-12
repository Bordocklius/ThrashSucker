using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashSucker.Models.Enemies
{
    public partial class Enemybase
    {
        public class EnemySearchState : EnemyMovementBaseState
        {
            private float _searchTimer;

            private float _searchDelay = 3f;

            public EnemySearchState(EnemyMovementFSM fsm) : base(fsm)
            {
            }

            public override void OnEnter()
            {
                _searchTimer = 0f;
                PlayerTrackingTimer = 0f;
                LastTarget = FSM.Adapter.GetTargetPosition();
                FSM.Adapter.RequestMoveTo(LastTarget);
            }

            public override void Update(float deltaTime)
            {
                // Check if enemy is back within range
                PlayerTrackingTimer += deltaTime;
                if(PlayerTrackingTimer >= PlayerTrackingDelay)
                {
                    if(FSM.Adapter.GetDistanceToPlayer() <= Context.DetectionEnterRadius)
                    {
                        FSM.TransitionTo(FSM.EnemyChaseState);
                        return;
                    }
                }

                // Search
                if(FSM.Adapter.IsAtDestination())
                {
                    _searchTimer += deltaTime;
                    if(_searchTimer >= _searchDelay)
                    {
                        FSM.TransitionTo(FSM.EnemyWanderState);
                        return ;
                    }
                }
            }
        }
    }
    
}
