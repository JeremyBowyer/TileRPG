using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProtagonistController : PlayerController
{
    public NavMeshAgent protagAgent;

    public override void Awake()
    {
        base.Awake();
        protagAgent = GetComponent<NavMeshAgent>();
    }

    public override void CreateCharacter()
    {
        character = new Protagonist
        {
            controller = this
        };
        character.Init();
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

    public override void AfterDeath()
    {
        Application.Quit();
    }

}
