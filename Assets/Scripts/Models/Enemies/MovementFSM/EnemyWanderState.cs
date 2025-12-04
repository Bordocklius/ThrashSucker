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
            private Vector3 _destination;


            public EnemyWanderState(EnemyMovementFSM fsm): base(fsm) { }

            public override void OnEnter()
            {                
                PickDestination();
            }

            public override void Update(float deltaTime)
            {
                UpdateTimers(_wanderTimer);
                if(_wanderTimer > Context.WanderTimeout || FSM.Adapter.IsAtDestination())
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
                if (FSM.Adapter.TryPickRandomNavMeshPoint(Context.RandomRadius, out Vector3 destination))
                {
                    _destination = destination;
                    FSM.Adapter.RequestMoveTo(destination);
                }
            }
        }
    }
    
}
