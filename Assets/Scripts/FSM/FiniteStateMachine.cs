using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashSucker.FSM
{
    public abstract class FiniteStateMachine
    {

        public event EventHandler CurrentStateChanged;
        private IState _currentState;
        public IState CurrentState
        {
            get { return _currentState; }
            set
            {
                if (_currentState == value) return;
                _currentState = value;
                OnStateChanged();
            }
        }

        protected virtual void OnStateChanged()
        {
            CurrentStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Update(float deltaTime)
        {
            CurrentState.Update(deltaTime);
        }

        public virtual void FixedUpdate(float fixedDeltaTime)
        {
            CurrentState.FixedUpdate(fixedDeltaTime);
        }

        public virtual void TransitionTo(IState newState)
        {
            if (newState == null) return;
            if (newState == CurrentState) return;
            if (CurrentState != null) CurrentState.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter();
        }
    }
}
