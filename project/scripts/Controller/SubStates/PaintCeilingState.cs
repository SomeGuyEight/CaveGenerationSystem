
namespace SlimeGame
{
    public class PaintCeilingState : PaintBaseSubState 
    {
        public PaintCeilingState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override PaintStates PaintStates { get { return PaintStates.Ceiling; } }

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
            instance.EditSurfaceTypeAndMesh(CellTypes.CeilingSurface);
        }
    }
}
