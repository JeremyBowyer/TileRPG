using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaBallAbility : EnvironmentSplashSpellAbility
{
    GameObject mbPrefabClone;
    List<Node> splashZone;

    public MagmaBallAbility(Character _character)
    {
        AbilityName = "Magma Ball";
        AbilityDescription = "Attack at range with a magma ball.";
        AbilityDamage = new Damage[] { new Damage(_character as IDamageSource, DamageTypes.DamageType.Fire, 5, MaladyTypes.MaladyType.Burn, 100) };
        ApCost = 1;
        MpCost = 1;
        AbilityRange = 4;
        diag = true;
        ignoreOccupant = true;
        isProjectile = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("GridClick");
        abilityIntent = IntentTypes.Intent.Hostile;
        icon = Resources.Load<Sprite>("Sprites/Ability Icons/MagmaBallAbility");
    }

    public override List<Node> GetRange()
    {
        List<Node> range = controller.bc.pathfinder.FindGeometricRange(controller.tile.node, AbilityRange);
        return range;
    }

    public override List<Node> GetSplashZone(Tile _target)
    {
        List<Node> range = controller.bc.pathfinder.FindRange(_target.node, 1, false, true, false, true, false);
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
        bc.damageQueue.Enqueue(new KeyValuePair<CharController, Damage[]>(_target, AbilityDamage));
        //_target.Damage(AbilityDamage);
    }

    public override void ApplyTileEffect(Tile _tile, Vector3 _sourceDirection, Grid _grid)
    {
        if(_tile.Occupant != null)
        {
            CharController character = _tile.Occupant.GetComponent<CharController>();
            if (character != null)
                ApplyCharacterEffect(character);
        }

        TileEffect oldEffect = _tile.GetComponent<TileEffect>();

        if(oldEffect is BurnTileEffect)
        {
            oldEffect.RefreshEffect();
            return;
        }
        else
        {
            if (oldEffect != null)
                oldEffect.RemoveEffect();
            TileEffect newEffect = _tile.gameObject.AddComponent<BurnTileEffect>();
            newEffect.Init(_tile, _sourceDirection, _grid, character);
        }

    }

    public override Vector3[] GetPath(Vector3 _target)
    {
        Vector3 startingPos = controller.transform.position + Vector3.up * controller.height;
        Vector3 deltaPos = _target - startingPos;
        float fbHeight = Vector3.Distance(startingPos, _target) / 2;
        Vector3 cp1 = startingPos + deltaPos * 0.5f + new Vector3(0, fbHeight, 0);

        int steps = 100;
        Vector3[] linePoints = new Vector3[steps];
        for (int i = 0; i < steps; i++)
        {
            float step = (float)i / steps;
            Vector3 framePos = MathCurves.Bezier(startingPos, _target, cp1, step);
            linePoints[i] = framePos;
        }

        return linePoints;
    }

    public override IEnumerator Initiate(Tile tile, List<Node> affectedArea, Action callback)
    {
        controller.animParamController.SetTrigger("cast_start");
        controller.animParamController.SetBool("cast_loop");
        controller.transform.LookAt(new Vector3(tile.transform.position.x, controller.transform.position.y, tile.transform.position.z));
        Vector3 spawnLocation = new Vector3(controller.transform.position.x, controller.transform.position.y + controller.height, controller.transform.position.z);
        mbPrefabClone = GameObject.Instantiate(AssetController.GetAsset("magma_ball"), spawnLocation, Quaternion.identity) as GameObject;
        mbPrefabClone.gameObject.tag = "SpellEnvironmentGO";
        //inProgress = true;
        Vector3 startingPos = mbPrefabClone.transform.position;
        Vector3 endingPos = tile.WorldPosition;
        float currentTime = 0f;
        float speed = 0.5f;
        float fbHeight = Vector3.Distance(startingPos, endingPos);
        float fbSpeed = speed + speed / fbHeight;

        Vector3 deltaPos = endingPos - startingPos;

        Vector3 cp1 = startingPos + deltaPos * 0.5f + new Vector3(0, fbHeight, 0);
        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * fbSpeed));
            float frameValue = (1f - 0f) * EasingEquations.EaseInCubic(0.0f, 1.0f, currentTime) + 0f;
            Vector3 framePos = MathCurves.Bezier(startingPos, endingPos, cp1, frameValue);
            mbPrefabClone.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }

        controller.animParamController.SetBool("idle");
        controller.animParamController.SetTrigger("cast_end");
        callback();
        GameObject.Destroy(mbPrefabClone);
        Vector3 faceDirection = controller.bc.grid.GetDirection(controller.tile.node, tile.node);
        controller.transform.rotation = Quaternion.LookRotation(faceDirection, Vector3.up);
        yield break;
    }
}