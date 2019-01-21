using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitLevelState : State
{
    private ProtagonistController protag;

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

        // Set protag as camera target
        gc.cameraRig.followTarget = gc.protag.transform;

        // Place protag on starting spot
        GameObject startingPlace = GameObject.FindGameObjectWithTag("StartingTilePlayer");
        gc.protag.gameObject.transform.position = startingPlace.transform.position;

        // Instantiate Party Members
        protag = gc.protag.GetComponent<ProtagonistController>();
        foreach (PartyMember member in protag.partyMembers)
        {
            GameObject goMember = Instantiate(Resources.Load("Prefabs/Characters/"+member.model)) as GameObject;
            PartyMemberController controller = goMember.AddComponent<PartyMemberController>();
            member.controller = controller;
            controller.character = member;
            member.Init();
            goMember.name = member.cName;
            goMember.tag = "Player";
            goMember.transform.localScale = Vector3.zero;
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
