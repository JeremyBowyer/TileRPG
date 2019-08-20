using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonLesser : Enemy
{
    public override void Init()
    {
        stats.maxHP = 10;
        stats.maxAP = 115;
        stats.agility = 25;
        stats.Init();
        resists.Init();
        buildUps.Init();

        controller.SetResistance(DamageTypes.DamageType.Fire, 0.5f);

        experience = 0;
        level = 1;

        isInitialized = true;

        cName = "Boner";
        cClass = "Lesser Skeleton";

        spells = new List<SpellAbility>();
        spells.Add(new CudgelAbility(controller));

        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);
        movementAbility.Speed = 1.5f;
    }
}
