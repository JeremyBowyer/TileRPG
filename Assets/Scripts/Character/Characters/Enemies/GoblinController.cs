using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinController : EnemyController
{

    public override void CreateCharacter()
    {
        character = new Goblin
        {
            controller = this
        };
        character.Init();
        //character.stats.curHealth = 17;
    }
}
