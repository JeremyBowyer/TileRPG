using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMemberController : PlayerController
{
    //public new PartyMember character;

    public override void Die()
    {
        ProtagonistController protag = bc.protag;
        PartyMember pm = character as PartyMember;
        protag.partyMembers.Remove(pm);
        base.Die();
    }
}
