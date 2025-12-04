using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThrashSucker.FSM;

namespace ThrashSucker.Models.Enemies
{
    public partial class Enemybase
    {
        public class EnemyMovementFSM: FiniteStateMachine
        {
            // Context
            public Enemybase Context { get; private set; }

            // Interface to request stuff
            public IEnemyMovement Adapter { get; private set; }

            // States
            public EnemyWanderState EnemyWanderState { get; protected set; }
            public EnemyChaseState EnemyChaseState { get; protected set; }

            // Current state
            public new EnemyMovementBaseState CurrentState { get => base.CurrentState as EnemyMovementBaseState; }

            public EnemyMovementFSM(Enemybase context, IEnemyMovement adapter)
            {
                Context = context;
                Adapter = adapter;

                EnemyWanderState = new EnemyWanderState(this);
                EnemyChaseState = new EnemyChaseState(this);
                TransitionTo(EnemyWanderState);
            }
        }
    }    
}
