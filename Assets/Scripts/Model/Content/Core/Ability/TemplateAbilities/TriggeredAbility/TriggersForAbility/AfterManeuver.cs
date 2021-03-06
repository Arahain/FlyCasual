﻿using Movement;
using Ship;

namespace Abilities
{
    public class AfterManeuver : TriggerForAbility
    {
        private TriggeredAbility Ability;
        public int MinSpeed { get; }
        public int MaxSpeed { get; }
        public MovementComplexity Complexity { get; }
        public bool OnlyIfFullyExecuted { get; }

        public AfterManeuver(
            ManeuverSpeed minSpeed = ManeuverSpeed.Speed0,
            ManeuverSpeed maxSpeed = ManeuverSpeed.Speed5,
            MovementComplexity complexity = MovementComplexity.None,
            bool onlyIfFullyExecuted = false
        )
        {
            ManeuverHolder minSpeedHolder = new ManeuverHolder() { Speed = minSpeed };
            MinSpeed = minSpeedHolder.SpeedIntSigned;

            ManeuverHolder maxSpeedHolder = new ManeuverHolder() { Speed = maxSpeed };
            MaxSpeed = maxSpeedHolder.SpeedIntSigned;
            Complexity = complexity;
            OnlyIfFullyExecuted = onlyIfFullyExecuted;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnMovementFinish += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnMovementFinish -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            if (ship.AssignedManeuver.Speed >= MinSpeed
                && ship.AssignedManeuver.Speed <= MaxSpeed
                && (Complexity == MovementComplexity.None || ship.AssignedManeuver.ColorComplexity == Complexity)
                && (OnlyIfFullyExecuted == false || (OnlyIfFullyExecuted && ship.CheckSuccessOfManeuver()))
            )
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnMovementFinish,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }
    }
}
