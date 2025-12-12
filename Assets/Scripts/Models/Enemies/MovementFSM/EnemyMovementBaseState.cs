using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrashSucker.FSM;
using UnityEngine;

namespace TrashSucker.Models.Enemies
{
    public partial class Enemybase
    {
        public class EnemyMovementBaseState : IState
        {
            public float PlayerTrackingTimer;
            public float PlayerTrackingDelay = 0.15f;

            public Vector3 LastTarget;

            public EnemyMovementFSM FSM { get; private set; }

            public Enemybase Context { get => FSM.Context; }
            
            public EnemyMovementBaseState(EnemyMovementFSM fsm)
            {
                FSM = fsm;
            }

            public virtual void OnEnter() { }
            public virtual void OnExit() { }
            public virtual void Update(float deltaTime) { }
            public virtual void FixedUpdate(float fixedDeltaTime) { }

        }
    }    
}
