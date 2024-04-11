using UnityEngine;

namespace SlimeGame
{
    public class PlaceVoidSubState : PlaceBaseSubState
    {
        public PlaceVoidSubState(ControllerStateMachine context)
            : base(context) 
        {

        }

        public override PlaceStates PlaceStates { get { return PlaceStates.VoidCell; } }

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
            Debug.Log("Placing Void Cells not implemented yet => returning void");
            //if (!Ctx.TryRaycastForInstance(out var hit,out var instance))
            //{
            //    return;
            //}
            //if (instance is CellInstance cellInstance && cellInstance.ParentTile != null)
            //{
            //    Ctx.GenerationManager.InstanceGenerator.GenerateNewVoidCell(cellInstance.ParentTile,hit.point);
            //}
        }
    }
}