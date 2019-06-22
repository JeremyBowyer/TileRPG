using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StateArgs {

    public CharController character;

    public Tile startingTile;
    public Tile targetTile;
    public CharController targetCharacter;
    public List<Node> path;

    public List<Node> affectedArea;
    public SpellAbility spell;
    public AttackAbility attackAbility;

    public List<StateMachine> waitingStateMachines;
    public Action callback;
}
