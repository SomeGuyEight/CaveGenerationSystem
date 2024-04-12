
namespace SlimeGame
{
    public class PlaceTileSubState : PlaceBaseSubState
    {
        public PlaceTileSubState(ControllerStateMachine context)
            : base(context) 
        {

        }

        public override PlaceStates PlaceStates { get { return PlaceStates.Tile; } }

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
            selected = ColliderTypes.Bound | ColliderTypes.Tile;
            unselected = ColliderTypes.Bound | ColliderTypes.Tile;
        }
        public override void HandleLeftMouseInputStarted()
        {
            if (Ctx.IsEPressed)
            {
                Ctx.GenerationManager.InstanceManager.InitializeNewTile(Ctx.transform.position);
                return;
            }
            if (Ctx.TryRaycastForInstance(out var hit,out _))
            {
                Ctx.GenerationManager.InstanceManager.InitializeNewTile(hit.point);
            }
        }
    }
}