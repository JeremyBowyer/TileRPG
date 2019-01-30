using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitLevelState : WorldState
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
        lc.cameraRig.FollowTarget = lc.protag.transform;

        // Place protag on starting spot
        GameObject startingPlace = GameObject.FindGameObjectWithTag("StartingTilePlayer");
        lc.protag.gameObject.transform.position = startingPlace.transform.position;

        // Instantiate Party Members
        protag = lc.protag.GetComponent<ProtagonistController>();
        protag.InitPartyMembers();
        PersistentObjects.SaveProtagonist(lc);

        // Find Enemies and Players, add them to lists
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            lc.enemies.Add(enemy);
            lc.characters.Add(enemy);
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            lc.players.Add(player);
            lc.characters.Add(player);
        }
        lc.players.Add(lc.protag.gameObject);
        lc.characters.Add(lc.protag.gameObject);


        inTransition = false;
        lc.ChangeState<WorldExploreState>();
    }

}
