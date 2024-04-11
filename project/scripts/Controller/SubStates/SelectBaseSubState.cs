using UnityEngine;

namespace SlimeGame
{
    public abstract class SelectBaseSubState : IState 
    {
        public SelectBaseSubState(ControllerStateMachine context) 
        {
            _ctx = context;
            _iconObject = _ctx.GetSubStateIconObject(ControllerStates.Select,SelectStates);
        }

        private readonly ControllerStateMachine _ctx;
        private readonly GameObject _iconObject;

        public abstract SelectStates SelectStates { get; }
        protected ControllerStateMachine Ctx { get { return _ctx; } }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void GetEnabledCollidersValue(out ColliderTypes selected,out ColliderTypes unselected);
        public abstract void TrySelectInstance(BaseGenerationInstance instance);

        protected void SetIconObjectActive(bool isActive)
        {
            if (_iconObject != null && _iconObject.activeSelf != isActive)
            {
                _iconObject.SetActive(isActive);
            }
        }
    }
}
