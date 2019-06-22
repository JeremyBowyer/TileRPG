using UnityEngine;
using System.Collections;

public class scBSPRoom{
	
	GameObject parentPartition;
	
	public scBSPRoom(GameObject _parentPartition){
		parentPartition = _parentPartition;
	}
	
	public GameObject getParentPartition(){
		return parentPartition;	
	}

}
