using UnityEngine;
using System.Collections;
/// <summary>
/// Handles the randomise splitting of BSP Partion areas
/// </summary>
public class BSPSplitHandler {
	
	private float splitMargin = 35;
    private int cnt;

	private GameObject folderObject;
	
	public BSPSplitHandler(GameObject _folder)
    {
        folderObject = _folder;
        cnt = 0;
	}
	
	public void split(GameObject _partitionSection, out GameObject _pieceA, out GameObject _pieceB)
    {
        if (_partitionSection.transform.localScale.y > _partitionSection.transform.localScale.x){
			splitNodeHorizontal(_partitionSection, out _pieceA, out _pieceB);
		}else if (_partitionSection.transform.localScale.y < _partitionSection.transform.localScale.x){
			splitNodeVertical(_partitionSection, out _pieceA, out _pieceB);
		}else{
			//randomise which way the split happens
			int choice = Random.Range(0,2);
			
			if (choice == 0){
				splitNodeVertical(_partitionSection, out _pieceA, out _pieceB);
			}else{
				splitNodeHorizontal(_partitionSection,out _pieceA, out _pieceB);
			}
		}
	}
	
	//Vertical Node Split
	private void splitNodeVertical(GameObject _partitionSection, out GameObject _pieceA, out GameObject _pieceB){
		
		float randSplitAmount = Random.Range(splitMargin,100 - splitMargin) / 100;

		//first sub area
		GameObject sectionA = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		sectionA.transform.localScale = new Vector3(Mathf.Round(_partitionSection.transform.localScale.x * randSplitAmount),
													_partitionSection.transform.localScale.y, 
													_partitionSection.transform.localScale.z);
		
		sectionA.transform.position = new Vector3(	_partitionSection.transform.position.x - _partitionSection.transform.localScale.x/2 + sectionA.transform.localScale.x/2, 
													_partitionSection.transform.position.y,
													_partitionSection.transform.position.z);

		//tidy the pieces into a folder in the hiearcy
		sectionA.transform.parent = folderObject.transform;
        sectionA.name = "Node " + ++cnt;
		_pieceA = sectionA;
		
		//second sub area
		GameObject sectionB = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		sectionB.transform.localScale = new Vector3(Mathf.Round(_partitionSection.transform.localScale.x * (1 - randSplitAmount)),
													_partitionSection.transform.localScale.y, 
													_partitionSection.transform.localScale.z);
		
		sectionB.transform.position = new Vector3(sectionA.transform.position.x + sectionA.transform.localScale.x/2 + sectionB.transform.localScale.x/2, 
													_partitionSection.transform.position.y,
													_partitionSection.transform.position.z);

		//tidy the pieces into a folder in the hiearcy
		sectionB.transform.parent = folderObject.transform;
        sectionB.name = "Node " + ++cnt;
        _pieceB = sectionB;		
	}
	
	//Horizontal Node Split
	private void splitNodeHorizontal(GameObject _partitionSection, out GameObject _pieceA, out GameObject _pieceB){
		float randSplitAmount = Random.Range(splitMargin,100 - splitMargin) / 100;
		
		//first sub area
		GameObject sectionA = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		sectionA.transform.localScale = new Vector3(_partitionSection.transform.localScale.x ,
													Mathf.Round(_partitionSection.transform.localScale.y * randSplitAmount), 
													_partitionSection.transform.localScale.z);
		
		sectionA.transform.position = new Vector3(	_partitionSection.transform.position.x, 
													_partitionSection.transform.position.y -_partitionSection.transform.localScale.y/2 + sectionA.transform.localScale.y/2,
													_partitionSection.transform.position.z);
		//sectionA.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);
		//tidy the pieces into a folder in the hiearcy
		sectionA.transform.parent = folderObject.transform;
        sectionA.name = "Node " + ++cnt;
        _pieceA = sectionA;
		
		//second sub area
		GameObject sectionB = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		sectionB.transform.localScale = new Vector3(_partitionSection.transform.localScale.x,
													Mathf.Round(_partitionSection.transform.localScale.y * (1 - randSplitAmount)), 
													_partitionSection.transform.localScale.z);
		
		sectionB.transform.position = new Vector3(	_partitionSection.transform.position.x, 
													sectionA.transform.position.y + sectionA.transform.localScale.y/2 + sectionB.transform.localScale.y/2,
													_partitionSection.transform.position.z);
		//sectionB.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);
		//tidy the pieces into a folder in the hiearcy
		sectionB.transform.parent = folderObject.transform;
        sectionB.name = "Node " + ++cnt;
        _pieceB = sectionB;
	}
}
