using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProtagonistController : PlayerController
{
    public NavMeshAgent protagAgent;

    public List<PartyMember> partyMembers = new List<PartyMember>();
    public Inventory inventory;

    public override void Awake()
    {
        base.Awake();
        protagAgent = GetComponent<NavMeshAgent>();
    }

    public override void LoadCharacter(Character _character)
    {
        character = _character;
        character.controller = this;
        character.InitAbilities();
    }

    public void LoadPartyMembers(List<PartyMember> _partyMembers)
    {
        partyMembers = _partyMembers;
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
                controller = this
            };
            character.Init();
            character.InitAbilities();
            character.cName = "Protagonist";
        }
        else
        {
            LoadCharacter(PersistentObjects.protagonist);
        }

        if(PersistentObjects.partyMembers == null)
        {
            Wizard member3 = new Wizard()
            {
                cName = "Son of Kong",
                cClass = "Wizard",
                model = "Wizard"
            };
            partyMembers.Add(member3);

            Rogue member1 = new Rogue()
            {
                cName = "Wingus",
                cClass = "Rogue",
                model = "Rogue"
            };
            partyMembers.Add(member1);

            Rogue member2 = new Rogue()
            {
                cName = "Dingus",
                cClass = "Rogue",
                model = "Rogue"
            };
            partyMembers.Add(member2);

        }
        else
        {
            LoadPartyMembers(PersistentObjects.partyMembers);
        }

        if(PersistentObjects.inventory == null)
        {
            inventory = new Inventory();
            inventory.Add(new Potion());
            inventory.Add(new Potion());
            inventory.Add(new Potion());
        } else
        {
            LoadInventory(PersistentObjects.inventory);
        }

    }

    private void LateUpdate()
    {
        if (bc != null)
            return;

        if (lc.CurrentState == null)
            return;

        if (lc.CurrentState.GetType().Name != "WorldExploreState")
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

    public override void AfterDeath()
    {
        Application.Quit();
    }

    public void InitPartyMembers()
    {
        foreach(PartyMember member in partyMembers)
        {
            member.Init();
        }
    }

    public void InstantiatePartyMembers()
    {
        foreach (PartyMember member in partyMembers)
        {
            GameObject goMember = Instantiate(Resources.Load("Prefabs/Characters/" + member.model)) as GameObject;
            PartyMemberController controller = goMember.AddComponent<PartyMemberController>();
            member.controller = controller;
            controller.character = member;
            member.InitAbilities();
            member.Init();

            goMember.name = member.cName;
            goMember.tag = "Player";
            goMember.transform.localScale = Vector3.zero;
        }
    }

}
