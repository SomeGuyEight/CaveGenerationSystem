using System;
using System.Collections.Generic;

namespace SlimeGame 
{
    public class SelectChildrenState : SelectBaseSubState
    {
        public SelectChildrenState(ControllerStateMachine context)
            : base(context)
        {

        }

        private static readonly Dictionary<Type,(ColliderTypes selected,ColliderTypes unselected)> _typeToEnabledColliders = new ()
        {
            { typeof(TileInstance)       ,(ColliderTypes.AllCellValues | ColliderTypes.AllSocketValues ,ColliderTypes.None ) },
            { typeof(CellInstance)       ,(ColliderTypes.AllCellValues    ,ColliderTypes.None ) },
            { typeof(BaseSocketInstance) ,(ColliderTypes.AllSocketValues  ,ColliderTypes.None ) },
        };

        public override SelectStates SelectStates { get { return SelectStates.Children; } }

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
            if (instance == null)
            {
                selected = unselected = ColliderTypes.Tile | ColliderTypes.TileSet;
                return;
            }
            if (_typeToEnabledColliders.TryGetValue(instance.GetType(),out var tuple))
            {
                selected = tuple.selected;
                unselected = tuple.unselected;
                return;
            }
            selected = unselected = ColliderTypes.None;
        }

        public override void TrySelectInstance(BaseGenerationInstance instance)
        {
            Ctx.GenerationManager.InstanceManager.SelectChildInstance(instance);
        }
    }
}
