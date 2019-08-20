using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireboltAbility : TargetSpellAbility
{

    public FireboltAbility(CharController _character)
    {
        AbilityName = "Firebolt";
        AbilityDescription = "Attack at range with a firebolt.";
        AbilityDamage = new Damage[] { new Damage(DamageTypes.DamageType.Fire, 30, MaladyTypes.MaladyType.Burn, 20) };
        ApCost = 25;
        MpCost = 10;
        AbilityRange = 5f;
        diag = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = AbilityTypes.Intent.Hostile;
    }

    public override List<Node> GetRange()
    {
        List<Node> range = character.bc.pathfinder.FindGeometricRange(character.tile.node, AbilityRange);
        return range;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
        character.Damage(AbilityDamage);
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        character.animParamController.SetTrigger("cast_start");
        character.animParamController.SetBool("cast_loop");
        character.transform.LookAt(new Vector3(_target.transform.position.x, character.transform.position.y, _target.transform.position.z));
        Vector3 spawnLocation = character.transform.position + Vector3.up * character.height / 2 + character.direction * 0.2f;
        GameObject fbPrefabClone = GameObject.Instantiate(Resources.Load("Prefabs/Abilities/FireboltPrefab") as GameObject, spawnLocation, Quaternion.identity) as GameObject;
        fbPrefabClone.gameObject.tag = "SpellTargetGO";
        Vector3 startingPos = fbPrefabClone.transform.position;
        Vector3 endingPos = _target.transform.position + Vector3.up * _target.height / 2;
        float currentTime = 0f;
        float speed = 2f;

        Vector3 deltaPos = endingPos - startingPos;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * speed));
            float frameValue = (1f - 0f) * EasingEquations.Linear(0.0f, 1.0f, currentTime) + 0f;
            fbPrefabClone.transform.position = startingPos + deltaPos * frameValue;
            yield return new WaitForEndOfFrame();
        }

        character.animParamController.SetBool("idle");
        character.animParamController.SetTrigger("cast_end");
        GameObject.Destroy(fbPrefabClone);
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        callback();
        yield break;
    }
}