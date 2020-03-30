using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Executioner : PartyMember
{

    public Executioner() : base()
    {
        avatar = Resources.Load<Sprite>("Sprites/Avatars/Executioner");
        attackAbility = new SwordAttackAbility(this);
        movementAbility = new WalkMovement(this);
        movementAbility.speed = 2f;
        movementAbility.costModifier = 3f;

        abilities = new List<SpellAbility>();
        abilities.Add(new ExecuteAbility(this));
        abilities.Add(new PommelAbility(this));

        audioProfile = Resources.Load<CharacterAudioProfile>("Sounds/Audio Profiles/Character Profiles/Players/ExecutionerAudioProfile");
    }

    public override void Init()
    {
        stats.maxHP = 200;
        stats.maxAP = 100;
        stats.agility = 200;
        stats.Init();
        resists.Init();
        damageBonuses.Init();
        buildUps.Init();

        cName = "Executioner";
        cClass = "Executioner";
        model = "Executioner";

        isInitialized = true;
    }
}
