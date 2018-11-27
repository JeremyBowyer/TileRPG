﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy {

    void Start()
    {
        base.Awake();
        characterName = "Goblin";
        stats.Init();
        attackAbility = new MeleeAbility(this);
        movementAbility = new WalkMovement(this, gc);
    }

    public override void InitBattle()
    {
       animParamController.SetBool("idle");
    }

    public override void SetAnimatorParameters()
    {
        animParamController = GetComponent<AnimationParameterController>();
        animParamController._bools = new List<string> { "idle", "combat_idle" };
        animParamController._triggers = new List<string> { "die", "attack", "damaged" };
    }

    public override void Die()
    {
        gc.worldEnemies.Remove(this.gameObject);
        gc.battleEnemies.Remove(this.gameObject);
        gc.characters.Remove(this.gameObject);
        animParamController.SetTrigger("die", AfterDeath);
    }

    public void AfterDeath()
    {
        Destroy(this.gameObject);
        base.Die();
    }
}
