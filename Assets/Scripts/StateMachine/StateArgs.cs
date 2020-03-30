using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StateArgs {

    public LevelPortal portal;

    public CharController character;

    public Tile startingTile;
    public Tile targetTile;
    public CharController targetCharacter;
    public List<Node> path;

    public List<Node> affectedArea;
    public List<CharController> affectedCharacters;
    public SpellAbility spell;
    public AttackAbility attackAbility;

    public KeepRoom room;

    public bool finishInterruptedState;
    public List<StateMachine> waitingStateMachines;
    public Action callback;

    public ItemChest chest;
    public MerchantController merchant;

    public Damage damage;

    public Item item;
}
