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
            private float _playerTrackingTimer;

            private float _playerTrackingInterval = 0.1f;

            public EnemyChaseState(EnemyMovementFSM fsm) : base(fsm) { }

            public override void OnEnter()
            {
                _playerTrackingTimer = 0f;
                FSM.Adapter.RequestMoveTo(FSM.Adapter.GetTargetPosition());
            }

            public override void Update(float deltaTime)
            {
                _playerTrackingTimer+= deltaTime;

                if(_playerTrackingTimer >= _playerTrackingInterval)
                {
                    if (FSM.Adapter.GetDistanceToPlayer() < Context.DetectionExitRadius)
                        FSM.Adapter.RequestMoveTo(FSM.Adapter.GetTargetPosition());
                    else
                        FSM.TransitionTo(FSM.EnemyWanderState);
                }
            }
        }
    } 
    
}
