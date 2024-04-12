using UnityEngine;

namespace SlimeGame 
{
    public abstract class ControllerBaseState : IState
    {
        public ControllerBaseState(ControllerStateMachine context,ControllerStateFactory factory)
        {
            _ctx = context;
            _stateFactory = factory;
            _activeStateObject = _ctx.GetSelectedSlotObject(ControllerStates);
        }

        private readonly ControllerStateMachine _ctx;
        private readonly ControllerStateFactory _stateFactory;
        private readonly GameObject _activeStateObject;

        protected abstract ControllerStates ControllerStates { get; }
        protected ControllerStateMachine Ctx { get { return _ctx; } }
        protected ControllerStateFactory StateFactory { get { return _stateFactory; } }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void SwitchSubState();
        public abstract void UpdateCurrentModeName();
        public abstract void HandleLeftMouseInputStarted();
        public abstract void HandleScrollInput();
        public abstract void GetEnabledColliders(out ColliderTypes selected,out ColliderTypes unselected);

        public void HandleKeysInput(ControllerStates states) 
        {
            if(states == ControllerStates) 
            {
                SwitchSubState();
                UpdateCurrentModeName();
            } 
            else 
            {
                var newState = StateFactory.GetSuperState(states);
                SwitchSuperState(newState);
                newState.UpdateCurrentModeName();
            }

            _ctx.GenerationManager.InstanceManager.UpdateCollidersEnabled();
        }
        private void SwitchSuperState(ControllerBaseState newState) 
        {
            ExitState();
            SetActiveObjectActive(false);
            _ctx.SetCurrentState(newState);
            newState.EnterState();
            newState.SetActiveObjectActive(true);
            _ctx.SetUIObjectsActive(UIObjectType.ModeDisplay,newState.GetType() != typeof(ControllerDefaultState));
        }
        private void SetActiveObjectActive(bool isActive)
        {
            if (_activeStateObject != null && _activeStateObject.activeSelf != isActive)
            {
                _activeStateObject.SetActive(isActive);
            }
        }
    }
}
