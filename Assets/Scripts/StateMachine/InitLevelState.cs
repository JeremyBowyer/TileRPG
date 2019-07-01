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
        protag = lc.protag.GetComponent<ProtagonistController>();

        // Set protag as camera target
        lc.cameraRig.FollowTarget = lc.protag.transform;

        // Place protag on starting spot
        GameObject startingPlace = GameObject.FindGameObjectWithTag("StartingTilePlayer");
        NavMeshAgent protagAgent = protag.GetComponent<NavMeshAgent>();
        protagAgent.Warp(startingPlace.transform.position);
        protag.transform.localScale = protag.transform.localScale * 0.5f;

        // Instantiate Party Members
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

        AdjustSceneObjects();

        inTransition = false;
        lc.ChangeState<WorldExploreState>();
    }

    public void AdjustSceneObjects()
    {
        // Clean up scene from battle
        if (PersistentObjects.enemyName != null)
        {
            PersistentObjects.enemyName = null;
            PersistentObjects.battleInitiator = null;
        }

        if (PersistentObjects.protagonistLocation != null && PersistentObjects.protagonistLocation != Vector3.zero)
        {
            protag.transform.position = PersistentObjects.protagonistLocation;
            PersistentObjects.protagonistLocation = Vector3.zero;
        }

        if (PersistentObjects.deadObjects != null)
        {
            foreach(string id in PersistentObjects.deadObjects)
            {
                GameObject go = GameObject.Find(id);
                go.SetActive(false);

                if (lc.characters.Contains(go)) lc.characters.Remove(go);
                if (lc.players.Contains(go)) lc.players.Remove(go);
                if (lc.enemies.Contains(go)) lc.enemies.Remove(go);
            }
        }
    }

}
