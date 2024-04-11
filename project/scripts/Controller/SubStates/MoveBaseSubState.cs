using UnityEngine;
using System;

namespace SlimeGame 
{
    public abstract class MoveBaseSubState : IState
    {
        public MoveBaseSubState(ControllerStateMachine context) 
        {
            _ctx = context;
            _iconObject = _ctx.GetSubStateIconObject(ControllerStates.Move,MoveStates);
        }

        private readonly ControllerStateMachine _ctx;
        private readonly GameObject _iconObject;

        public abstract MoveStates MoveStates { get; }
        protected ControllerStateMachine Ctx { get { return _ctx; } }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void GetEnabledCollidersValue(out ColliderTypes selected,out ColliderTypes unselected);

        public abstract void HandleLeftMouseInputStarted();
        public abstract void HandleScrollInput();

        protected void SetIconObjectActive(bool isActive) 
        {
            if(_iconObject != null && _iconObject.activeSelf != isActive) 
            {
                _iconObject.SetActive(isActive);
            }
        }
    }
}
