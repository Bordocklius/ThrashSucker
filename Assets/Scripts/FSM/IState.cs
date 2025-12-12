using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashSucker.FSM
{
    public interface IState
    {
        public void OnEnter();
        public void OnExit();
        public void Update(float deltaTime);
        public void FixedUpdate(float fixedDeltaTime);
    }
}
