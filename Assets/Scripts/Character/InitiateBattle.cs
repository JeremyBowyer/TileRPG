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

    void Start()
    {
        gc = GetComponent<CharacterController>().gc;
        protagAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (gc.CurrentState == null)
            return;

        if (gc.CurrentState.GetType().Name == "WorldExploreState")
        {
            origin = transform.position;
            Collider[] alertColliders = Physics.OverlapSphere(origin, sphereRadius, layerMask, QueryTriggerInteraction.UseGlobal);
            foreach (Collider col in alertColliders)
            {
                if (col.tag == "Enemy")
                {
                    gc.battleInitiator = col.gameObject.GetComponent<Enemy>();
                    protagAgent.SetDestination(protagAgent.transform.position);
                    protagAgent.isStopped = true;
                    gc.ChangeState<InitBattleState>();
                }
            }
        }
    }
}
