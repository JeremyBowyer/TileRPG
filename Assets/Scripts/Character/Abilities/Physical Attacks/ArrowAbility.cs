using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAbility : AttackAbility {

    GameObject arrowPrefabClone;

    public ArrowAbility(CharController _character)
    {
        AbilityName = "Arrow attack";
        AbilityDescription = "Attack at range with an arrow.";
        AbilityDamage = new Damage[] { new Damage(DamageTypes.DamageType.Physical, 10) };
        ApCost = 75;
        AbilityRange = 3;
        diag = true;
        isProjectile = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = AbilityTypes.Intent.Hostile;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
        character.Damage(AbilityDamage);
    }

    public override List<Node> GetRange()
    {
        List<Node> range = character.bc.pathfinder.FindGeometricRange(character.tile.node, AbilityRange);
        return range;
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        character.transform.LookAt(new Vector3(_target.tile.transform.position.x, character.transform.position.y, _target.tile.transform.position.z));
        character.animParamController.SetTrigger("attack");
        arrowPrefabClone = GameObject.Instantiate(Resources.Load("Prefabs/Abilities/ArrowPrefab") as GameObject, character.transform.position, Quaternion.identity) as GameObject;
        arrowPrefabClone.gameObject.tag = "AttackAbilityGO";

        Vector3 startingPos = arrowPrefabClone.transform.position;
        Vector3 endingPos = _target.transform.position;
        float currentTime = 0f;
        float speed = 1.5f;
        float arrowHeight = Vector3.Distance(startingPos, endingPos) / 4;
        float arrowSpeed = speed + speed / arrowHeight;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * arrowSpeed));
            Vector3 framePos = MathCurves.Parabola(startingPos, endingPos, arrowHeight, currentTime);
            arrowPrefabClone.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }

        // Clean up
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        character.animParamController.SetBool("idle");
        callback();
        GameObject.Destroy(arrowPrefabClone);
        yield break;
    }
}
