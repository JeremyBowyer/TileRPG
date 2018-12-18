using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitiateBattle : MonoBehaviour
{

    public float sphereRadius = 5f;
    public LayerMask layerMask;
    public GameObject[] enemies;
    public GameController gc;
    private Vector3 origin;
    public NavMeshAgent protagAgent;
    public CharController character;

    void Start()
    {
        character = GetComponent<CharController>();
        protagAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (character.gc.CurrentState == null)
            return;

        if (character.gc.CurrentState.GetType().Name == "WorldExploreState")
        {
            origin = transform.position;
            Collider[] alertColliders = Physics.OverlapSphere(origin, sphereRadius, layerMask, QueryTriggerInteraction.UseGlobal);
            foreach (Collider col in alertColliders)
            {
                if (col.tag == "Enemy")
                {
                    character.gc.battleInitiator = col.gameObject.GetComponent<EnemyController>();
                    protagAgent.SetDestination(protagAgent.transform.position);
                    protagAgent.isStopped = true;
                    character.gc.ChangeState<InitBattleState>();
                }
            }
        }
    }
}
