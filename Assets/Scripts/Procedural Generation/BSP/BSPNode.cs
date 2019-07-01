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
        get { return GetLeftChild() == null; }
    }

    public bool IsRoot
    {
        get { return parent == null; }
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

}
