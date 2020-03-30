using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictorySequence : BattleState
{

    public override bool IsInterruptible
    {
        get { return false; }
    }
    public override bool IsMaster
    {
        get { return true; }
    }

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
        InTransition = true;
        base.Enter();
        CombatLogController.instance.RemoveEntries();
        battleUI.victoryScreenController.confirmAction = OnConfirm;

        battleUI.ShowPanel("victory");
        Dictionary<Character, int> expDict = bc.rewardController.GatherExperience();
        StartCoroutine(battleUI.victoryScreenController.AddExperiences(expDict));
    }

    public void OnConfirm()
    {
        // Hide party members
        ProtagonistController protag = bc.protag;
        protag.TerminateBattle();
        protag.protagAgent.enabled = true;
        foreach (PartyMember member in PersistentObjects.party.GetMembers())
        {
            if (member == protag.character)
                continue;
            member.controller.gameObject.transform.localScale = Vector3.zero;
            member.controller.TerminateBattle();
        }
        battleUI.victoryScreenController.ClearEntries();
        SpawnRewardChest();
        battleUI.ShowPanel("none");

        bc.TerminateBattle();
        InTransition = false;
        bc.ChangeState<IdleState>();
        bc.lc.ChangeState<WorldExploreState>();
    }

    public void SpawnRewardChest()
    {
        List<Node> range = bc.pathfinder.FindRange(bc.protag.tile.node, float.MaxValue, true, true, true, false, true);
        range = bc.pathfinder.CullNodes(range, true, true, true);

        Node closestNode = range[0];
        float dist = Vector3.Distance(bc.protag.tile.anchorPointWorld, closestNode.tile.anchorPointWorld);
        foreach (Node node in range)
        {
            float newDist = Vector3.Distance(bc.protag.tile.anchorPointWorld, node.tile.anchorPointWorld);
            if (newDist < dist)
            {
                dist = newDist;
                closestNode = node;
            }
        }

        List<Item> rewards = bc.rewardController.GatherRewards();
        Currency currency = bc.rewardController.GatherCurrency();
        rewards.Add(currency);
        bc.rewardController.Reset();

        GameObject chestGO = Instantiate(Resources.Load<GameObject>("Prefabs/Map/Props/ItemChest"));
        chestGO.transform.position = closestNode.tile.anchorPointWorld;
        ItemChest chest = chestGO.GetComponent<ItemChest>();

        chest.LoadItems(rewards);
    }

    public override void Exit()
    {
        base.Exit();
        UserInputController.ResetEvents();
    }

}
