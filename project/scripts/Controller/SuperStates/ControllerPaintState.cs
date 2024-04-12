
namespace SlimeGame 
{
    public enum PaintStates 
    {
        Wall,
        Floor,
        Ceiling,
    }
    public class ControllerPaintState : ControllerBaseState
    {
        public ControllerPaintState(ControllerStateMachine context,ControllerStateFactory factory)
            : base(context,factory)
        {
            _subStates = new PaintBaseSubState[]
            {
                new PaintWallState    (context),
                new PaintFloorState   (context),
                new PaintCeilingState (context),
            };
            _subState = _subStates[_subStateIndex];
            _subState.EnterState();
        }

        private int _subStateIndex = 0;
        private PaintBaseSubState _subState;
        private readonly PaintBaseSubState[] _subStates;

        protected override ControllerStates ControllerStates { get { return ControllerStates.Paint; } }

        public override void GetEnabledColliders(out ColliderTypes selected,out ColliderTypes unselected)
        {
            selected = ColliderTypes.SurfaceCell;
            unselected = ColliderTypes.SurfaceCell;
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
            Ctx.SetUIText(UITextType.CurrentModeName,$"Paint {_subState.PaintStates}");
        }
        public override void HandleLeftMouseInputStarted()
        {
            if(Ctx.TryRaycastForInstance(out _,out var instance)) 
            {
                if(instance is CellInstance cellInstance && _subState != null) 
                {
                    _subState.TryPaintCellInstance(cellInstance);
                }
            }         
        }
        public override void HandleScrollInput()
        {

        }
    }
}
