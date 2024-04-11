
namespace SlimeGame
{
    public class SelectSingleState : SelectBaseSubState
    {
        public SelectSingleState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override SelectStates SelectStates { get { return SelectStates.Single; } }

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
            selected = unselected = ColliderTypes.Bound | ColliderTypes.Tile | ColliderTypes.TileSet | ColliderTypes.AllSocketValues;
        }

        public override void TrySelectInstance(BaseGenerationInstance instance)
        {
            Ctx.GenerationManager.InstanceManager.TrySelectSingleInstance(instance);
        }

    }
}
