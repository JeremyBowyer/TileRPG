using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour {

    public float sphereRadius = 5f;
    public LayerMask layerMask;
    public GameObject[] enemies;
    public GameController gc;
    private Vector3 origin;

	// Use this for initialization
	void Start () {
        layerMask = LayerMask.NameToLayer("Protag");
        gc = GetComponent<Character>().gc;
    }
	
	// Update is called once per frame
	void Update () {

        //origin = transform.position;
        //Collider[] noticeColliders = Physics.OverlapSphere(origin, sphereRadius*1.5f, layerMask, QueryTriggerInteraction.UseGlobal);
        //foreach (Collider col in noticeColliders)
        //{
        //    if (col.tag == "Protag")
        //    {
        //        gc.protag.transform.LookAt(transform);
        //    }
        //}


        origin = transform.position;
        Collider[] alertColliders = Physics.OverlapSphere(origin, sphereRadius, layerMask, QueryTriggerInteraction.UseGlobal);
        foreach(Collider col in alertColliders)
        {
            if (col.tag == "Protag")
            {
                foreach(GameObject enemy in gc.worldEnemies)
                {
                    enemy.GetComponent<DetectPlayer>().enabled = false;
                }
                gc.battleInitiator = gameObject.GetComponent<Enemy>();
                gc.ChangeState<InitBattleState>();
            }
        }
	}
}
