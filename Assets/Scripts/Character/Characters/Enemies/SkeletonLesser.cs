using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonLesser : Enemy
{

    public SkeletonLesser() : base()
    {
        avatar = Resources.Load<Sprite>("Sprites/Avatars/LesserSkeleton");

        abilities = new List<SpellAbility>();
        abilities.Add(new CudgelAbility(this));

        attackAbility = new PunchAttackAbility(this);
        movementAbility = new WalkMovement(this);
        movementAbility.Speed = 1.5f;

        audioProfile = Resources.Load<CharacterAudioProfile>("Sounds/Audio Profiles/Character Profiles/Enemies/LesserSkeletonAudioProfile");
    }

    public override void Init()
    {
        stats.maxAP = 115;
        stats.agility = 25;
        stats.Init();
        //stats.curHP = 1;

        resists.Init(new Dictionary<DamageTypes.DamageType, float>()
        {
            {DamageTypes.DamageType.Fire, 0.5f }
        });
        damageBonuses.Init();
        buildUps.Init();

        isInitialized = true;

        cName = "Lesser Skeleton";
        cClass = "Lesser Skeleton";
    }
}
