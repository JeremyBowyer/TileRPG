using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKnightController : EnemyController
{

    public override void CreateCharacter()
    {
        character = new SkeletonKnight
        {
            controller = this
        };
        character.Init();
    }
}
