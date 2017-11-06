using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform _target;
	public Vector3 Margin, Smoothing;
	public BoxCollider2D Bounds;
	public bool isFollowing;
	private Vector3 _min, _max;
    private BattleMaster bm;

	// Use this for initialization
	void Start () {
        bm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<BattleMaster>();
		isFollowing = true;
	}


    public void LateUpdate(){

        if(bm.curPlayer != null)
        {
            _target = bm.curPlayer.transform;

            var x = transform.position.x;
            var y = transform.position.y;
            var z = transform.position.z;

            if (isFollowing)
            {
                if (Mathf.Abs(x - _target.position.x) > Margin.x)
                    x = Mathf.Lerp(x, _target.position.x, Smoothing.x * Time.deltaTime);
                if (Mathf.Abs(y - _target.position.y) > Margin.y)
                    y = Mathf.Lerp(y, _target.position.y, Smoothing.y * Time.deltaTime);
                if (Mathf.Abs(z - _target.position.z) > Margin.z)
                    z = Mathf.Lerp(z, _target.position.z, Smoothing.z * Time.deltaTime);
            }

            transform.position = new Vector3(x, y, z);
        }

	}

}
