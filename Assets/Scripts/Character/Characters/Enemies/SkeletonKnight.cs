using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKnight : Enemy
{
    public override void Init()
    {
        stats.maxHP = 10;
        stats.maxAP = 75;
        stats.agility = 15;
        stats.Init();
        resists.Init();
        buildUps.Init();

        resists.SetResistance(DamageTypes.DamageType.Fire, 0.5f);

        experience = 0;
        level = 1;

        isInitialized = true;

        cName = "Fatso";
        cClass = "Skeleton Knight";

        spells = new List<SpellAbility>();
        spells.Add(new CorruptedBladeAbility(controller));

        attackAbility = new MeleeAbility(controller);
        movementAbility = new WalkMovement(controller);
        movementAbility.Speed = 2.5f;
    }
}
