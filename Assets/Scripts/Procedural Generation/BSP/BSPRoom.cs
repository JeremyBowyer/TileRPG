using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BSPRoom : BSPArea
{
    public BSPNode Node { get; set; }
    public GameObject[,] floorGrid;
    protected string[] floorPropPrefabs;
    protected System.Random rnd;

	// Use this for initialization
	public override void Init (int _id, Vector2 _bufferBounds) {

        base.Init(_id, _bufferBounds);
        floorPropPrefabs = new string[] { "Containers_01", "Keg" };
        Destroy(GetComponent<MeshRenderer>());
        floorGrid = new GameObject[Mathf.RoundToInt(xSize), Mathf.RoundToInt(zSize)];

        rnd = new System.Random(Id);

        //AddWalls();
        AddFloor();
	}

    public void AddWalls()
    {
        foreach (GameObject floorGO in edgeFloors)
        {
            Vector3 midPoint = floorGO.transform.position + Vector3.left * 0.5f + Vector3.forward * 0.5f;

            BSPFloor floor = floorGO.GetComponent<BSPFloor>();
            if (floor == null)
                continue;

            int x = floor.x;
            int y = floor.y;

            // Determine Directions
            List<Vector3> directions = new List<Vector3>();
            if (x == 0)
                directions.Add(Vector3.left);

            if (x + 1 == Mathf.RoundToInt(xSize))
                directions.Add(Vector3.right);

            if (y == 0)
                directions.Add(Vector3.back);

            if (y + 1 == Mathf.RoundToInt(zSize))
                directions.Add(Vector3.forward);

            foreach (Vector3 direction in directions)
            {
                GameObject wall = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Walls/SM_Env_Wall_01_DoubleSided"));
                float xMod = wall.GetComponent<BoxCollider>().bounds.size.x;
                wall.transform.localScale = new Vector3(wall.transform.localScale.x / xMod, wall.transform.localScale.y / xMod, wall.transform.localScale.z / xMod) * 1.1f;

                // Place floor, adjusting for floor offset
                wall.transform.position = midPoint + direction * 0.49f + Vector3.right * 0.5f;

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

                wall.transform.parent = floorGO.transform.parent;
                BSPWall wallObj = wall.AddComponent<BSPWall>();
                wallObj.facingDirection = direction;
                AddWall(wall);
                floor.walls.Add(wall);
            }
        }
    }

    private void AddFloor() {
        Vector3 midPoint = transform.position;

        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < zSize; j++) {
                GameObject floorGO = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Floors/SM_Env_Tiles_07"));
                BSPFloor floor = floorGO.AddComponent<BSPFloor>();
                floor.x = i;
                floor.y = j;
                floor.isCorner = (i == 0 && j == zSize - 1) || (j == 0 && i == xSize-1) || (i == xSize - 1 && j == zSize-1) || (i == 0 && j == 0);

                float xMod = floorGO.GetComponent<BoxCollider>().bounds.size.x;
                floorGO.transform.localScale = new Vector3(floorGO.transform.localScale.x / xMod, floorGO.transform.localScale.y / xMod, floorGO.transform.localScale.z / xMod);

                floorGO.transform.position = midPoint - new Vector3((xSize / 2) - 1, 0f, zSize / 2) + new Vector3(i, 0f, j);
                floorGO.name = "(" + i + "," + j + ")";
                floorGO.transform.parent = transform;
                AddFloor(floorGO);
                floorGrid[i, j] = floorGO;

                if (floor.isCorner)
                    AddCornerFloor(floorGO);
            }
        }

        // Add edge floors in order, around the edge of the room
        // Bottom right to top right
        for (int i = 0; i < Mathf.RoundToInt(zSize); i++)
        {
            AddEdgeFloor(floorGrid[0, i]);
        }

        // Top right to top left
        for (int i = 1; i < xSize; i++)
        {
            AddEdgeFloor(floorGrid[i, Mathf.RoundToInt(zSize)-1]);
        }

        // Top left to bottom left
        for (int i = Mathf.RoundToInt(zSize)-1; i >= 0; i--)
        {
            AddEdgeFloor(floorGrid[Mathf.RoundToInt(xSize)-1, i]);
        }

        // Bottom left to bottom right
        for (int i = Mathf.RoundToInt(xSize)-1; i > 0; i--)
        {
            AddEdgeFloor(floorGrid[i, 0]);
        }
    }

    public List<GameObject> GetNeighborFloors(GameObject _floor)
    {
        List<GameObject> neighbors = new List<GameObject>();

        BSPFloor floor = _floor.GetComponent<BSPFloor>();

        int x = floor.x;
        int y = floor.y;

        for(int i=-1; i<2; i++)
        {
            for(int j=-1; j<2; j++)
            {
                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX < 0 || neighborY < 0 || neighborX > xSize || neighborY > zSize)
                    continue;

                neighbors.Add(floorGrid[neighborX, neighborY]);

            }
        }
        return neighbors;
    }

    public override void AddProps()
    {
        if(xSize >= 7 && zSize >= 7)
        {
            AddChandelier();
        }

        AddRug();
        AddCornerFloorProps();
        AddEdgeFloorProps();
        AddFloorProps();
    }
    
    public void AddFloorProps()
    {
        int cnt = Mathf.RoundToInt(floors.Count / 15f);

        for (int i=0; i<cnt; i++)
        {
            int floorIdx = rnd.Next(0, floors.Count);
            int propIdx = rnd.Next(0, floorPropPrefabs.Length);

            GameObject floor = floors[floorIdx];
            BSPFloor floorObj = floor.GetComponent<BSPFloor>();
            if (floorObj.prop != null || floorObj.isDoorway)
                continue;
            GameObject prop = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/"+floorPropPrefabs[propIdx]));

            prop.transform.position = floor.transform.Find("AnchorPoint").transform.position;
            floorObj.prop = prop;

            AddProp(prop);
        }
    }

    public void AddChandelier()
    {
        GameObject chandelier = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/Chandelier"));
        chandelier.transform.position = transform.position + Vector3.up * 5f;
        chandelier.transform.parent = transform;
        AddProp(chandelier);
    }

    public void AddCornerFloorProps()
    {
        foreach(GameObject cornerFloor in cornerFloors)
        {
            BSPFloor floorObj = cornerFloor.GetComponent<BSPFloor>();
            if(!floorObj.isDoorway)
                AddCandlestand(cornerFloor);
        }
    }

    public void AddCandlestand(GameObject _floor)
    {
        BSPFloor floorObj = _floor.GetComponent<BSPFloor>();

        if (floorObj.prop != null)
            return;

        GameObject candlestand = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/CandleStand"));
        candlestand.transform.position = _floor.transform.Find("AnchorPoint").transform.position;
        candlestand.transform.parent = transform;
        floorObj.prop = candlestand;
        AddProp(candlestand);
    }

    public void AddEdgeFloorProps()
    {
        int cnt = Mathf.RoundToInt(edgeFloors.Count / 10f);
        
        for(int i=0; i<cnt; i++)
        {
            // Determine list of eligible floors
            List<GameObject> eligibleFloors = new List<GameObject>();
            foreach(GameObject floor in edgeFloors)
            {
                BSPFloor floorObj = floor.GetComponent<BSPFloor>();

                if (floorObj.prop != null || floorObj.isDoorway || floorObj.isCorner)
                    continue;
                eligibleFloors.Add(floor);
            }

            if (eligibleFloors.Count == 0)
                continue;
            int span = rnd.Next(1, 3);
            int floorIdx = rnd.Next(0, eligibleFloors.Count);

            AddBookCase(eligibleFloors[floorIdx], span);
        }

    }

    public void AddBookCase(GameObject floor, int span)
    {
        // Add String of bookcases, recursively

        // Get floor and wall data
        BSPFloor floorObj = floor.GetComponent<BSPFloor>();
        BSPWall wallObj = floorObj.walls[0].GetComponent<BSPWall>();

        // Load random bookcase
        string[] bookcasePrefabs = new string[] { "Bookcase_01", "Bookcase_02" };
        int bookCaseIdx = rnd.Next(0, bookcasePrefabs.Length);
        GameObject bookcase = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/"+bookcasePrefabs[bookCaseIdx]));

        // Place bookcase
        bookcase.transform.position = floor.transform.Find("AnchorPoint").transform.position;

        // Rotate bookcase
        Vector3 direction = wallObj.facingDirection;
        if (direction == Vector3.left)
        {
            bookcase.transform.Rotate(new Vector3(0f, 90f, 0f));
        }
        else if (direction == Vector3.right)
        {
            bookcase.transform.Rotate(new Vector3(0f, -90f, 0f));
        }
        else if (direction == Vector3.forward)
        {
            bookcase.transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        bookcase.transform.parent = transform;
        AddProp(bookcase);

        BSPFloor thisFloor = floor.GetComponent<BSPFloor>();
        thisFloor.prop = bookcase;

        // Check for end condition
        if (span - 1 <= 0)
            return;

        int floorIdx = edgeFloors.IndexOf(floor);

        BSPFloor nextFloor, previousFloor;
        if (floorIdx > 0)
        {
            bool validFloor = false;
            bool inLine = false;
            previousFloor = edgeFloors[floorIdx - 1].GetComponent<BSPFloor>();

            if (previousFloor.prop == null && !previousFloor.isDoorway && !previousFloor.isCorner)
                validFloor = true;

            if (previousFloor.x == thisFloor.x && Math.Abs(previousFloor.y - thisFloor.y) == 1)
                inLine = true;

            if (previousFloor.y == thisFloor.y && Math.Abs(previousFloor.x - thisFloor.x) == 1)
                inLine = true;

            if (validFloor && inLine)
            {
                AddBookCase(previousFloor.gameObject, span - 1);
                return;
            }
        }

        if (floorIdx < edgeFloors.Count - 1)
        {
            bool validFloor = false;
            bool inLine = false;
            nextFloor = edgeFloors[floorIdx + 1].GetComponent<BSPFloor>();

            if (nextFloor.prop == null && !nextFloor.isDoorway && !nextFloor.isCorner)
                validFloor = true;

            if (nextFloor.x == thisFloor.x && Math.Abs(nextFloor.y - thisFloor.y) == 1)
                inLine = true;

            if (nextFloor.y == thisFloor.y && Math.Abs(nextFloor.x - thisFloor.x) == 1)
                inLine = true;

            if (validFloor && inLine)
            {
                AddBookCase(nextFloor.gameObject, span - 1);
                return;
            }
        }


    }

    public void AddRug()
    {
        if((xSize > 7 && zSize > 4) || (xSize > 4 && zSize > 7))
        {
            GameObject fatrug = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/FatRug"));
            if (zSize > xSize)
                fatrug.transform.Rotate(new Vector3(0f, 90f, 0f));

            fatrug.transform.position = transform.position + Vector3.up * 0.02f;
            fatrug.transform.parent = transform;

            AddProp(fatrug);
        } else if ((xSize > 8 && zSize > 3) || (xSize > 3 && zSize > 8))
        {
            GameObject longrug = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/LongRug"));
            if (zSize > xSize)
                longrug.transform.Rotate(new Vector3(0f, 90f, 0f));

            longrug.transform.position = transform.position + Vector3.up * 0.02f;
            longrug.transform.parent = transform;

            AddProp(longrug);
        }
    }

    public Vector3 GetRoundedCenter()
    {
        float roundX;

        if(xSize % 2 == 0)
        {
            roundX = transform.position.x + 0.5f;
        }
        else
        {
            roundX = transform.position.x;
        }

        float roundZ;
        if(zSize % 2 == 0)
        {
            roundZ = transform.position.z + 0.5f;
        }
        else
        {
            roundZ = transform.position.z;
        }

        return new Vector3(roundX, transform.position.y, roundZ);
    }

    public GameObject GetMiddleFloor()
    {
        Collider[] cols = Physics.OverlapSphere(GetRoundedCenter(), 0.4f);
        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                if (col.tag == "Ground")
                {
                    return col.gameObject;
                }
            }
        }
        return null;
    }

    public float GetTop()
    {
        return transform.position.z + zSize / 2;
    }

    public float GetBottom()
    {
        return transform.position.z - zSize / 2;
    }

    public float GetLeft()
    {
        return transform.position.x - xSize / 2;
    }

    public float GetRight()
    {
        return transform.position.x + xSize / 2;
    }

    public float GetXSize()
    {
        return xSize;
    }

    public float GetZSize()
    {
        return zSize;
    }

    public GameObject GetData()
    {
        return gameObject;
    }		
}
