
namespace SlimeGame 
{
    public class PaintWallState : PaintBaseSubState
    {
        public PaintWallState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override PaintStates PaintStates { get { return PaintStates.Wall; } }

        public override void EnterState()
        {
            SetIconObjectActive(true);
        }
        public override void UpdateState()
        {

        }
        public override void ExitState() 
        {
            SetIconObjectActive(false);
        }

        public override void TryPaintCellInstance(CellInstance instance) 
        {
            instance.EditSurfaceTypeAndMesh(CellTypes.WallSurface);
        }
    }
}
