using UnityEngine;
using System.Collections;

public class scTransitiveConnect {

	private Hashtable connectionTable;
	
	public scTransitiveConnect(){
		connectionTable = new Hashtable();
	}
	
	public void addConnection(GameObject _room1,GameObject _room2){
		if (connectionTable.Contains(_room1)){
			ArrayList room1List =(ArrayList) connectionTable[_room1];
			
			if (!room1List.Contains(_room2)){
				room1List.Add(_room2);	
			}
			
			if (!room1List.Contains(_room1)){
				room1List.Add(_room1);	
			}
			
			if (connectionTable.Contains(_room2)){
				ArrayList room2List = (ArrayList) connectionTable[_room2];
				
				foreach(GameObject aItem in room2List){
					if (!room1List.Contains(aItem)){
						room1List.Add(aItem);
					}
				}
				connectionTable[_room2] = _room1;
			}else{
				connectionTable.Add(_room2,room1List);	
			}
			
			
			connectionTable[_room1] = room1List;

			
			foreach(GameObject aObj in room1List){
				if (!connectionTable.Contains(aObj)){
					connectionTable.Add(aObj,room1List);	
				}else{
					connectionTable[aObj] = room1List;	
				}
			}
			
		}else{
			ArrayList room1List = new ArrayList();
			
			room1List.Add(_room2);
			room1List.Add(_room1);
			
			if (connectionTable.Contains(_room2)){
				ArrayList room2List = (ArrayList) connectionTable[_room2];
				foreach(GameObject aItem in room2List){
					if (aItem != _room1){
						if (!room1List.Contains(aItem)){
							room1List.Add(aItem);
						}
					}
				}
				connectionTable[_room2] = _room1;
			}else{
				connectionTable.Add(_room2,room1List);	
			}
			
			connectionTable.Add(_room1,room1List);
			
		}
	}
	
	public void findNearestRooms(GameObject _room1, GameObject _room2, out GameObject _oRoom1, out GameObject _oRoom2){
		
		ArrayList room1List;
		ArrayList room2List;
		
		_oRoom1 = _room1;
		_oRoom2 = _room2;
		
		if (!connectionTable.Contains(_room1) && !connectionTable.Contains(_room2)){			
			return;
		}
		
		//float distance = 100000;
		
		//only room1 already exists in the table
		if (connectionTable.Contains(_room1) && !connectionTable.Contains(_room2)){
			room1List = (ArrayList) connectionTable[_room1];
			
			float distance = 100000;
			
			foreach(GameObject aRoom in room1List){
				float thisDist = Vector3.Distance(aRoom.transform.position,_room2.transform.position);
				
				if (aRoom == _room2){
					continue;	
				}
				
				if (thisDist < distance){
					distance = thisDist;
					_oRoom1 = aRoom;
					_oRoom2 = _room2;
				}
			}
			return;	
		}
		
		//only room 2 already exists in the table
		if (!connectionTable.Contains(_room1) && connectionTable.Contains(_room2)){
			room2List = (ArrayList) connectionTable[_room2];
			
			float distance = 100000;
			
			foreach(GameObject aRoom in room2List){
				float thisDist = Vector3.Distance(aRoom.transform.position,_room1.transform.position);
				
				if (aRoom == _room1){
					continue;	
				}
				
				if (thisDist < distance){
					distance = thisDist;
					_oRoom1 = _room1;
					_oRoom2 = aRoom;
				}
			}
			return;
		}
		
		if (connectionTable.Contains(_room1) && connectionTable.Contains(_room2)){
			room1List = (ArrayList) connectionTable[_room1];
			room2List = (ArrayList) connectionTable[_room2];

			foreach(GameObject aRoom in room1List){
				float distance = 100000;
				foreach(GameObject aRoom1 in room2List){
					
					if (aRoom == aRoom1){
						continue;	
					}
					

					float thisDist = Vector3.Distance(aRoom.transform.position,aRoom1.transform.position);
					//Debug.Log("current Distance: " + thisDist);
					if (thisDist < distance){
						distance = thisDist;
						_oRoom1 = aRoom;
						_oRoom2 = aRoom1;
					}
				}
			}
			return;
		}
		
		Debug.Log("error");
		
	}
	
	private void printList(ArrayList _list){
		string list;
		list = "List: ";
		
		foreach(GameObject aObj in _list){
			list += ""+aObj.GetInstanceID() + ", ";
		}
	}
	
	public void printMap(){
		foreach(GameObject aObj in connectionTable.Keys){
			ArrayList theList = (ArrayList) connectionTable[aObj];
			
			printList(theList);
		}
	}
	
	public bool checkConnected(GameObject _room0, GameObject _room1){
		if (connectionTable.Contains(_room0)){
			ArrayList theConnections = (ArrayList) connectionTable[_room0];
			
			if (theConnections.Contains(_room1)){
				return true;	
			}
		}
		
		if(connectionTable.Contains(_room1)){
			ArrayList theConnections = (ArrayList) connectionTable[_room1];
			
			return theConnections.Contains(_room0);
		}
		
		return false;
	}
	
	
}
