using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThrashSucker.Models.Enemies
{
    public partial class Enemybase
    {
        public class EnemyWanderState: EnemyMovementBaseState
        {
            private float _wanderTimer;
            private float _stuckTimer;

            public EnemyWanderState(EnemyMovementFSM fsm): base(fsm) { }

            public override void OnEnter()
            {                
                PickDestination();
            }

            public override void Update(float deltaTime)
            {
                UpdateTimers(deltaTime);

                if(FSM.Adapter.GetDistanceToPlayer() <= Context.DetectionEnterRadius)
                {
                    FSM.TransitionTo(FSM.EnemyChaseState);
                    return;
                }

                if(FSM.Adapter.IsAtDestination() || _wanderTimer >= Context.WanderTimeout)
                {
                    PickDestination();
                }
            }

            private void UpdateTimers(float deltaTime)
            {
                _wanderTimer += deltaTime;
            }

            private void PickDestination()
            {
                _wanderTimer = 0f;
                Vector3 destination = Vector3.zero;
                Vector3 origin = FSM.Adapter.GetSelfPosition();

                int attempts = 0;
                while(attempts < Context.MaxPickAttempts)
                {
                    attempts++;

                    if (!FSM.Adapter.TryPickRandomNavMeshPoint(Context.RandomRadius, out destination))
                        continue;

                    float distance = Vector3.Distance(origin, destination);
                    if (distance >= Context.MinTargetDistance)
                    {
                        FSM.Adapter.RequestMoveTo(destination);
                        LastTarget = destination;
                        return;
                    }
                }           
            }
        }
    }
    
}
