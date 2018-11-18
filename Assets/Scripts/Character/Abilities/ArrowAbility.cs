using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAbility : BaseAbility {

    GameObject arrowPrefabClone;

    public ArrowAbility(Character _character)
    {
        AbilityName = "Arrow attack";
        AbilityDescription = "Attack at range with an arrow.";
        AbilityID = 2;
        AbilityPower = 50;
        AbilityCost = 25;
        AbilityRange = 60;
        diag = true;
        character = _character;
    }

    public override IEnumerator Initiate(Character _target)
    {

        arrowPrefabClone = GameObject.Instantiate(Resources.Load("Prefabs/Abilities/ArrowPrefab") as GameObject, character.transform.position, Quaternion.identity) as GameObject;
        isAttacking = true;
        Vector3 startingPos = arrowPrefabClone.transform.position;
        Vector3 endingPos = _target.transform.position;
        float currentTime = 0f;
        float speed = 2f;
        float arrowHeight = Vector3.Distance(startingPos, endingPos) / 4;
        float arrowSpeed = speed + speed / arrowHeight;

        while (!Mathf.Approximately(currentTime, 1.0f))
        {
            currentTime = Mathf.Clamp01(currentTime + (Time.deltaTime * arrowSpeed));
            Vector3 framePos = MathCurves.Parabola(startingPos, endingPos, arrowHeight, currentTime);
            arrowPrefabClone.transform.position = framePos;
            yield return new WaitForEndOfFrame();
        }
        isAttacking = false;
        _target.Damage(AbilityPower);
        GameObject.Destroy(arrowPrefabClone);
        yield break;
    }
}
