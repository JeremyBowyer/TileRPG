using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BSPNode {
	
	private GameObject data;
	private BSPNode leftChild;
	private BSPNode rightChild;
	private BSPNode parent;
    private BSPRoom room;
    private List<BSPRoom> allRooms;

    public bool IsLeaf
    {
        get { return GetLeftChild() == null && GetRightChild() == null; }
    }

    public bool IsRoot
    {
        get { return parent == null; }
    }

    public bool SiblingIsLeaf
    {
        get { return GetSibling() != null && GetSibling().IsLeaf; }
    }

    /// <summary>
    /// Returns a Vector2 of the X and Y size of the box collider.
    /// </summary>
    public Vector2 GetSize()
    {
        BoxCollider box = data.GetComponent<BoxCollider>();
        return new Vector2(box.bounds.size.x, box.bounds.size.z);
    }

    /// <summary>
    /// Returns an array of 2 float arrays, one for each dimension (x and y).
    /// Each array returns the min and max position for that dimension.
    /// </summary>
    public float[][] GetBounds()
    {
        Vector3 pos = data.transform.position;
        Vector2 size = GetSize();

        float[][] bounds = new float[2][];
        bounds[0] = new float[2] { pos.x - size.x / 2, pos.x + size.x / 2};
        bounds[1] = new float[2] { pos.y - size.y / 2, pos.y + size.y / 2 };
        return bounds;
    }

    /// <summary>
    /// Returns a Vector2 containing 2 floats between 0f and 1f.
    /// The values reprents the position of this node relative to the root node.
    /// </summary>
    public Vector2 GetRelativePosition()
    {
        BSPNode root = GetRoot();
        Vector2 rootSize = root.GetSize();

        Vector3 pos = data.transform.position;

        float[][] bounds = root.GetBounds();

        float xPos = (pos.x - bounds[0][0]) / rootSize.x;
        float yPos = (pos.z - bounds[1][0]) / rootSize.y;

        return new Vector2(xPos, yPos);
    }

    public BSPNode(GameObject _partion, BSPNode _parent){
		data = _partion;
		parent = _parent;
	}
	
	public BSPNode GetLeftChild(){
		return leftChild;	
	}
	
	public void SetLeftChild(GameObject _partition){
		leftChild = new BSPNode(_partition, this);	
	}
	
	public BSPNode GetRightChild(){
		return rightChild;	
	}
	
	public void SetRightChild(GameObject _partition){
        rightChild = new BSPNode(_partition, this);
	}
	
	public GameObject GetData(){
        if (room == null)
            return data;
        else
            return room.GetData();
	}
	
	public BSPNode GetParent(){
		return parent;	
	}

    public BSPNode GetRoot()
    {
        if (parent == null)
            return this;
        else
            return parent.GetRoot();
    }

    public void SetRoom(BSPRoom _room)
    {
        room = _room;
    }

    public BSPRoom GetRoom()
    {
        return room;
    }

    public List<BSPRoom> GetAllRooms()
    {
        allRooms = new List<BSPRoom>();
        AddRooms(this);
        return allRooms;
    }

    public void AddRooms(BSPNode _node)
    {
        if (_node == null)
            return;

        if(_node.GetRoom() != null)
        {
            allRooms.Add(_node.GetRoom());
            return;
        }

        AddRooms(_node.GetLeftChild());
        AddRooms(_node.GetRightChild());
    }

    public BSPNode GetSibling()
    {
        if (IsRoot)
            return null;

        BSPNode leftSibling = parent.GetLeftChild();
        BSPNode rightSibling = parent.GetRightChild();

        if (this == leftSibling)
            return rightSibling;

        if (this == rightSibling)
            return leftSibling;

        return null;

    }

    public BSPRoom FindLeftRoom()
    {
        if (GetLeftChild() == null)
        {
            return room;
        }
        else
        {
            return GetLeftChild().FindLeftRoom();
        }
    }

    public BSPRoom FindRightRoom()
    {
        if (GetRightChild() == null)
        {
            return room;
        }
        else
        {
            return GetRightChild().FindRightRoom();
        }
    }

    public BSPNode FindLeftLeaf()
    {
        if (GetLeftChild() == null)
        {
            return this;
        }
        else
        {
            return GetLeftChild().FindLeftLeaf();
        }
    }

    public BSPNode FindRightLeaf()
    {
        if (GetRightChild() == null)
        {
            return this;
        }
        else
        {
            return GetRightChild().FindRightLeaf();
        }
    }

}
