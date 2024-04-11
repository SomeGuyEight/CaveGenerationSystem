
namespace SlimeGame
{
    public class PlaceAirSubState : PlaceBaseSubState
    {
        public PlaceAirSubState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override PlaceStates PlaceStates { get { return PlaceStates.AirCell; } }

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

        public override void GetEnabledCollidersValue(out ColliderTypes selected,out ColliderTypes unselected)
        {
            selected = ColliderTypes.SurfaceCell;
            unselected = ColliderTypes.None;
        }
        public override void HandleLeftMouseInputStarted()
        {
            if (!Ctx.TryRaycastForInstance(out var hit,out var instance))
            {
                return;
            }
            if (instance is CellInstance cellInstance && cellInstance.ParentTile != null)
            {
                Ctx.GenerationManager.InstanceGenerator.GenerateNewAirCell(cellInstance.ParentTile,hit.point);
            }
        }
    }
}