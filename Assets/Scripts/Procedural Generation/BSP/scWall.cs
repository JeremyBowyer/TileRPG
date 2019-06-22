using UnityEngine;
using System.Collections;

public class scWall : MonoBehaviour {
	
	private GameObject checkCollider;
	
	public int hash;
	
	//Delunay setup
	public void autoTile(){
		checkCollider = GameObject.FindGameObjectWithTag("CheckCollider");
		checkCollider.GetComponent<scCheckCollider>().setup(this.gameObject);
		
		hash = checkCollider.GetComponent<scCheckCollider>().getHash();
		
		GameObject tile = checkCollider.GetComponent<scCheckCollider>().createTile(hash);

		if (tile != null){
					
			tile.transform.position = new Vector3(transform.position.x, (transform.position.y - GetComponent<Renderer>().bounds.size.y/2) + tile.GetComponent<Renderer>().bounds.size.y/2, transform.position.z);
			Instantiate(tile);
			Destroy(this.gameObject);
			
		}
	}
}
