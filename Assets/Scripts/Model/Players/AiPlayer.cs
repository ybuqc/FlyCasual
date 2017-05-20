﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class AiPlayer : GenericPlayer
    {

        public AiPlayer(): base() {
            Type = PlayerType.Ai;
            Name = "AI";
        }

        public override void SetupShip()
        {
            Phases.Next();
        }

        public override void AssignManeuver()
        {
            foreach (var shipHolder in Ships)
            {
                shipHolder.Value.AssignedManeuver = Game.Movement.ManeuverFromString("2.F.S");
            }
            Phases.Next();
        }

        public override void PerformManeuver()
        {
            foreach (var shipHolder in Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    Selection.ThisShip = shipHolder.Value;
                    Game.Movement.PerformStoredManeuver();
                    break;
                }
            }
        }

        public override void PerformAction()
        {
            if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                ActionsList.GenericAction action = Selection.ThisShip.GetAvailableActionsList()[0];
                action.ActionTake();
                Selection.ThisShip.AddAlreadyExecutedAction(action);
                Phases.Next();
            }
        }

        public override void PerformFreeAction()
        {
            if (Selection.ThisShip.GetAvailableFreeActionsList().Count > 0)
            {
                ActionsList.GenericAction action = Selection.ThisShip.GetAvailableFreeActionsList()[0];
                action.ActionTake();
                Selection.ThisShip.AddAlreadyExecutedAction(action);
            }
        }

        public override void PerformAttack()
        {
            foreach (var shipHolder in Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    Selection.ThisShip = shipHolder.Value;
                    break;
                }
            }

            bool attackPerformed = false;
            foreach (var shipHolder in Roster.GetPlayer(Roster.AnotherPlayer(PlayerNo)).Ships)
            {
                Selection.AnotherShip = Roster.AllShips[shipHolder.Key];
                if (Actions.CheckShot())
                {
                    attackPerformed = true;
                    Actions.PerformAttack();
                }
            }
            if (!attackPerformed) Phases.Next();

        }

        public override void UseDiceModifications()
        {
            if (Selection.ThisShip.GetAvailableActionsList().Count > 0)
            {
                Selection.ThisShip.GetAvailableActionsList()[0].ActionEffect();
                Game.StartCoroutine(Wait());
            }
            else
            {
                Combat.ConfirmDiceResults();
            }
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3);
            Combat.ConfirmDiceResults();
        }

    }

}