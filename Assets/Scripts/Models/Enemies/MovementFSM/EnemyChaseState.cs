using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrashSucker.Models.Enemies
{
    public partial class Enemybase
    {
        public class EnemyChaseState: EnemyMovementBaseState
        {
            public EnemyChaseState(EnemyMovementFSM fsm) : base(fsm) { }

            public override void OnEnter()
            {
                PlayerTrackingTimer = 0f;
                FSM.Adapter.RequestMoveTo(FSM.Adapter.GetTargetPosition());
            }

            public override void Update(float deltaTime)
            {
                PlayerTrackingTimer += deltaTime;

                if(PlayerTrackingTimer >= PlayerTrackingDelay)
                {
                    if (FSM.Adapter.GetDistanceToPlayer() < Context.DetectionExitRadius)
                    {
                        FSM.Adapter.RequestMoveTo(FSM.Adapter.GetTargetPosition());
                    }
                    else
                    {
                        LastTarget = FSM.Adapter.GetTargetPosition();
                        FSM.TransitionTo(FSM.EnemySearchState);
                    }

                    PlayerTrackingTimer -= PlayerTrackingDelay;
                }
            }
        }
    } 
    
}
