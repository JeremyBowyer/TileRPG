using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateBattle : MonoBehaviour
{

    public float sphereRadius = 5f;
    public LayerMask layerMask;
    public GameObject[] enemies;
    public GameController gc;
    private Vector3 origin;

    void Start()
    {
        //layerMask = LayerMask.NameToLayer("Character");
        gc = GetComponent<Character>().gc;
    }

    void Update()
    {
        //origin = transform.position;
        //Collider[] noticeColliders = Physics.OverlapSphere(origin, sphereRadius*1.5f, layerMask, QueryTriggerInteraction.UseGlobal);
        //foreach (Collider col in noticeColliders)
        //{
        //    if (col.tag == "Protag")
        //    {
        //        gc.protag.transform.LookAt(transform);
        //    }
        //}

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
                    /*
                    foreach (GameObject enemy in gc.worldEnemies)
                    {
                        enemy.GetComponent<DetectPlayer>().enabled = false;
                    }
                    */
                    gc.battleInitiator = col.gameObject.GetComponent<Enemy>();
                    gc.ChangeState<InitBattleState>();
                }
            }
        }
    }
}
