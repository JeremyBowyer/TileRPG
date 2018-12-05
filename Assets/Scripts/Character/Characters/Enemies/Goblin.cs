using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy {

    void Start()
    {
        base.Awake();
        //characterName = "Goblin";
        stats.Init();
        attackAbility = new MeleeAbility(this);
        movementAbility = new JumpMovement(this, gc);
    }

    public override void InitBattle()
    {
       animParamController.SetBool("idle");
    }

    public override void SetAnimatorParameters()
    {
        animParamController = GetComponent<AnimationParameterController>();
        animParamController._bools = new List<string> { "idle", "falling", "running" };
        animParamController._triggers = new List<string> { "jump", "die", "attack" };
    }

    public override void Die()
    {
        base.Die();
        gc.worldEnemies.Remove(this.gameObject);
        gc.battleEnemies.Remove(this.gameObject);
    }
}
