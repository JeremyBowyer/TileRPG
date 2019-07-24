﻿using System;
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
        aiAction.text = "Healing Self...";
        character.ChangeState<SpellTargetSequenceState>(args);
        InTransition = false;
    }
}
