using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUnitsState : BattleState
{
    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(UnitPlacementState),
            typeof(SelectUnitState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        if (bc.unitsToPlace.Count == 0)
        {
            bc.protag.gameObject.transform.localScale = Vector3.one;
            inTransition = false;
            bc.ChangeState<SelectUnitState>();
            return;
        }

        GameObject unit = bc.unitsToPlace.Dequeue();
        unit.gameObject.transform.localScale = Vector3.one;
        CharController character = unit.GetComponent<CharController>();
        character.InitBattle();
        //gc.currentCharacter = character;
        StateArgs placeUnitArgs = new StateArgs()
        {
            character = character
        };
        inTransition = false;
        bc.ChangeState<UnitPlacementState>(placeUnitArgs);

    }
}
