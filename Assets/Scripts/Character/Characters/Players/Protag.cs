using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Protag : Player
{
    public NavMeshAgent protagAgent;
    public List<PartyMember> partyMembers = new List<PartyMember>();

    private void Start()
    {
        base.Awake();
        stats.maxHealth = 1000;
        stats.maxAP = 10000;
        stats.Init();
        characterName = "Protagonist";
        spells.Add(new MagmaBallAbility(this));
        spells.Add(new FireboltAbility(this));
        attackAbility = new ArrowAbility(this);
        movementAbility = new JumpMovement(this, gc);
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        protagAgent = GetComponent<NavMeshAgent>();

        partyMembers.Add(new PartyMember()
        {
            cName = "Fat Andrew",
            cClass = "Vagrant"
        });

        partyMembers.Add(new PartyMember()
        {
            cName = "Dumb Charlie",
            cClass = "Ingrate"
        });

        partyMembers.Add(new PartyMember()
        {
            cName = "Lazy Tedd",
            cClass = "Degenerate"
        });
    }

    private void LateUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float velocity = protagAgent.velocity.magnitude;

        if (x == 0 && y == 0 && velocity < 0.1f)
        {
            animParamController.SetBool("idle", true);
        } else
        {
            animParamController.SetBool("running");
        }
    }
}
