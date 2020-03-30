using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPConnectionHandler : MonoBehaviour
{
    private BSPController controller;
    private GameObject doorwayPrefab;

    public void Init(BSPController _controller)
    {
        controller = _controller;
        doorwayPrefab = Resources.Load<GameObject>("Prefabs/MiniMap/MapDoorway");
    }


    public void BuildCorridor(KeepRoom roomA, KeepRoom roomB)
    {
        GameObject corridorGO = new GameObject("corridor");
        corridorGO.transform.position = roomA.transform.position;
        KeepCorridor corridor = corridorGO.AddComponent<KeepCorridor>();
        corridor.roomA = roomA;
        corridor.roomB = roomB;
        corridor.DetermineType();
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
            BuildHorizontally(roomA, roomB, pos, out pos, corridor);
            BuildVertically(roomA, roomB, pos, out pos, corridor);
        }
        // If moving horizontally and starting room center is outside the Z bounds of the target room
        // OR
        // Moving vertically and target room center is outside the x bounds of the starting room
        // THEN move vertically first, then horizontally.
        else if ((!movingVertically && (pos.z > roomB.GetTop() || pos.z < roomB.GetBottom())) ||
                 (movingVertically && (endPos.x > roomA.GetRight() || endPos.x < roomA.GetLeft())))
        {
            BuildVertically(roomA, roomB, pos, out pos, corridor);
            BuildHorizontally(roomA, roomB, pos, out pos, corridor);
        }
        // Otherwise, order doesn't matter.
        else
        {
            BuildHorizontally(roomA, roomB, pos, out pos, corridor);
            BuildVertically(roomA, roomB, pos, out pos, corridor);
        }

        // Expand Corridor
        ExpandCorridor(corridor);

        // Destroy walls around each corridor floor
        foreach (GameObject floor in corridor.floors)
        {
            Transform ap = floor.transform.Find("AnchorPoint");
            if (ap != null)
                DestroyWalls(ap.position);
        }


        // Place new "walls" (likely some sort of railing)
        foreach (GameObject floor in corridor.floors)
        {
            Transform ap = floor.transform.Find("AnchorPoint");
            if (ap != null)
                PlaceWalls(ap.position, corridor, new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right } );
        }


        corridor.AddMapIcon();
    }

    public void BuildHorizontally(KeepRoom _roomA, KeepRoom _roomB, Vector3 pos, out Vector3 outPos, KeepCorridor _corridor)
    {

        bool movingLeft = _roomA.transform.position.x > _roomB.transform.position.x;
        if (movingLeft)
        {
            while (pos.x > _roomB.GetRoundedCenter().x)
            {
                _corridor.PlaceFloor(pos);
                //PlaceWalls(pos, _corridor, new Vector3[] { Vector3.forward, Vector3.back });
                DestroyWall(pos - Vector3.right * 0.5f, new Vector3[] { Vector3.left, Vector3.right });
                pos.x -= 1;
            }
        }
        else
        {
            while (pos.x < _roomB.GetRoundedCenter().x)
            {
                _corridor.PlaceFloor(pos);
                //PlaceWalls(pos, _corridor, new Vector3[] { Vector3.forward, Vector3.back });
                DestroyWall(pos + Vector3.right * 0.5f, new Vector3[] { Vector3.left, Vector3.right });
                pos.x += 1;
            }
        }

        outPos = pos;
    }

    public void BuildVertically(KeepRoom _roomA, KeepRoom _roomB, Vector3 pos, out Vector3 outPos, KeepCorridor _corridor)
    {
        bool movingUp = _roomA.transform.position.z > _roomB.transform.position.z;
        if (movingUp)
        {
            while (pos.z > _roomB.GetRoundedCenter().z)
            {
                _corridor.PlaceFloor(pos);
                //PlaceWalls(pos, _corridor, new Vector3[] { Vector3.left, Vector3.right });
                DestroyWall(pos - Vector3.forward * 0.5f, new Vector3[] { Vector3.forward, Vector3.back });
                pos.z -= 1;
            }
        }
        else
        {
            while (pos.z < _roomB.GetRoundedCenter().z)
            {
                _corridor.PlaceFloor(pos);
                //PlaceWalls(pos, _corridor, new Vector3[] { Vector3.left, Vector3.right });
                DestroyWall(pos + Vector3.forward * 0.5f, new Vector3[] { Vector3.forward, Vector3.back });
                pos.z += 1;
            }
        }

        outPos = pos;
    }

    public void ExpandCorridor(KeepCorridor _corridor)
    {

        int floorCnt = _corridor.floors.Count;

        if (floorCnt < 2)
            return;

        for(int i=0; i<floorCnt; i++)
        {
            Transform prevFloor;
            Transform thisFloor;

            if (i == 0)
            {
                prevFloor = _corridor.floors[i + 1].transform.Find("AnchorPoint");
                thisFloor = _corridor.floors[i].transform.Find("AnchorPoint");
            }
            else
            {
                prevFloor = _corridor.floors[i - 1].transform.Find("AnchorPoint");
                thisFloor = _corridor.floors[i].transform.Find("AnchorPoint");
            }

            Vector3 heading = thisFloor.position - prevFloor.position;
            float distance = heading.magnitude;
            Vector3 direction = heading / distance;

            Vector3 expDirection;
            if(direction == Vector3.forward || direction == Vector3.back)
            {

                if(_corridor.roomB.GetMiddle().x - thisFloor.transform.position.x > 0)
                {
                    expDirection = Vector3.left;
                } else if(_corridor.roomB.GetMiddle().x - thisFloor.transform.position.x < 0)
                {
                    expDirection = Vector3.right;
                }
                else
                {
                    expDirection = CustomUtils.GetRandomBool() ? Vector3.left : Vector3.right;
                }

                for(int j=1; j<_corridor.corridorWidth; j++)
                {
                    Vector3 pos = thisFloor.position + expDirection * j;
                    if (_corridor.roomA.WithinLeftRightBounds(pos) && _corridor.roomB.WithinLeftRightBounds(pos))
                        _corridor.PlaceFloor(pos);
                }

            } else if(direction == Vector3.left || direction == Vector3.right)
            {

                if (_corridor.roomB.GetMiddle().z - thisFloor.transform.position.z > 0)
                {
                    expDirection = Vector3.forward;
                }
                else if (_corridor.roomB.GetMiddle().z - thisFloor.transform.position.z < 0)
                {
                    expDirection = Vector3.back;
                }
                else
                {
                    expDirection = CustomUtils.GetRandomBool() ? Vector3.forward : Vector3.back;
                }

                for (int j=1; j< _corridor.corridorWidth; j++)
                {
                    Vector3 pos = thisFloor.position + expDirection * j;
                    if (_corridor.roomA.WithinTopBottomBounds(pos) && _corridor.roomB.WithinTopBottomBounds(pos))
                        _corridor.PlaceFloor(pos);
                }

            }
        }
    }

    public void DestroyWalls(Vector3 _pos)
    {
        DestroyWall(_pos - Vector3.forward * 0.5f, new Vector3[] { Vector3.forward, Vector3.back });
        DestroyWall(_pos + Vector3.forward * 0.5f, new Vector3[] { Vector3.forward, Vector3.back });

        DestroyWall(_pos - Vector3.right * 0.5f, new Vector3[] { Vector3.left, Vector3.right });
        DestroyWall(_pos + Vector3.right * 0.5f, new Vector3[] { Vector3.left, Vector3.right });
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

                    GameObject go = Instantiate(doorwayPrefab);
                    go.transform.SetParent(LevelController.instance.mapHolder.transform);
                    RectTransform rect = go.GetComponent<RectTransform>();
                    go.transform.position = new Vector3(_pos.x, LevelController.instance.mapHolder.transform.position.y, _pos.z);

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
                            KeepFloor floor = col.gameObject.GetComponent<KeepFloor>();
                            if (floor == null)
                                continue;
                            floor.isDoorway = true;
                            floor.doorwayDirection = direction * -1;
                        }
                    }
                }
            }
        }

    }

    public void PlaceWalls(Vector3 _pos, KeepCorridor _corridor, Vector3[] directions)
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
                GameObject prefab = null;
                if (_corridor.secondaryWallPrefabs != null && _corridor.secondaryWallPrefabs.Length > 0 && CustomUtils.GetRandomBool(25))
                    prefab = _corridor.secondaryWallPrefabs[CustomUtils.GetRandom(0, _corridor.secondaryWallPrefabs.Length)];
                else
                    prefab = _corridor.wallPrefab;

                GameObject wall = Instantiate(prefab);
                float xMod = wall.GetComponent<BoxCollider>().bounds.size.x;
                wall.transform.localScale = new Vector3(wall.transform.localScale.x / xMod, wall.transform.localScale.y / xMod, wall.transform.localScale.z / xMod);

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

                KeepWall wallObj = wall.AddComponent<KeepWall>();
                wallObj.facingDirection = direction;
                wall.transform.parent = _corridor.transform;
                _corridor.AddWall(wall);
            }

        }
    }

}
