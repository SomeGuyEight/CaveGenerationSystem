
namespace SlimeGame 
{
    public class ControllerDefaultState : ControllerBaseState
    {
        public ControllerDefaultState(ControllerStateMachine context,ControllerStateFactory factory)
            : base(context,factory)
        {

        }

        protected override ControllerStates ControllerStates { get { return ControllerStates.Default; } }

        public override void GetEnabledColliders(out ColliderTypes selected,out ColliderTypes unselected) 
        {
            selected = ColliderTypes.None;
            unselected = ColliderTypes.None;
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

        }

        public override void UpdateCurrentModeName()
        {
            Ctx.SetUIText(UITextType.CurrentModeName,"_");
        }
        public override void HandleLeftMouseInputStarted()
        {

        }
        public override void HandleScrollInput()
        {

        }
    }
}
