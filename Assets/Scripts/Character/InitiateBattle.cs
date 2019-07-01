using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class InitiateBattle : MonoBehaviour
{

    public float sphereRadius = 2f;
    public LayerMask layerMask;
    public GameObject[] enemies;
    public BattleController gc;
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
        if (character.lc == null)
            return;

        if (character.lc.CurrentState == null)
            return;

        if (character.lc.CurrentState.GetType().Name == "WorldExploreState")
        {
            origin = transform.position;
            Collider[] alertColliders = Physics.OverlapSphere(origin, sphereRadius, layerMask, QueryTriggerInteraction.UseGlobal);
            foreach (Collider col in alertColliders)
            {
                if (col.tag == "Enemy")
                {
                    character.lc.battleInitiator = col.gameObject.GetComponent<EnemyController>();
                    protagAgent.SetDestination(protagAgent.transform.position);
                    protagAgent.isStopped = true;
                    //character.lc.ChangeState<InitBattleState>();
                    PersistentObjects.enemyName = col.name;
                    PersistentObjects.battleInitiator = col.GetComponent<EnemyController>().character as Enemy;
                    PersistentObjects.protagonistLocation = origin;
                    PersistentObjects.RemoveObject(col.gameObject.name);
                    SceneManager.LoadScene("Battle");
                }
            }
        }
    }
}
