using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMemberController : PlayerController
{
    //public new PartyMember character;

    public override void Die()
    {
        Protagonist protag = gc.protag.character as Protagonist;
        PartyMember pm = character as PartyMember;
        protag.partyMembers.Remove(pm);
        base.Die();
    }
}
