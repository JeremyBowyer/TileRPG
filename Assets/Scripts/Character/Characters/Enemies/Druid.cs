using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid : Enemy
{

    void Start()
    {
        base.Awake();
        characterName = "Druid";
        stats.Init();
        attackAbility = new ArrowAbility(this);
        movementAbility = new WalkMovement(this, gc);
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
