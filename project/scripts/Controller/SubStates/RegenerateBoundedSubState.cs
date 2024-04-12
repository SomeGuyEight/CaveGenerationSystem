
namespace SlimeGame 
{
    public class RegenerateBoundedSubState : RegenerateBaseSubState 
    {
        public RegenerateBoundedSubState(ControllerStateMachine context)
          : base(context)
        {

        }

        public override RegenerateStates RegenerateStates { get { return RegenerateStates.Bounded; } }

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
            selected = ColliderTypes.Bound;
            unselected = ColliderTypes.None;
        }
        public override void HandleLeftMouseInputStarted()
        {
            if(!Ctx.IsEPressed)
            {
                return;
            }

            var tileInstance = Ctx.GetFirstSelectedInstance(typeof(TileInstance));
            var boundInstance = Ctx.GetFirstSelectedInstance(typeof(BoundInstance));
            if (tileInstance != null && boundInstance != null)
            {
                Ctx.GenerationManager.InstanceManager.RegenerateInstanceBounds((TileInstance)tileInstance,boundInstance.CellBound);
            }
        }
    }
}
