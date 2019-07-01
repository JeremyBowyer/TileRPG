using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPConnectionHandler
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

    public BSPConnectionHandler(GameObject _folder, BSPController _controller)
    {
        controller = _controller;
        folder = _folder;
        connections = new List<BSPConnection>();
        pathRenderers = new List<LineRenderer>();
    }

    public void ConnectRooms(BSPRoom _roomA, BSPRoom _roomB)
    {
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

        BSPCorridor corridor = roomA.GetData().AddComponent<BSPCorridor>();
        controller.AddRoom(corridor);

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
                points.Add(pos + Vector3.up * 0.1f);
                pos.x -= 1;
            }
        }
        else
        {
            while (pos.x < _roomB.GetRoundedCenter().x)
            {
                PlaceFloor(pos, _corridor);
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
                points.Add(pos + Vector3.up * 0.1f);
                pos.z -= 1;
            }
        }
        else
        {
            while (pos.z < _roomB.GetRoundedCenter().z)
            {
                PlaceFloor(pos, _corridor);
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
        floor.transform.parent = folder.transform;
        _corridor.AddEdgeFloor(floor);
    }
}
