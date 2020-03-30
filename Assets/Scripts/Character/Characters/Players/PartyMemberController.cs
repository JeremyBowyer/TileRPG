using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMemberController : PlayerController
{
    //public new PartyMember character;

    public override void Die(Damage _damage)
    {
        ProtagonistController protag = bc.protag;
        PartyMember pm = character as PartyMember;
        PersistentObjects.party.RemoveMember(pm);
        base.Die(_damage);
    }
}
