using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPArea : MonoBehaviour
{
    protected List<GameObject> edgeFloors { get; set; }
    protected List<GameObject> walls { get; set; }

    public void Awake()
    {
        edgeFloors = new List<GameObject>();
        walls = new List<GameObject>();
    }

    public void AddWalls()
    {
        foreach (GameObject floor in edgeFloors)
        {
            Vector3 midPoint = floor.transform.position + Vector3.left * 0.5f + Vector3.forward * 0.5f;
            // Check for neighboring floors
            foreach (Vector3 direction in new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right })
            {
                // Check for existing floor.
                bool foundFloor = false;
                Collider[] cols = Physics.OverlapSphere(midPoint + direction * 1f, 0.4f);
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

                // If no floor was found, spawn a wall
                if (!foundFloor)
                {
                    GameObject wall = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Map/Walls/SM_Env_Wall_01_DoubleSided"));
                    float xMod = wall.GetComponent<BoxCollider>().bounds.size.x;
                    wall.transform.localScale = new Vector3(wall.transform.localScale.x / xMod, wall.transform.localScale.y / xMod, wall.transform.localScale.z / xMod) * 1.1f;

                    // Place floor, adjusting for floor offset
                    wall.transform.position = midPoint + direction * 0.5f + Vector3.right * 0.5f;

                    if(direction == Vector3.left)
                    {
                        wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                        wall.transform.position += Vector3.back * 0.5f + Vector3.left * 0.5f;
                    } else if(direction == Vector3.right)
                    {
                        wall.transform.Rotate(new Vector3(0f, -90f, 0f));
                        wall.transform.position += Vector3.forward * 0.5f + Vector3.left * 0.5f;
                    } else if(direction == Vector3.forward)
                    {
                        wall.transform.Rotate(new Vector3(0f, 180f, 0f));
                        wall.transform.position += Vector3.left * 1f;
                    }

                    /*
                    if (direction == Vector3.left || direction == Vector3.right)
                    {
                        wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                        wall.transform.position += Vector3.back * 0.5f + Vector3.left * 0.5f;
                    }
                    */
                    wall.transform.parent = floor.transform.parent;
                    BSPWall wallObj = wall.AddComponent<BSPWall>();
                    wallObj.facingDirection = direction;
                    AddWall(wall);
                }
            }
        }
    }

    public void AddEdgeFloor(GameObject _edgeFloor)
    {
        edgeFloors.Add(_edgeFloor);
    }

    public void AddWall(GameObject _wall)
    {
        walls.Add(_wall);
    }
}
