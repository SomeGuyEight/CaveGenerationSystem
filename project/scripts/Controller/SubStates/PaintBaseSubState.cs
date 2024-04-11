using UnityEngine;

namespace SlimeGame
{
    public abstract class PaintBaseSubState : IState 
    {
        public PaintBaseSubState(ControllerStateMachine context)
        {
            _ctx = context;
            _iconObject = _ctx.GetSubStateIconObject(ControllerStates.Paint,PaintStates);
        }

        private readonly ControllerStateMachine _ctx;
        private readonly GameObject _iconObject;

        public abstract PaintStates PaintStates { get; }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        public abstract void TryPaintCellInstance(CellInstance instance);

        protected void SetIconObjectActive(bool isActive) 
        {
            if(_iconObject != null && _iconObject.activeSelf != isActive) 
            {
                _iconObject.SetActive(isActive);
            }
        }
    }
}
