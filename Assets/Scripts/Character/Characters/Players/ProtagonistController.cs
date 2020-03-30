using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProtagonistController : PlayerController
{
    public NavMeshAgent protagAgent;

    public Inventory inventory;

    private HubController hc;

    public override void InitController()
    {
        base.InitController();
        GameObject hcGO = GameObject.Find("HubController");
        if (hcGO != null)
            hc = hcGO.GetComponent<HubController>();
        protagAgent = GetComponent<NavMeshAgent>();
    }

    public override void LoadCharacter(Character _character)
    {
        character = _character;
        character.controller = this;
    }

    public void LoadInventory(Inventory _inventory)
    {
        inventory = _inventory;
    }

    public override void CreateCharacter()
    {
        if(PersistentObjects.protagonist == null)
        {
            character = new Executioner
            {
                controller = this,
                cName = "Protagonist"
            };
            character.Init();
        }
        else
        {
            LoadCharacter(PersistentObjects.protagonist);
        }

        if (PersistentObjects.bag != null)
            LoadInventory(PersistentObjects.bag);
    }

    private void LateUpdate()
    {
        if (protagAgent == null)
            return;

        if (bc != null && bc.InBattle)
            return;

        if (lc != null && (lc.CurrentState == null || lc.CurrentState.GetType().Name != "WorldExploreState"))
            return;

        if (!protagAgent.isOnNavMesh)
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float dist = protagAgent.remainingDistance;

        if (x == 0 && y == 0 && dist < 0.1f)
        {
            animParamController.SetBool("idle", true);
        } else
        {
            animParamController.SetBool("running");
        }
    }

    public override void Pause()
    {
        base.Pause();
        protagAgent.isStopped = true;
    }

    public override void Resume()
    {
        base.Resume();
        protagAgent.isStopped = false;
    }

    public override void AfterDeath(Damage _damage)
    {
        Application.Quit();
    }

    public void InstantiatePartyMembers()
    {
        foreach (PartyMember member in PersistentObjects.party.GetMembers())
        {
            if (member.controller == this)
                continue;

            if (member.controller != null)
                continue;
            GameObject goMember = Instantiate(Resources.Load("Prefabs/Characters/Players/" + member.model)) as GameObject;
            PartyMemberController controller = goMember.AddComponent<PartyMemberController>();
            member.controller = controller;
            controller.character = member;
            member.Init();

            goMember.name = member.cName;
            goMember.tag = "Player";
            goMember.transform.localScale = Vector3.zero;
        }
    }

}
