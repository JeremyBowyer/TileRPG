using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidController : EnemyController
{
    public override void CreateCharacter()
    {
        character = new Druid
        {
            controller = this
        };
        character.Init();
    }

}
