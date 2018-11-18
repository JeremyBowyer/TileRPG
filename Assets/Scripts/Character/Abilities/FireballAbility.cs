using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAbility : BaseAbility
{

    GameObject fbPrefabClone;

    public FireballAbility(Character _character)
    {
        AbilityName = "Fireball attack";
        AbilityDescription = "Attack at range with an arrow.";
        AbilityID = 2;
        AbilityPower = 50;
        AbilityCost = 25;
        AbilityRange = 100;
        diag = true;
        character = _character;
    }

    public override IEnumerator Initiate(Character _target)
    {
        character.transform.LookAt(new Vector3(_target.transform.position.x, character.transform.position.y, _target.transform.position.z));
        Vector3 spawnLocation = new Vector3(character.transform.position.x, character.transform.position.y + 2f, character.transform.position.z);
        fbPrefabClone = GameObject.Instantiate(Resources.Load("Prefabs/Abilities/FireballPrefab") as GameObject, spawnLocation, Quaternion.identity) as GameObject;
        isAttacking = true;
        Vector3 startingPos = fbPrefabClone.transform.position;
        Vector3 endingPos = _target.transform.position;
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
            fbPrefabClone.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }
        isAttacking = false;
        _target.Damage(AbilityPower);
        GameObject.Destroy(fbPrefabClone);
        character.transform.rotation = Quaternion.LookRotation(character.gc.grid.GetDirection(character.tile.node, _target.tile.node), Vector3.up);
        yield break;
    }
}