using UnityEngine;
using System.Collections;

public class scBSPNode {
	
	private GameObject data;
	private scBSPNode leftChild;
	private scBSPNode rightChild;
	private scBSPNode parent;
	
	public scBSPNode(GameObject _partion, scBSPNode _parent){
		data = _partion;
		parent = _parent;
	}
	
	public scBSPNode getLeftChild(){
		return leftChild;	
	}
	
	public void setLeftChild(GameObject _partion){
		leftChild = new scBSPNode(_partion, this);	
	}
	
	public scBSPNode getRightChild(){
		return rightChild;	
	}
	
	public void setRightChild(GameObject _partion){
		rightChild = new scBSPNode(_partion, this);
	}
	
	public GameObject getData(){
		return data;	
	}
	
	public scBSPNode getParent(){
		return parent;	
	}

}
