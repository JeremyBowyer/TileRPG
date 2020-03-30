using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSelfState : AIBattleState
{
    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(SpellTargetSequenceState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        base.Enter();
        args.callback = InitiateTurn;
        args.targetCharacter = character;
        superUI.ShowMinorMessage(args.spell.AbilityName, 1.5f);
        character.ChangeState<SpellTargetSequenceState>(args);
        InTransition = false;
    }
}
