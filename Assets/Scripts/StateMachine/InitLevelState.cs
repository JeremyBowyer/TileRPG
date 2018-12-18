using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitLevelState : State
{
    private Protagonist protag;

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(WorldExploreState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        base.Enter();
        gc.cameraRig.followTarget = gc.protag.transform;
        GameObject startingPlace = GameObject.FindGameObjectWithTag("StartingTilePlayer");
        gc.protag.gameObject.transform.position = startingPlace.transform.position;
        protag = gc.protag.GetComponent<ProtagonistController>().character as Protagonist;
        foreach (PartyMember member in protag.partyMembers)
        {
            GameObject goMember = Instantiate(Resources.Load("Prefabs/Characters/character_base")) as GameObject;
            goMember.transform.localScale = Vector3.zero;
            PartyMemberController controller = goMember.AddComponent<PartyMemberController>();
            member.controller = controller;
            controller.character = member;
            member.Init();
            goMember.transform.Find(member.model).gameObject.SetActive(true);
            goMember.name = member.cName;
            goMember.tag = "Player";
        }

        // Find Enemies and Players, add them to lists
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            gc.worldEnemies.Add(enemy);
            gc.characters.Add(enemy);
        }

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            gc.players.Add(player);
            gc.characters.Add(player);
        }
        gc.players.Add(gc.protag.gameObject);
        gc.characters.Add(gc.protag.gameObject);

        inTransition = false;
        gc.ChangeState<WorldExploreState>();
    }

}
