
namespace SlimeGame
{
    public class PaintFloorState : PaintBaseSubState
    {
        public PaintFloorState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override PaintStates PaintStates { get { return PaintStates.Floor; } }

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
            instance.EditSurfaceTypeAndMesh(CellTypes.FloorSurface);
        }
    }
}
