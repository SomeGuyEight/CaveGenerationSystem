
namespace SlimeGame 
{
    public enum MoveStates 
    {
        Relocate,
        Resize,
    }
    public class ControllerMoveState : ControllerBaseState
    {
        public ControllerMoveState(ControllerStateMachine context,ControllerStateFactory factory)
        : base(context,factory)
        {
            _subStates = new MoveBaseSubState[]
            {
                new MoveRelocateState (context),
                new MoveResizeState   (context),
            };
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }

        private int _subStateIndex = 0;
        private MoveBaseSubState _subState;
        private readonly MoveBaseSubState[] _subStates;

        protected override ControllerStates ControllerStates { get { return ControllerStates.Move; } }

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
            Ctx.SetUIText(UITextType.CurrentModeName,$"{_subState.MoveStates} Instance");
        }
        public override void HandleLeftMouseInputStarted() 
        {
            _subState.HandleLeftMouseInputStarted();
        }
        public override void HandleScrollInput() 
        {
            _subState.HandleScrollInput();
        }
    }
}
