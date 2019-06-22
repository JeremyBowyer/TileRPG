using UnityEngine;
using System.Collections;

public class scBSPController : MonoBehaviour {
	
	//Object to control spliting of BSP partion areas
	private scSplitHandler theSplitHandler;
	
	//controls converting BSP into a 3D dungeon
	private scBSP3DConverter the3DConverter;
	
	//controls connecting together rooms
	private scBSPCorridorDigger theDigger;
	
	//stores all the transitive connections a room has
	private scTransitiveConnect theConnections;
	
	//top node of the BSP tree
	private scBSPNode rootNode;
	
	private int splitCount = 0;
	private int targetSplits = 5;
	
	private bool firstFrame = false;
	
	private ArrayList roomList = new ArrayList();
	
	// Use this for initialization
	void Start () {
		theSplitHandler = new scSplitHandler();
		theConnections = new scTransitiveConnect();
		
		GameObject theSection = (GameObject) GameObject.Instantiate(Resources.Load("PartionSection"));
		theSection.transform.localScale = new Vector3(100,100,0.1f);
		theSection.transform.position = new Vector3(0,1,0);
		theSection.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);

		rootNode = new scBSPNode(theSection, null);
		
		for (int i = 0; i < 5; i++){
			splitLeafs(rootNode);
		}

		GameObject folder = GameObject.FindGameObjectWithTag("BSPPartitionSections");
		rootNode.getData().transform.parent = folder.transform;
		
		//rotate all cells to be on correct axis, no longer working in 2D
		folder.transform.eulerAngles = new Vector3(90,0,0);
		//move it to ensure no tiles are at a negative position (which will break the grid)
		folder.transform.position = new Vector3(100,0,100);
		addRoomToLeafs(rootNode);

	}
	
	// Update is called once per frame
	void Update () {

		if(!firstFrame){
			firstFrame = true;
			
			the3DConverter = new scBSP3DConverter();

			theDigger = new scBSPCorridorDigger(the3DConverter.getLevelGrid());
			theDigger.setFloorY(the3DConverter.getFloorY());
			
			connectRooms(rootNode);

			the3DConverter.addWalls();
			
			GameObject folder = GameObject.FindGameObjectWithTag("BSPPartitionSections");
			Destroy(folder);
			
			theConnections.printMap();

		}
	}
	
	//sub divide the leafs of the BSP tree
	private void splitLeafs(scBSPNode _aNode){
		if (_aNode.getLeftChild() == null){
			split (_aNode);
		}else{
			splitLeafs(_aNode.getLeftChild());
			splitLeafs(_aNode.getRightChild());
		}
	}
	
	//find and add rooms to the leafs of the BSP tree
	private void addRoomToLeafs(scBSPNode _aNode){
		if (_aNode.getLeftChild() == null){
			createRoom(_aNode.getData());
			
		}else{
			addRoomToLeafs(_aNode.getLeftChild());
			addRoomToLeafs(_aNode.getRightChild());
		}
	}
	
	private GameObject findLeafOfNode(scBSPNode _aNode){
		if (_aNode.getLeftChild() != null){
			findLeafOfNode (_aNode.getLeftChild());	
		}
		
		return _aNode.getData();
	}
	
	private GameObject findChildWithRoom(scBSPNode _aNode){
		
		foreach(scBSPRoom aRoom in roomList){
			if (aRoom.getParentPartition() == _aNode.getData()){
				return aRoom.getParentPartition();	
			}
		}
		
		return findChildWithRoom(_aNode.getRightChild());	

	}
	
	
	//connect rooms in the BSP together
	private void connectRooms(scBSPNode _aNode){
		
		if (_aNode.getLeftChild() == null){
			
			GameObject child0 = _aNode.getParent().getRightChild().getData();
			
			if (_aNode.getParent().getRightChild().getData() == _aNode.getData()){
				child0 = _aNode.getParent().getLeftChild().getData();
			}
			
			GameObject child1 =_aNode.getData();
			
			theConnections.addConnection(child0,child1);
			
			theConnections.findNearestRooms(child0,child1,out child0, out child1);
			
			theDigger.connect(child0, child1);
			
		}else{	
			connectRooms(_aNode.getLeftChild());
			connectRooms(_aNode.getRightChild());
			

			if (_aNode.getParent() != null){
			
				
				scBSPNode targetNode = _aNode.getParent().getLeftChild();
				
			    if (targetNode == _aNode){
					targetNode = _aNode.getParent().getRightChild();	
				}
				
				GameObject child0 = findChildWithRoom(_aNode);
				
				//GameObject child1 = findChildWithRoom(targetNode);
				GameObject child1 = findChildWithRoom(targetNode);
				
				if (!theConnections.checkConnected(child0,child1)){
					
					theConnections.findNearestRooms(child0,child1,out child0, out child1);
					theConnections.addConnection(child0,child1);
					
		
					theDigger.connect(child0, child1);
				}
			}
		}
	}
	
	//split the leaf partition
	private void split(scBSPNode _aNode){
		
		GameObject a;
		GameObject b;
		
		//split this partition
		theSplitHandler.split(_aNode.getData(), out a, out b);
		
		_aNode.setLeftChild(a);
		_aNode.setRightChild(b);
		
	}
	
	private void createRoom(GameObject _partition){
		
		GameObject room  = GameObject.CreatePrimitive(PrimitiveType.Cube);
		room.transform.localScale = new Vector3(_partition.transform.localScale.x -2, 1 ,_partition.transform.localScale.y -2);
		room.transform.position = new Vector3(_partition.transform.position.x , 0, _partition.transform.position.z );
		room.AddComponent<scRoom>();
		
		scBSPRoom aRoom = new scBSPRoom(_partition);
		roomList.Add(aRoom);
		
	}
	
}
