using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StateArgs {

    public Character character;

    public Tile startingTile;
    public Tile targetTile;
    public Character targetCharacter;
    public List<Node> path;

    public List<Node> splashZone;
    public SpellAbility spell;
    public AttackAbility attackAbility;

    public List<StateMachine> waitingStateMachines;
    public Action callback;
}
