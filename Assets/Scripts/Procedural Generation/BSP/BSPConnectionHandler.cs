using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPConnectionHandler : MonoBehaviour
{
    public struct BSPConnection : IEquatable<BSPConnection>
    {
        public BSPRoom roomA;
        public BSPRoom roomB;

        public BSPConnection(BSPRoom _roomA, BSPRoom _roomB)
        {
            roomA = _roomA;
            roomB = _roomB;
        }

        public bool Equals(BSPConnection other)
        {
            if (roomA == other.roomA && roomB == other.roomB || roomA == other.roomB && roomB == other.roomA)
                return true;

            return false;
        }
    }
    private List<BSPConnection> connections;
    private List<LineRenderer> pathRenderers;
    private GameObject folder;
    private BSPController controller;

    public void Init(GameObject _folder, BSPController _controller)
    {
        controller = _controller;
        folder = _folder;
        connections = new List<BSPConnection>();
        pathRenderers = new List<LineRenderer>();
    }

    public void ConnectRooms(BSPRoom _roomA, BSPRoom _roomB)
    {
        if (_roomA == null)
            Debug.Log("roomA");

        if (_roomB == null)
            Debug.Log("roomB");

        BSPConnection connection = new BSPConnection(_roomA, _roomB);
        AddConnection(connection);
    }

    public void AddConnection(BSPConnection _connection)
    {
        if (!connections.Contains(_connection))
            connections.Add(_connection);
    }

    public void ConnectNodes(BSPNode _nodeA, BSPNode _nodeB)
    {
        List<BSPRoom> roomsA = _nodeA.GetAllRooms();
        List<BSPRoom> roomsB = _nodeB.GetAllRooms();

        BSPRoom closestA = _nodeA.GetRoom();
        BSPRoom closestB = _nodeB.GetRoom();

        float dist = 999f;
        foreach(BSPRoom roomA in roomsA)
        {
            foreach(BSPRoom roomB in roomsB)
            {
                float newDist = Vector3.Distance(roomA.GetData().transform.position, roomB.GetData().transform.position);
                if(newDist < dist)
                {
                    dist = newDist;
                    closestA = roomA;
                    closestB = roomB;
                }
            }
        }

        BSPConnection connection = new BSPConnection(closestA, closestB);
        AddConnection(connection);
    }

    public List<BSPRoom> GetConnectingRooms(BSPRoom _room)
    {
        List<BSPRoom> rooms = new List<BSPRoom>();
        foreach(BSPConnection connection in connections)
        {
            if(_room == connection.roomA)
            {
                rooms.Add(connection.roomB);
            } else if(_room == connection.roomB)
            {
                rooms.Add(connection.roomA);
            }
        }
        return rooms;
    }

    public void DisplayConnections()
    {
        for(int i=0; i< connections.Count; i++)
        {
            BSPConnection connection = connections[i];
            GameObject linerendererGO = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/LineRenderer"));
            LineRenderer pathLine = linerendererGO.GetComponent<LineRenderer>();
            pathLine.positionCount = 2;
            pathRenderers.Add(pathLine);
            Vector3[] points = new Vector3[2] { connection.roomA.GetData().transform.position + Vector3.up, connection.roomB.GetData().transform.position + Vector3.up };
            pathLine.SetPositions(points);
            Debug.Log(connection.roomA.GetData().name + " to " + connection.roomB.GetData().name);
        }
    }

    public void HideConnections()
    {
        foreach(LineRenderer path in pathRenderers)
        {
            path.positionCount = 0;
        }
    }

    public void BuildAllCorridors(bool show = false)
    {
        foreach(BSPConnection connection in connections)
        {
            BuildCorridor(connection);
        }
    }

    public void BuildCorridor(BSPConnection _connection, bool show = false)
    {
        BSPRoom roomA = _connection.roomA;
        BSPRoom roomB = _connection.roomB;

        
        GameObject corridorGO = new GameObject("corridor");
        corridorGO.transform.position = roomA.transform.position;
        BSPCorridor corridor = corridorGO.AddComponent<BSPCorridor>();
        controller.AddCorridor(corridor);

        Vector3 pos = roomA.GetRoundedCenter();
        Vector3 endPos = roomB.GetRoundedCenter();

        bool movingVertically = Mathf.Abs(roomA.GetRoundedCenter().z - roomB.GetRoundedCenter().z) > Mathf.Abs(roomA.GetRoundedCenter().x - roomB.GetRoundedCenter().x);

        // If moving vertically and starting room center is outside the X bounds of the target room
        // OR
        // Moving horizontally and target room center is outside the Z bounds of the starting room
        // THEN move horizontally first, then vertically.
        if((movingVertically && (pos.x > roomB.GetRight() || pos.x < roomB.GetLeft())) ||
           (!movingVertically && (endPos.z > roomA.GetTop() || endPos.z < roomA.GetBottom())))
        {
            BuildHorizontally(roomA, roomB, pos, out pos, corridor, show);
            BuildVertically(roomA, roomB, pos, out pos, corridor, show);
        }
        // If moving horizontally and starting room center is outside the Z bounds of the target room
        // OR
        // Moving vertically and target room center is outside the x bounds of the starting room
        // THEN move vertically first, then horizontally.
        else if ((!movingVertically && (pos.z > roomB.GetTop() || pos.z < roomB.GetBottom())) ||
                 (movingVertically && (endPos.x > roomA.GetRight() || endPos.x < roomA.GetLeft())))
        {
            BuildVertically(roomA, roomB, pos, out pos, corridor, show);
            BuildHorizontally(roomA, roomB, pos, out pos, corridor, show);
        }
        // Otherwise, order doesn't matter.
        else
        {
            BuildHorizontally(roomA, roomB, pos, out pos, corridor, show);
            BuildVertically(roomA, roomB, pos, out pos, corridor, show);
        }
    }

    public void BuildHorizontally(BSPRoom _roomA, BSPRoom _roomB, Vector3 pos, out Vector3 outPos, BSPCorridor _corridor, bool show = false)
    {
        List<Vector3> points = new List<Vector3>();
        bool movingLeft = _roomA.transform.position.x > _roomB.transform.position.x;
        if (movingLeft)
        {
            while (pos.x > _roomB.GetRoundedCenter().x)
            {
                PlaceFloor(pos, _corridor);
                PlaceWalls(pos, _corridor, new Vector3[] { Vector3.forward, Vector3.back });
                DestroyWall(pos - Vector3.right * 0.5f, new Vector3[] { Vector3.left, Vector3.right });
                points.Add(pos + Vector3.up * 0.1f);
                pos.x -= 1;
            }
        }
        else
        {
            while (pos.x < _roomB.GetRoundedCenter().x)
            {
                PlaceFloor(pos, _corridor);
                PlaceWalls(pos, _corridor, new Vector3[] { Vector3.forward, Vector3.back });
                DestroyWall(pos + Vector3.right * 0.5f, new Vector3[] { Vector3.left, Vector3.right });
                points.Add(pos + Vector3.up * 0.1f);
                pos.x += 1;
            }
        }

        // Show line renderer, if requested
        if (show)
        {
            GameObject linerendererGO = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/LineRenderer"));
            linerendererGO.name = _roomA.name + " to " + _roomB.name;
            LineRenderer pathLine = linerendererGO.GetComponent<LineRenderer>();
            pathLine.positionCount = points.Count;
            pathLine.SetPositions(points.ToArray());
            pathLine.material.color = Color.green;
        }

        outPos = pos;
    }

    public void BuildVertically(BSPRoom _roomA, BSPRoom _roomB, Vector3 pos, out Vector3 outPos, BSPCorridor _corridor, bool show = false)
    {
        List<Vector3> points = new List<Vector3>();
        bool movingUp = _roomA.transform.position.z > _roomB.transform.position.z;
        if (movingUp)
        {
            while (pos.z > _roomB.GetRoundedCenter().z)
            {
                PlaceFloor(pos, _corridor);
                PlaceWalls(pos, _corridor, new Vector3[] { Vector3.left, Vector3.right });
                DestroyWall(pos - Vector3.forward * 0.5f, new Vector3[] { Vector3.forward, Vector3.back });
                points.Add(pos + Vector3.up * 0.1f);
                pos.z -= 1;
            }
        }
        else
        {
            while (pos.z < _roomB.GetRoundedCenter().z)
            {
                PlaceFloor(pos, _corridor);
                PlaceWalls(pos, _corridor, new Vector3[] { Vector3.left, Vector3.right });
                DestroyWall(pos + Vector3.forward * 0.5f, new Vector3[] { Vector3.forward, Vector3.back });
                points.Add(pos + Vector3.up * 0.1f);
                pos.z += 1;
            }
        }

        // Show line renderer, if requested
        if (show)
        {
            GameObject linerendererGO = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/LineRenderer"));
            linerendererGO.name = _roomA.name + " to " + _roomB.name;
            LineRenderer pathLine = linerendererGO.GetComponent<LineRenderer>();
            pathLine.positionCount = points.Count;
            pathLine.SetPositions(points.ToArray());
            pathLine.material.color = Color.green;
        }

        outPos = pos;
    }

    public void DestroyWall(Vector3 _pos, Vector3[] directions)
    {
        // Check for existing floor.
        Collider[] cols = Physics.OverlapSphere(_pos, 0.4f);
        bool foundWall = false;
        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.tag == "Wall")
                {
                    foundWall = true;
                    MapObject obj = col.gameObject.GetComponent<MapObject>();
                    if(obj != null)
                    {
                        obj.area.RemoveWall(col.gameObject);
                    }
                    Destroy(col.gameObject);
                }
            }
        }

        if (foundWall)
        {
            foreach(Vector3 direction in directions)
            {
                Vector3 floorPos = _pos + direction * 0.5f;
                Collider[] floorCols = Physics.OverlapSphere(floorPos, 0.4f);
                if (cols.Length > 0)
                {
                    foreach (Collider col in floorCols)
                    {
                        if (col.tag == "Ground")
                        {
                            BSPFloor floor = col.gameObject.GetComponent<BSPFloor>();
                            if (floor == null)
                                continue;
                            floor.isDoorway = true;
                        }
                    }
                }
            }
        }

    }

    public void PlaceFloor(Vector3 _pos, BSPCorridor _corridor)
    {
        // Check for existing floor.
        Collider[] cols = Physics.OverlapSphere(_pos, 0.4f);
        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.tag == "Ground")
                {
                    return;
                }
            }
        }

        GameObject floor = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Floors/SM_Env_Tiles_07"));
        float xMod = floor.GetComponent<BoxCollider>().bounds.size.x;
        floor.transform.localScale = new Vector3(floor.transform.localScale.x / xMod, floor.transform.localScale.y / xMod, floor.transform.localScale.z / xMod);

        // Place floor, adjusting for floor offset
        floor.transform.position = _pos + new Vector3(0.5f, 0f, -0.5f);
        floor.transform.parent = _corridor.transform;
        _corridor.AddFloor(floor);
        _corridor.AddEdgeFloor(floor);
    }

    public void PlaceWalls(Vector3 _pos, BSPCorridor _corridor, Vector3[] directions)
    {
        foreach (Vector3 direction in directions)
        {
            // Check for existing floor.
            bool foundFloor = false;
            Collider[] cols = Physics.OverlapSphere(_pos + direction * 1f, 0.4f);
            if (cols.Length > 0)
            {
                foreach (Collider col in cols)
                {
                    if (col.tag == "Ground")
                    {
                        foundFloor = true;
                    }
                }
            }

            // Check for existing Wall.
            bool foundWall = false;
            Collider[] wallCols = Physics.OverlapSphere(_pos + direction * 0.5f, 0.4f);
            if (wallCols.Length > 0)
            {
                foreach (Collider col in wallCols)
                {
                    if (col.tag == "Wall")
                    {
                        foundWall = true;
                    }
                }
            }

            // If no floor was found, spawn a wall
            if (!foundFloor && !foundWall)
            {
                GameObject wall = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Walls/SM_Env_Wall_01_DoubleSided"));
                float xMod = wall.GetComponent<BoxCollider>().bounds.size.x;
                wall.transform.localScale = new Vector3(wall.transform.localScale.x / xMod, wall.transform.localScale.y / xMod, wall.transform.localScale.z / xMod) * 1.1f;

                // Place floor, adjusting for floor offset
                wall.transform.position = _pos + direction * 0.5f + Vector3.right * 0.5f;

                if (direction == Vector3.left)
                {
                    wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                    wall.transform.position += Vector3.back * 0.5f + Vector3.left * 0.5f;
                }
                else if (direction == Vector3.right)
                {
                    wall.transform.Rotate(new Vector3(0f, -90f, 0f));
                    wall.transform.position += Vector3.forward * 0.5f + Vector3.left * 0.5f;
                }
                else if (direction == Vector3.forward)
                {
                    wall.transform.Rotate(new Vector3(0f, 180f, 0f));
                    wall.transform.position += Vector3.left * 1f;
                }

                BSPWall wallObj = wall.AddComponent<BSPWall>();
                wallObj.facingDirection = direction;
                wall.transform.parent = _corridor.transform;
                _corridor.AddWall(wall);
            }

        }
    }

}
