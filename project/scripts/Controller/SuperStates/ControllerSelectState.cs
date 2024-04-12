
namespace SlimeGame 
{
    public enum SelectStates 
    {
        Single,
        Multiple,
        Children,
    }
    public class ControllerSelectState : ControllerBaseState
    {
        public ControllerSelectState(ControllerStateMachine context,ControllerStateFactory factory)
            : base(context,factory)
        {
            _subStates = new SelectBaseSubState[]
            {
                new SelectSingleState   (context),
                new SelectMultipleState (context),
                new SelectChildrenState (context),
            };
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }

        private int _subStateIndex = 0;
        private SelectBaseSubState _subState;
        private readonly SelectBaseSubState[] _subStates;

        protected override ControllerStates ControllerStates { get { return ControllerStates.Select; } }

        public override void GetEnabledColliders(out ColliderTypes selected,out ColliderTypes unselected) 
        {
            _subState.GetEnabledCollidersValue(out selected,out unselected);
        }

        public override void EnterState()
        {

        }
        public override void UpdateState()
        {

        }
        public override void ExitState()
        {

        }

        public override void SwitchSubState() 
        {
            _subState.ExitState();
            _subStateIndex = _subStateIndex + 1 == _subStates.Length ? 0 : _subStateIndex + 1;
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }

        public override void UpdateCurrentModeName()
        {
            Ctx.SetUIText(UITextType.CurrentModeName,$"Select {_subState.SelectStates}");
        }
        public override void HandleLeftMouseInputStarted() 
        {
            if(!Ctx.TryRaycastForInstance(out _,out var instance) || instance == null) {
                return;
            }
            _subState.TrySelectInstance(instance);
        }
        public override void HandleScrollInput() { }
    }
}
