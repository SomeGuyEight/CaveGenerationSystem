using System.Collections.Generic;

namespace SlimeGame 
{
    public class ControllerStateFactory 
    {
        public ControllerStateFactory(ControllerStateMachine context)
        {
            _statesToBaseState = new ()
            {
                { ControllerStates.Default    , new ControllerDefaultState    (context,this) },
                { ControllerStates.Place      , new ControllerPlaceState      (context,this) },
                { ControllerStates.Mine       , new ControllerMineState       (context,this) },
                { ControllerStates.Paint      , new ControllerPaintState      (context,this) },
                { ControllerStates.Move       , new ControllerMoveState       (context,this) },
                { ControllerStates.Select     , new ControllerSelectState     (context,this) },
                { ControllerStates.Regenerate , new ControllerRegenerateState (context,this) },
            };
        }

        private readonly Dictionary<ControllerStates, ControllerBaseState> _statesToBaseState = new ();

        public ControllerBaseState GetSuperState(ControllerStates states)
        {
            return _statesToBaseState[states];
        }
    }
}