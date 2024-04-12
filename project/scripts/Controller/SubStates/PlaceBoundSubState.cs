
namespace SlimeGame 
{
    public class PlaceBoundSubState : PlaceBaseSubState 
    {
        public PlaceBoundSubState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override PlaceStates PlaceStates { get { return PlaceStates.Bound; } }

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
            if(Ctx.IsEPressed)
            {
                Ctx.GenerationManager.InstanceManager.InitializeNewBound(Ctx.transform.position);
                return;
            }
            if (Ctx.TryRaycastForInstance(out var hit,out _))
            {
                Ctx.GenerationManager.InstanceManager.InitializeNewBound(hit.point);
            }
        }
    }
}