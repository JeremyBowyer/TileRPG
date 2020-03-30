using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKnight : Enemy
{

    public SkeletonKnight() : base()
    {
        avatar = Resources.Load<Sprite>("Sprites/Avatars/SkeletonKnight");

        abilities = new List<SpellAbility>();
        abilities.Add(new StabAbility(this));

        attackAbility = new SwordAttackAbility(this);
        movementAbility = new WalkMovement(this);
        movementAbility.Speed = 2.0f;

        audioProfile = Resources.Load<CharacterAudioProfile>("Sounds/Audio Profiles/Character Profiles/Enemies/SkeletonKnightAudioProfile");
    }

    public override void Init()
    {
        stats.maxAP = 75;
        stats.agility = 15;
        stats.Init();
        //stats.curHP = 1;

        resists.Init(new Dictionary<DamageTypes.DamageType, float>()
        {
            {DamageTypes.DamageType.Fire, 0.5f }
        });

        damageBonuses.Init();
        buildUps.Init();

        isInitialized = true;

        cName = "Skeleton Knight";
        cClass = "Skeleton Knight";
    }
}
