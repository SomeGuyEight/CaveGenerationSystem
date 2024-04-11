using UnityEngine;

namespace SlimeGame 
{
    public abstract class RegenerateBaseSubState : IState 
    {
        public RegenerateBaseSubState(ControllerStateMachine context) 
        {
            _ctx = context;
            _iconObject = _ctx.GetSubStateIconObject(ControllerStates.Regenerate,RegenerateStates);
        }

        private readonly ControllerStateMachine _ctx;
        private readonly GameObject _iconObject;

        public abstract RegenerateStates RegenerateStates { get; }
        protected ControllerStateMachine Ctx { get { return _ctx; } }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        public abstract void GetEnabledCollidersValue(out ColliderTypes selected,out ColliderTypes unselected);
        public abstract void HandleLeftMouseInputStarted();
        protected void SetIconObjectActive(bool isActive)
        {
            if(_iconObject != null && _iconObject.activeSelf != isActive)
            {
                _iconObject.SetActive(isActive);
            }
        }
    }
}
