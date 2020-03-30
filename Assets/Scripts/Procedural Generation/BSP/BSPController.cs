using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class BSPController : MonoBehaviour
{
    [Tooltip("Size of map from end to end, before shrinking rooms for buffer space.")]
    public Vector2 gridSize;
    [Tooltip("BSP Splitter will never exceed this number of splits. Setting this value high will effectively defer to 'Min Split Size' determining number of rooms.")]
    public int numberOfSplits;
    [Tooltip("BSP Splitter will not generate nodes smaller than this size.")]
    public int minSplitSize;
    [Tooltip("Rooms will never exceed this aspect ratio (X to Y)")]
    public float maxAspectRatio;
    [Tooltip("Bounds to generate random amount of 'buffer' space around a room. This will be overridden by max and min room requirements.")]
    public Vector2 roomBufferBounds;
    [Tooltip("Rooms will always be at least this many floors along both axes.")]
    public int minRoomSize;
    [Tooltip("Rooms will always be at most this many floors along both axes.")]
    public int maxRoomSize;

    private GameObject partitionFolder;
    public LevelGraph graph;
    public List<LevelVertex> criticalPath;

    private BSPSplitHandler splitter;	
	private BSPConnectionHandler connections;
	private KeepRoomsController roomsController;
    private LevelController lc;
    private NavMeshSurface navMesh;

    LevelVertex start;
    LevelVertex end;
    LevelVertex shop;
    LevelVertex respite;

    //top node of the BSP tree
    public BSPNode rootNode;
	public ArrayList roomList = new ArrayList();
	private ArrayList corridorList = new ArrayList();

    public static BSPController instance;

    private void Start()
    {
        instance = this;
        if(ValidateInputs())
            GenerateMap();
    }

    public bool ValidateInputs()
    {

        // Errors
        if (minSplitSize == 0)
        {
            Debug.LogError("Min Split Size cannot be 0.");
            return false;
        }

        if (minRoomSize == 0)
        {
            Debug.LogError("Min Room Size cannot be 0.");
            return false;
        }

        if (minSplitSize < minRoomSize)
        {
            Debug.LogError("Min Room Size cannot be greater than Min Split Size.");
            return false;
        }

        if (minRoomSize > maxRoomSize)
        {
            Debug.LogError("Min Room Size cannot be greater than Max Room Size.");
            return false;
        }

        if (roomBufferBounds.x > roomBufferBounds.y)
        {
            Debug.LogError("Minimum Room Buffer cannot be greater than Maxiumum Room Buffer");
            return false;
        }

        // Warnings
        if (gridSize.x == 0 || gridSize.y == 0)
        {
            Debug.LogWarning("Invalid grid size provided, using 100x100.");
            gridSize = new Vector2(100, 100);
        }

        if (numberOfSplits == 0)
        {
            Debug.LogWarning("No splits given, defaulting to 4.");
            numberOfSplits = 4;
        }

        return true;
    }

    public void ClearMap()
    {
        Destroy(partitionFolder);
        roomList = new ArrayList();
        corridorList = new ArrayList();
    }

    public void GenerateMap ()
    {

        partitionFolder = new GameObject();
        partitionFolder.name = "BSPPartitionPieces";
        partitionFolder.tag = "BSPPartitionSections";

        splitter = new BSPSplitHandler(partitionFolder, minSplitSize, maxAspectRatio);

        connections = gameObject.AddComponent<BSPConnectionHandler>();
        connections.Init(this);

        GameObject theSection = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		theSection.transform.localScale = new Vector3(gridSize.x,gridSize.y,0.2f);
		theSection.transform.position = new Vector3(0,0,0);
        theSection.name = "Root Node";
        theSection.transform.parent = partitionFolder.transform;

        // Create Graph using recursion
        rootNode = new BSPNode(theSection, null);
		for (int i = 0; i < numberOfSplits; i++){
			SplitLeafs(rootNode);
		}
        //ShrinkLeafs(rootNode);
        DestroyNonLeafGOs(rootNode);

        //rotate all cells to be on correct axis, no longer working in 2D
        partitionFolder.transform.eulerAngles = new Vector3(90, 0, 0);
        //move it to ensure no tiles are at a negative position (which will break the grid)
        partitionFolder.transform.position = new Vector3(0, 0, 0);

        graph = new LevelGraph();
        AddVerticesToGraph(rootNode);
        AddEdgesToGraph(rootNode);

        roomsController = new KeepRoomsController();

        // Populate Graph with Rooms
        EstablishRoomTypes();

        roomsController.InstantiateRoom(start, KeepRoomsController.KeepRoomType.StartRoom);
        roomsController.InstantiateRoom(end, KeepRoomsController.KeepRoomType.EndRoom);
        roomsController.InstantiateRoom(respite, KeepRoomsController.KeepRoomType.RespiteRoom);
        roomsController.InstantiateRoom(shop, KeepRoomsController.KeepRoomType.ShopRoom);
        roomsController.InstantiateRoom(criticalPath[1], KeepRoomsController.KeepRoomType.ShopRoom);
        
        // Instantiate remaining rooms randomly
        foreach(LevelVertex vertex in graph.vertices)
        {
            roomsController.InstantiateRoom(vertex);
        }


        // Add Walls
        foreach (LevelVertex vertex in graph.vertices)
        {
            KeepRoom room = vertex.data.GetComponent<KeepRoom>();
            if (room != null)
                room.AddWalls();
        }

        // Build Corridors
        foreach(LevelEdge edge in graph.edges)
        {
            KeepRoom roomA = edge.a.data.GetComponent<KeepRoom>();
            KeepRoom roomB = edge.b.data.GetComponent<KeepRoom>();

            connections.BuildCorridor(roomA, roomB);
        }

        // Add Props
        foreach (LevelVertex vertex in graph.vertices)
        {
            KeepRoom room = vertex.data.GetComponent<KeepRoom>();
            if (room != null)
                room.AddProps();
        }

        foreach (KeepCorridor corridor in corridorList)
        {
            corridor.AddProps();
        }

        // Add Enemies
        foreach (LevelVertex vertex in graph.vertices)
        {
            KeepRoom room = vertex.data.GetComponent<KeepRoom>();
            if (room != null && room is KeepBattleRoom)
                ((KeepBattleRoom)room).AddEnemies();
        }

        // Combine Meshes
        foreach (LevelVertex vertex in graph.vertices)
        {
            KeepRoom room = vertex.data.GetComponent<KeepRoom>();
            if (room != null)
                room.CombineMeshes();
                continue;
        }

        lc = GetComponent<LevelController>();
        navMesh = GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();

        lc.InitializeLevel();

        /*
        DisplayVertices();
        graph.DisplayEdges();
        */
    }
	
    public void EstablishRoomTypes()
    {
        SetStartRoom(rootNode);
        SetEndRoom(rootNode);
        CalculateCriticalPath();
        SetBonusRooms();
    }

    public void DisplayVertices()
    {
        foreach(LevelVertex vertex in graph.vertices)
        {
            if (criticalPath.Contains(vertex))
                vertex.Show(Color.yellow);
            if (vertex == start)
                vertex.Show(Color.green);
            if (vertex == end)
                vertex.Show(Color.red);
            if (vertex == shop)
                vertex.Show(Color.magenta);
            if (vertex == respite)
                vertex.Show(Color.blue);
        }
    }

    public void AddRoom(KeepRoom _room)
    {
        roomList.Add(_room);
    }

    private void SetStartRoom(BSPNode _node)
    {
        BSPNode startNode = _node.GetLeftLeafNode();
        start = graph.GetVertex(startNode.GetData());
    }

    private void SetEndRoom(BSPNode _node)
    {
        BSPNode endNode = _node.GetRightLeafNode();
        end = graph.GetVertex(endNode.GetData());
    }

    private void SetBonusRooms()
    {
        List<LevelVertex> reservedVertices = new List<LevelVertex>(criticalPath);

        shop = graph.GetFurthestVertex(criticalPath, _exclude: reservedVertices);
        reservedVertices.Add(shop);

        respite = graph.GetClosestVertex(criticalPath[criticalPath.Count / 2], _exclude: reservedVertices);
        reservedVertices.Add(respite);
    }

    private void CalculateCriticalPath()
    {
        criticalPath = graph.FindShortestPath(start, end);
        criticalPath.Insert(0, start);
    }

    private void DestroyNonLeafGOs(BSPNode _node)
    {
        if (_node == null)
            return;

        if (!_node.IsLeaf)
            Destroy(_node.GetData());

        DestroyNonLeafGOs(_node.GetLeftChild());
        DestroyNonLeafGOs(_node.GetRightChild());
    }

	//sub divide the leafs of the BSP tree
	private void SplitLeafs(BSPNode _node){
		if (_node.IsLeaf){
			Split(_node);
		}else{
            SplitLeafs(_node.GetRightChild());
            SplitLeafs(_node.GetLeftChild());
        }
	}

    //split the leaf partition
    private void Split(BSPNode _node)
    {

        GameObject a;
        GameObject b;

        //split this partition
        splitter.split(_node.GetData(), out a, out b);

        _node.SetLeftChild(a);
        _node.SetRightChild(b);
    }

    private void AddVerticesToGraph(BSPNode _node)
    {
        if (_node.IsLeaf)
        {
            graph.AddVertex(_node.GetData());
        }
        else
        {
            AddVerticesToGraph(_node.GetLeftChild());
            AddVerticesToGraph(_node.GetRightChild());
        }
    }

    private void AddEdgesToGraph(BSPNode _node)
    {
        // If the node is a leaf, connect it with its sibling
        if (_node.IsLeaf && _node.SiblingIsLeaf)
        {
            graph.AddEdge(_node.GetParent().GetLeftChild().GetData(), _node.GetParent().GetRightChild().GetData());
        }
        // If the node is not a leaf, restart process with its children
        else
        {
            if (_node.GetLeftChild() != null)
                AddEdgesToGraph(_node.GetLeftChild());

            if (_node.GetRightChild() != null)
                AddEdgesToGraph(_node.GetRightChild());

            // If the node is not a leaf and not root, connect it with its sibling node
            if (!_node.IsRoot)
            {
                BSPNode siblingNode = _node.GetSibling();

                GameObject a;
                GameObject b;

                FindClosestEdge(_node, siblingNode, out a, out b);

                graph.AddEdge(a, b);
            }

        }
    }

    private void AddEnemies()
    {
        foreach (KeepRoom room in roomList)
        {
            if (room is KeepBattleRoom)
            {
                KeepBattleRoom battleRoom = room as KeepBattleRoom;
                battleRoom.AddEnemies();
            }
        }
    }

    public void AddCorridor(KeepCorridor _corridor)
    {
        _corridor.transform.parent = partitionFolder.transform;
        corridorList.Add(_corridor);
        _corridor.gameObject.name = "corridor_" + corridorList.Count;
    }

    public void FindClosestEdge(BSPNode _nodeA, BSPNode _nodeB, out GameObject _closestA, out GameObject _closestB)
    {
        List<BSPNode> roomsA = _nodeA.GetAllLeafNodes();
        List<BSPNode> roomsB = _nodeB.GetAllLeafNodes();

        GameObject closestA = _nodeA.GetData();
        GameObject closestB = _nodeB.GetData();

        float dist = 999f;
        foreach (BSPNode roomA in roomsA)
        {
            foreach (BSPNode roomB in roomsB)
            {
                float newDist = Vector3.Distance(roomA.GetData().transform.position, roomB.GetData().transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    closestA = roomA.GetData();
                    closestB = roomB.GetData();
                }
            }
        }
        _closestA = closestA;
        _closestB = closestB;
    }

}
