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

    public override void CreateCharacter()
    {
        character = new Executioner
        {
            controller = this
        };
        character.Init();
        //character.stats.maxHealth = 1000;
        //character.stats.maxAP = 10000;
        //character.stats.Init();
        character.cName = "Protagonist";

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

        Wizard member3 = new Wizard()
        {
            cName = "Son of Kong",
            cClass = "Wizard",
            model = "Wizard"
        };
        partyMembers.Add(member3);

        inventory = new Inventory();
        inventory.Add(new Potion());
        inventory.Add(new Potion());
        inventory.Add(new Potion());

    }

    private void LateUpdate()
    {
        if (gc.CurrentState == null)
            return;

        if (gc.CurrentState.GetType().Name != "WorldExploreState")
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

}
