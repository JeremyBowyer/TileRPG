using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class BSPController : MonoBehaviour
{

    [SerializeField]
    public Vector2 gridSize;
    [SerializeField]
    public int numberOfSplits;

    private GameObject partitionFolder;

	private BSPSplitHandler splitter;	
	private BSPConnectionHandler connections;
    private LevelController lc;
    private NavMeshSurface navMesh;

	//top node of the BSP tree
	private BSPNode rootNode;
	
	private ArrayList roomList = new ArrayList();

    private void Start()
    {
        GenerateMap();
    }

    public void ClearMap()
    {
        Destroy(partitionFolder);
        connections.HideConnections();
    }

    public void GenerateMap ()
    {

        if (gridSize.x == 0 || gridSize.y == 0)
        {
            Debug.LogWarning("Invalid grid size provided, using 100x100.");
            gridSize = new Vector2(100, 100);
        }

        if(numberOfSplits == 0)
        {
            Debug.LogWarning("No splits given, defaulting to 4.");
            numberOfSplits = 4;
        }

        partitionFolder = new GameObject();
        partitionFolder.name = "BSPPartitionPieces";
        partitionFolder.tag = "BSPPartitionSections";

        splitter = new BSPSplitHandler(partitionFolder);
		connections = new BSPConnectionHandler(partitionFolder, this);

        GameObject theSection = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		theSection.transform.localScale = new Vector3(gridSize.x,gridSize.y,0.2f);
		theSection.transform.position = new Vector3(0,0,0);
        theSection.name = "Root Node";
        theSection.transform.parent = partitionFolder.transform;
        rootNode = new BSPNode(theSection, null);
		
		for (int i = 0; i < numberOfSplits; i++){
			SplitLeafs(rootNode);
		}

        //rotate all cells to be on correct axis, no longer working in 2D
        partitionFolder.transform.eulerAngles = new Vector3(90, 0, 0);
        //move it to ensure no tiles are at a negative position (which will break the grid)
        partitionFolder.transform.position = new Vector3(0, 0, 0);

        AddRoomToLeafs(rootNode);

        ConnectRooms(rootNode);
        connections.BuildAllCorridors();
        AddWalls();
        AddProps();
        SetStartingPlace();
        SpawnEnemies();

        lc = GetComponent<LevelController>();
        navMesh = GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();

        lc.InitializeLevel();
        //connections.DisplayConnections();
	}
	
    public void AddRoom(BSPArea _area)
    {
        roomList.Add(_area);
    }

    private void SetStartingPlace()
    {
        BSPRoom leftRoom = rootNode.FindLeftRoom();
        Vector3 midPoint = leftRoom.GetRoundedCenter();
        GameObject startingPlace = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/StartingPlace"));
        startingPlace.transform.position = midPoint;
        startingPlace.transform.parent = partitionFolder.transform;
    }

    private void SpawnEnemies()
    {
        BSPRoom leftRoom = rootNode.FindLeftRoom();
        List<BSPRoom> connectingRooms = connections.GetConnectingRooms(leftRoom);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(connectingRooms.Count > 0)
        {
            enemies[0].transform.position = connectingRooms[0].GetRoundedCenter();
            enemies[0].transform.localScale = enemies[0].transform.localScale * 0.5f;
        }
    }

	//sub divide the leafs of the BSP tree
	private void SplitLeafs(BSPNode _aNode){
		if (_aNode.GetLeftChild() == null){
			Split (_aNode);
		}else{
			SplitLeafs(_aNode.GetLeftChild());
			SplitLeafs(_aNode.GetRightChild());
		}
	}
	
	//find and add rooms to the leafs of the BSP tree
	private void AddRoomToLeafs(BSPNode _aNode){
		if (_aNode.IsLeaf){
			CreateRoom(_aNode);
		} else {
			AddRoomToLeafs(_aNode.GetLeftChild());
			AddRoomToLeafs(_aNode.GetRightChild());
		}
	}
	
	//connect rooms in the BSP together
	private void ConnectRooms(BSPNode _node){

        // If the node is a leaf, connect it with its sibling
        if (_node.IsLeaf)
        {
            BSPRoom leftRoom = _node.GetParent().GetLeftChild().GetRoom();
            BSPRoom rightRoom = _node.GetParent().GetRightChild().GetRoom();
            connections.ConnectRooms(leftRoom, rightRoom);
        }
        // If the node is not a leaf, restart process with its children
        else
        {
            ConnectRooms(_node.GetLeftChild());
            ConnectRooms(_node.GetRightChild());

            // If the node is not a leaf and not root, connect it with its sibling node
            if (!_node.IsRoot)
            {
                BSPNode siblingNode = _node.GetSibling();
                connections.ConnectNodes(_node, siblingNode);
            }

        }
	}
	
	//split the leaf partition
	private void Split(BSPNode _node){
		
		GameObject a;
		GameObject b;
		
		//split this partition
		splitter.split(_node.GetData(), out a, out b);
		
		_node.SetLeftChild(a);
		_node.SetRightChild(b);
	}
	
	private void CreateRoom(BSPNode _node)
    {
		GameObject roomGO  = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roomGO.transform.localScale = new Vector3(_node.GetData().transform.localScale.x, 1 ,_node.GetData().transform.localScale.y);
        roomGO.transform.position = new Vector3(_node.GetData().transform.position.x , 0, _node.GetData().transform.position.z );
        roomGO.GetComponent<BoxCollider>().isTrigger = true;

        BSPRoom room = roomGO.AddComponent<BSPRoom>();
        _node.SetRoom(room);
        room.Node = _node;
        room.Init();
        AddRoom(room);
        
        roomGO.name = "Room_" + roomList.Count;
        roomGO.transform.parent = partitionFolder.transform;
	}

    private void AddWalls()
    {
        foreach(BSPArea room in roomList)
        {
            room.AddWalls();
        }

    }

    private void AddProps()
    {
        foreach (BSPArea area in roomList)
        {
            if (area is BSPRoom)
            {
                BSPRoom room = area as BSPRoom;
                room.AddProps();
            }
        }
    }
	
}
