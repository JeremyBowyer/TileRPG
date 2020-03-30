using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonLesserController : EnemyController
{

    public override void CreateCharacter()
    {
        character = new SkeletonLesser
        {
            controller = this
        };
        character.Init();
    }
}
