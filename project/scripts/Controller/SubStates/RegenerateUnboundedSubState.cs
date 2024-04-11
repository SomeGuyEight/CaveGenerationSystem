
namespace SlimeGame 
{
    public class RegenerateUnboundedSubState : RegenerateBaseSubState 
    {
        public RegenerateUnboundedSubState(ControllerStateMachine context)
          : base(context)
        {

        }

        public override RegenerateStates RegenerateStates { get { return RegenerateStates.Unbounded; } }

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
            selected = ColliderTypes.None;
            unselected = ColliderTypes.None;
        }
        public override void HandleLeftMouseInputStarted() 
        {
            if (!Ctx.IsEPressed)
            {
                return;
            }

            var tileInstance = Ctx.GetFirstSelectedInstance(typeof(TileInstance));
            if (tileInstance != null)
            {
                Ctx.GenerationManager.InstanceManager.RegenerateInstanceBounds((TileInstance)tileInstance,tileInstance.CellBound);
            }
        }
    }
}
