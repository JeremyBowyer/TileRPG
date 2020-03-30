using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireboltAbility : TargetSpellAbility
{

    public FireboltAbility(Character _character)
    {
        AbilityName = "Firebolt";
        AbilityDescription = "Ranged attack that deals fire damage but applies no burning buildup.";
        AbilityDamage = new Damage[] { new Damage(_character, DamageTypes.DamageType.Fire, 45) };
        ApCost = 25;
        //MpCost = 10;
        AbilityRange = 5f;
        diag = true;
        isProjectile = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("Character");
        abilityIntent = IntentTypes.Intent.Hostile;
        icon = Resources.Load<Sprite>("Sprites/Ability Icons/FireboltAbility");
    }

    public override List<Node> GetRange()
    {
        List<Node> range = controller.bc.pathfinder.FindGeometricRange(controller.tile.node, AbilityRange);
        return range;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController _target)
    {
        foreach (Damage dmg in AbilityDamage)
        {
            dmg.ability = this;
        }
        character.controller.DealDamage(AbilityDamage, _target);
    }

    public override Vector3[] GetPath(Vector3 _target)
    {
        Vector3 startingPos = controller.transform.position + Vector3.up * controller.height;
        Vector3 endPos = _target + Vector3.up * controller.height;

        int steps = 100;
        Vector3[] linePoints = new Vector3[steps];
        for (int i = 0; i < steps; i++)
        {
            float step = (float)i / steps;
            Vector3 framePos = MathCurves.Linear(startingPos, endPos, step);
            linePoints[i] = framePos;
        }

        return linePoints;
    }

    public override IEnumerator Initiate(CharController _target, Action callback)
    {
        controller.animParamController.SetTrigger("cast_2_start");
        controller.animParamController.SetBool("cast_2_loop");
        controller.transform.LookAt(new Vector3(_target.transform.position.x, controller.transform.position.y, _target.transform.position.z));
        Vector3 spawnLocation = controller.transform.position + Vector3.up * controller.height / 2 + controller.direction * 0.2f;
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

        controller.animParamController.SetBool("idle");
        controller.animParamController.SetTrigger("cast_2_end");
        GameObject.Destroy(fbPrefabClone);
        controller.transform.rotation = Quaternion.LookRotation(controller.bc.grid.GetDirection(controller.tile.node, _target.tile.node), Vector3.up);
        callback();
        yield break;
    }
}