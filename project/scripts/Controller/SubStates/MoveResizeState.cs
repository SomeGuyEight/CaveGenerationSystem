using System;
using System.Collections.Generic;

namespace SlimeGame 
{
    public class MoveResizeState : MoveBaseSubState
    {
        public MoveResizeState(ControllerStateMachine context)
            : base(context)
        {

        }

        private static readonly Dictionary<Type,(ColliderTypes selected,ColliderTypes unselected)> _typeToEnabledColliders = new ()
        {
            { typeof(TileInstance)       ,(ColliderTypes.Tile             ,ColliderTypes.Tile             ) },
            { typeof(BoundInstance)      ,(ColliderTypes.Bound            ,ColliderTypes.Bound            ) },
            { typeof(CellInstance)       ,(ColliderTypes.AllCellValues    ,ColliderTypes.None             ) },
            { typeof(BaseSocketInstance) ,(ColliderTypes.AllSocketValues  ,ColliderTypes.None             ) },
        };

        public override MoveStates MoveStates { get { return MoveStates.Resize; } }

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
            if (instance != null && _typeToEnabledColliders.TryGetValue(instance.GetType(),out var tuple))
            {
                selected = tuple.selected;
                unselected = tuple.unselected;
                return;
            }
            selected = ColliderTypes.None;
            unselected = ColliderTypes.None;
        }
        public override void HandleLeftMouseInputStarted()
        {
            /// TODO: Add ability to move hit face to position
            /// -> similar to <see cref="MoveRelocateState.HandleLeftMouseInputStarted"/>
        }
        public override void HandleScrollInput()
        {
            if (!Ctx.TryRaycastForInstance(out var hit,out var instance))
            {
                return;
            }
            instance.TryResize(Ctx.ScrollInput,hit);
        }

    }
}
