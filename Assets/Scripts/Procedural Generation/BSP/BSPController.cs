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
    [SerializeField]
    public Vector2 roomBufferBounds;

    private GameObject partitionFolder;

	private BSPSplitHandler splitter;	
	private BSPConnectionHandler connections;
    private LevelController lc;
    private NavMeshSurface navMesh;

    public MapController uiMapController;

	//top node of the BSP tree
	public BSPNode rootNode;
	public ArrayList roomList = new ArrayList();
	private ArrayList corridorList = new ArrayList();

    private void Start()
    {
        GenerateMap();
        uiMapController.Init();
        uiMapController.GenerateMapUI();
    }

    public void ClearMap()
    {
        Destroy(partitionFolder);
        connections.HideConnections();
        roomList = new ArrayList();
        corridorList = new ArrayList();
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

        connections = gameObject.AddComponent<BSPConnectionHandler>();
        connections.Init(partitionFolder, this);

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

        AddWalls();
        ConnectRooms(rootNode);
        connections.BuildAllCorridors();

        AddProps();
        SetStartingPlace();
        AddEnemies();

        lc = GetComponent<LevelController>();
        navMesh = GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();

        lc.InitializeLevel();
	}
	
    public void ShowRoom(BSPRoom _room)
    {
        foreach(BSPRoom room in roomList)
        {
            if (room != _room)
                room.HideRoom();
        }

        foreach (BSPCorridor corridor in corridorList)
        {
            corridor.HideRoom();
        }
    }

    public void ShowAllRooms()
    {
        foreach (BSPRoom room in roomList)
        {
            room.ShowRoom();
        }

        foreach (BSPCorridor corridor in corridorList)
        {
            corridor.ShowRoom();
        }
    }

    public void AddRoom(BSPRoom _room)
    {
        roomList.Add(_room);
    }

    private void SetStartingPlace()
    {
        BSPRoom leftRoom = rootNode.FindLeftRoom();
        Vector3 midPoint = leftRoom.GetRoundedCenter();
        GameObject startingPlace = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/StartingPlace"));
        startingPlace.transform.position = midPoint;
        startingPlace.transform.parent = partitionFolder.transform;
    }

	//sub divide the leafs of the BSP tree
	private void SplitLeafs(BSPNode _node){
		if (_node.IsLeaf){
			Split (_node);
		}else{
            SplitLeafs(_node.GetRightChild());
            //if (Random.value > 0.3f)
            //    SplitLeafs(_node.GetLeftChild());
            SplitLeafs(_node.GetLeftChild());
        }
	}
	
	//find and add rooms to the leafs of the BSP tree
	private void AddRoomToLeafs(BSPNode _node){
		if (_node.IsLeaf){
			CreateRoom(_node);
		} else {
			AddRoomToLeafs(_node.GetLeftChild());
			AddRoomToLeafs(_node.GetRightChild());
		}
	}
	
	//connect rooms in the BSP together
	private void ConnectRooms(BSPNode _node){

        // If the node is a leaf, connect it with its sibling
        if (_node.IsLeaf && _node.SiblingIsLeaf)
        {
            BSPRoom leftRoom = _node.GetParent().GetLeftChild().GetRoom();
            BSPRoom rightRoom = _node.GetParent().GetRightChild().GetRoom();

            connections.ConnectRooms(leftRoom, rightRoom);
        }
        // If the node is not a leaf, restart process with its children
        else
        {
            if(_node.GetLeftChild() != null)
                ConnectRooms(_node.GetLeftChild());

            if(_node.GetRightChild() != null)
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
        roomGO.layer = LayerMask.NameToLayer("Ignore Raycast");
        roomGO.transform.localScale = new Vector3(_node.GetData().transform.localScale.x, 1 ,_node.GetData().transform.localScale.y);
        roomGO.transform.position = new Vector3(_node.GetData().transform.position.x , 0, _node.GetData().transform.position.z );
        roomGO.GetComponent<BoxCollider>().isTrigger = true;
        int roomId = roomList.Count + 1;

        if (roomGO.GetComponent<BoxCollider>().bounds.size.x > 9 && roomGO.GetComponent<BoxCollider>().bounds.size.z > 9 && _node != rootNode.FindLeftLeaf())
        {
            BSPBattleRoom room = roomGO.AddComponent<BSPBattleRoom>();
            _node.SetRoom(room);
            room.Node = _node;
            room.Init(roomId, roomBufferBounds);
            AddRoom(room);
        }
        else
        {
            BSPRoom room = roomGO.AddComponent<BSPRoom>();
            _node.SetRoom(room);
            room.Node = _node;
            room.Init(roomId, roomBufferBounds);
            AddRoom(room);
        }
        
        roomGO.name = "Room_" + roomId;
        roomGO.transform.parent = partitionFolder.transform;
	}

    private void AddWalls()
    {
        foreach(BSPRoom room in roomList)
        {
            room.AddWalls();
        }

    }

    private void AddProps()
    {
        foreach (BSPRoom room in roomList)
        {
            room.AddProps();
        }

        foreach (BSPCorridor corridor in corridorList)
        {
            corridor.AddProps();
        }
    }

    private void AddEnemies()
    {
        foreach (BSPRoom room in roomList)
        {
            if (room is BSPBattleRoom)
            {
                BSPBattleRoom battleRoom = room as BSPBattleRoom;
                battleRoom.AddEnemies();
            }
        }
    }

    public void AddCorridor(BSPCorridor _corridor)
    {
        _corridor.transform.parent = partitionFolder.transform;
        corridorList.Add(_corridor);
        _corridor.gameObject.name = "corridor_" + corridorList.Count;
    }
	
}
