
namespace SlimeGame 
{
    public enum RegenerateStates 
    {
        Bounded,
        Unbounded,
    }
    public class ControllerRegenerateState : ControllerBaseState 
    {
        public ControllerRegenerateState(ControllerStateMachine context,ControllerStateFactory factory)
            : base(context,factory) 
        {
            _subStates = new RegenerateBaseSubState[]
            {
                new RegenerateBoundedSubState   (context),
                new RegenerateUnboundedSubState (context),
            };            
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }

        private int _subStateIndex = 0;
        private RegenerateBaseSubState _subState;
        private readonly RegenerateBaseSubState[] _subStates;

        protected override ControllerStates ControllerStates { get { return ControllerStates.Regenerate; } }

        public override void EnterState()
        {

        }
        public override void UpdateState()
        {

        }
        public override void SwitchSubState() 
        {
            _subState.ExitState();
            _subStateIndex = _subStateIndex + 1 == _subStates.Length ? 0 : _subStateIndex + 1;
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }
        public override void ExitState()
        {

        }

        public override void UpdateCurrentModeName()
        {
            Ctx.SetUIText(UITextType.CurrentModeName,$"Regen {_subState.RegenerateStates}");
        }
        public override void GetEnabledColliders(out ColliderTypes selected,out ColliderTypes unselected)
        {
            _subState.GetEnabledCollidersValue(out selected,out unselected);
        }
        public override void HandleLeftMouseInputStarted()
        {
            _subState.HandleLeftMouseInputStarted();
        }
        public override void HandleScrollInput() { }
    }
}