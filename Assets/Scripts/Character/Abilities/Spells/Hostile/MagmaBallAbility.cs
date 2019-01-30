using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaBallAbility : EnvironmentSpellAbility
{

    GameObject mbPrefabClone;
    List<Node> splashZone;

    public MagmaBallAbility(CharController _character)
    {
        AbilityName = "Magma Ball";
        AbilityDescription = "Attack at range with a magma ball.";
        AbilityID = 2;
        AbilityPower = 12;
        ApCost = 25;
        MpCost = 50;
        AbilityRange = 100;
        diag = true;
        character = _character;
        mouseLayer = LayerMask.NameToLayer("GridClick");
        abilityIntent = AbilityTypes.Intent.Hostile;
    }

    public override bool ValidateCost(CharController _owner)
    {
        return _owner.Stats.curAP >= ApCost && _owner.Stats.curMP >= MpCost;
    }

    public override void ApplyCharacterEffect(CharController character)
    {
        character.Damage(AbilityPower);
    }

    public override void ApplyTileEffect(Tile tile)
    {
        if(tile.Occupant != null)
        {
            CharController character = tile.Occupant.GetComponent<CharController>();
            if (character != null)
                ApplyCharacterEffect(character);
        }

        TileEffect oldEffect = tile.GetComponent<TileEffect>();
        if (oldEffect != null)
            oldEffect.RemoveEffect();
        TileEffect newEffect = tile.gameObject.AddComponent<BurnTileEffect>();
        newEffect.Init(tile);
    }

    public override IEnumerator Initiate(Tile tile, Action callback)
    {
        character.animParamController.SetTrigger("cast_start");
        character.animParamController.SetBool("cast_loop");
        character.transform.LookAt(new Vector3(tile.transform.position.x, character.transform.position.y, tile.transform.position.z));
        Vector3 spawnLocation = new Vector3(character.transform.position.x, character.transform.position.y + 2f, character.transform.position.z);
        mbPrefabClone = GameObject.Instantiate(Resources.Load("Prefabs/Abilities/MagmaBallPrefab") as GameObject, spawnLocation, Quaternion.identity) as GameObject;
        mbPrefabClone.gameObject.tag = "SpellEnvironmentGO";
        inProgress = true;
        Vector3 startingPos = mbPrefabClone.transform.position;
        Vector3 endingPos = tile.WorldPosition;
        float currentTime = 0f;
        float speed = 0.8f;
        float fbHeight = Vector3.Distance(startingPos, endingPos);
        float fbSpeed = speed + speed / fbHeight;

        Vector3 deltaPos = endingPos - startingPos;

        Vector3 cp1 = startingPos + deltaPos * -0.5f + new Vector3(0, fbHeight, 0);
        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * fbSpeed));
            float frameValue = (1f - 0f) * EasingEquations.EaseInExpo(0.0f, 1.0f, currentTime) + 0f;
            Vector3 framePos = MathCurves.Bezier(startingPos, endingPos, cp1, frameValue);
            mbPrefabClone.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }

        character.animParamController.SetBool("idle");
        character.animParamController.SetTrigger("cast_end");
        callback();
        GameObject.Destroy(mbPrefabClone);
        character.transform.rotation = Quaternion.LookRotation(character.bc.grid.GetDirection(character.tile.node, tile.node), Vector3.up);
        yield break;
    }
}