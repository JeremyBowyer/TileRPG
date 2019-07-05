using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSPRoom : BSPArea
{
    public BSPNode Node { get; set; }

    private float xSize;
    private float ySize;
    private float zSize;

    private int buffer = 1;

	// Use this for initialization
	public void Init () {
        // Get initial size, so we know what % to scale down, based on buffer size
		xSize = GetComponent<BoxCollider>().bounds.size.x;
		ySize = GetComponent<BoxCollider>().bounds.size.y;
		zSize = GetComponent<BoxCollider>().bounds.size.z;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * ((xSize - buffer*2) / xSize), gameObject.transform.localScale.y, gameObject.transform.localScale.z * ((zSize - buffer*2) / zSize));

        // Get scaled down size
        xSize = Mathf.Round(GetComponent<BoxCollider>().bounds.size.x);
        ySize = Mathf.Round(GetComponent<BoxCollider>().bounds.size.y);
        zSize = Mathf.Round(GetComponent<BoxCollider>().bounds.size.z);

        Destroy(GetComponent<MeshRenderer>());

        //AddWalls();
        AddFloor();
	}
	
	private void AddFloor(){
        Vector3 midPoint = transform.position;
        
		for (int i = 0; i < xSize; i++){
			for (int j = 0; j < zSize; j++){
				GameObject floor = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Map/Floors/SM_Env_Tiles_07"));
                float xMod = floor.GetComponent<BoxCollider>().bounds.size.x;
                floor.transform.localScale = new Vector3(floor.transform.localScale.x / xMod, floor.transform.localScale.y / xMod, floor.transform.localScale.z / xMod);

                floor.transform.position = midPoint - new Vector3((xSize/2)-1, 0f, zSize/2) + new Vector3(i, 0f, j);
                floor.name = "(" + i + "," + j + ")";
				floor.transform.parent = transform;
                if(i == 0 || j == 0 || i >= xSize - 1 || j >= zSize - 1)
                {
                    AddEdgeFloor(floor);
                }

			}
		}
        
        /*
        GameObject floor = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Floors/SM_Env_Tiles_07"));
        float xMod = floor.GetComponent<BoxCollider>().bounds.size.x;
        floor.transform.localScale = new Vector3(floor.transform.localScale.x / xMod * xSize, floor.transform.localScale.y / xMod, floor.transform.localScale.z / xMod * zSize);

        floor.transform.position = midPoint + new Vector3(xSize / 2, 0f, -(zSize / 2));

        floor.transform.parent = transform;
        */
    }

    public void AddProps()
    {
        /*
        foreach (GameObject wall in walls)
        {
            float rnd = Random.value;
            if(rnd < 0.1f)
            {
                GameObject torch = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/Wall_Torch"));
                torch.transform.position = wall.transform.Find("TorchAnchor").transform.position;
                torch.transform.parent = wall.transform.Find("TorchAnchor").transform;
                torch.transform.localScale = Vector3.one * 2f;

                BSPWall wallObj = wall.GetComponent<BSPWall>();

                if (wallObj.facingDirection == Vector3.left)
                {
                    torch.transform.Rotate(new Vector3(0f, 90f, 0f));
                }
                else if (wallObj.facingDirection == Vector3.right)
                {
                    torch.transform.Rotate(new Vector3(0f, -90f, 0f));                }
                else if (wallObj.facingDirection == Vector3.forward)
                {
                    torch.transform.Rotate(new Vector3(0f, 180f, 0f));
                }
            }
        }
        */
        GameObject chandelier = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Map/Props/Chandelier"));
        chandelier.transform.position = GetRoundedCenter() + Vector3.up * 5f;
        chandelier.transform.parent = transform;
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

    public GameObject GetData()
    {
        return gameObject;
    }		
}
