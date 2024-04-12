
using System;

namespace SlimeGame
{
    public class MoveRelocateState : MoveBaseSubState 
    {
        public MoveRelocateState(ControllerStateMachine context)
            : base(context)
        {

        }

        public override MoveStates MoveStates { get { return MoveStates.Relocate; } }

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
            var instance = Ctx.GetLastSelectedInstance();
            switch (instance)
            {
                case BoundInstance:
                    selected = ColliderTypes.Bound;
                    unselected = ColliderTypes.None;
                    return;
                case TileInstance:
                    selected = ColliderTypes.Tile;
                    unselected = ColliderTypes.None;
                    return;
                case CellInstance:
                    selected = ColliderTypes.AllCellValues;
                    unselected = ColliderTypes.None;
                    return;
                case BaseSocketInstance:
                    selected = ColliderTypes.AllSocketValues;
                    unselected = ColliderTypes.None;
                    return;
                default:
                    selected = unselected = ColliderTypes.None;
                    return;
            }            
        }
        public override void HandleLeftMouseInputStarted()
        {
   

            if (!Ctx.TryRaycastForInstance(out var hit,out var instance))
            {
                if (Ctx.IsEPressed && Ctx.GenerationManager.InstanceManager.SelectedInstance != null)
                {
                    var types = new Type[] 
                    { 
                        typeof(TileInstance),
                        typeof(BoundInstance)
                    };
                    Ctx.GenerationManager.InstanceManager.TryGetFirstSelected(types, out instance);
                    instance.TryRelocate(Ctx.GetCellFromPosition(Ctx.transform.position));
                }
                return;
            }

            if (Ctx.IsEPressed)
            {
                instance.TryRelocate(Ctx.GetCellFromPosition(Ctx.transform.position));
            }
            else
            {
                instance.TryRelocate(Ctx.GetCellFromPosition(hit.point));
            }
        }
        public override void HandleScrollInput()
        {
            if (!Ctx.TryRaycastForInstance(out _,out var instance) || instance == null)
            {
                return;
            }

            instance.TryTranslate(Ctx.GetCellOffsetFromScrollInput());
        }
    }
}
