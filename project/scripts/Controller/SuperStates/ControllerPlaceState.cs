
namespace SlimeGame
{
    public enum PlaceStates 
    {
        Bound,
        Tile,
        AirCell,
        VoidCell,
    }
    public class ControllerPlaceState : ControllerBaseState 
    {
        public ControllerPlaceState(ControllerStateMachine context,ControllerStateFactory factory)
            : base(context,factory)
        {
            _subStates = new PlaceBaseSubState[]
            {
                new PlaceAirSubState (context),
                /// new PlaceVoidSubState    (context),
                new PlaceBoundSubState   (context),
                new PlaceTileSubState    (context),
            };
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }

        private int _subStateIndex = 0;
        private PlaceBaseSubState _subState;
        private readonly PlaceBaseSubState[] _subStates;

        protected override ControllerStates ControllerStates { get { return ControllerStates.Place; } }

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
            Ctx.SetUIText(UITextType.CurrentModeName,$"Place {_subState.PlaceStates}");
        }
        public override void HandleLeftMouseInputStarted() 
        {
            _subState.HandleLeftMouseInputStarted();
        }
        public override void HandleScrollInput()
        {

        }
    }
}
