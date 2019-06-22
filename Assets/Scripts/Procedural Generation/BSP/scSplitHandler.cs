using UnityEngine;
using System.Collections;
/// <summary>
/// Handles the randomise splitting of BSP Partion areas
/// </summary>
public class scSplitHandler {
	
	private float splitMargine = 35;
	
	private GameObject folderObject;
	
	public scSplitHandler(){

		folderObject = new GameObject();
		folderObject.name = "BSPPartitionPieces";
		folderObject.tag = "BSPPartitionSections";
	}
	
	public void split(GameObject _partionSection, out GameObject _pieceA, out GameObject _pieceB){
		
		if (_partionSection.transform.localScale.y > _partionSection.transform.localScale.x){
			splitNodeHorizontal(_partionSection, out _pieceA, out _pieceB);
		}else if (_partionSection.transform.localScale.y < _partionSection.transform.localScale.x){
			splitNodeVertical(_partionSection, out _pieceA, out _pieceB);
		}else{
			//randomise which way the split happens
			int choice = Random.Range(0,2);
			
			if (choice == 0){
				splitNodeVertical(_partionSection, out _pieceA, out _pieceB);
			}else{
				splitNodeHorizontal(_partionSection,out _pieceA, out _pieceB);
			}
		}
	}
	
	//Vertical Node Split
	private void splitNodeVertical(GameObject _partionSection, out GameObject _pieceA, out GameObject _pieceB){
		
		float randSplitAmount = Random.Range(splitMargine,100 - splitMargine) / 100;
		
		//first sub area
		GameObject sectionA = (GameObject) GameObject.Instantiate(Resources.Load("PartionSection"));
		sectionA.transform.localScale = new Vector3(Mathf.Round(_partionSection.transform.localScale.x * randSplitAmount),
													_partionSection.transform.localScale.y, 
													_partionSection.transform.localScale.z);
		
		sectionA.transform.position = new Vector3(	_partionSection.transform.position.x - _partionSection.transform.localScale.x/2 + sectionA.transform.localScale.x/2, 
													_partionSection.transform.position.y,
													_partionSection.transform.position.z);
		sectionA.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);
		//tidy the pieces into a folder in the hiearcy
		sectionA.transform.parent = folderObject.transform;
		_pieceA = sectionA;
		
		//second sub area
		GameObject sectionB = (GameObject) GameObject.Instantiate(Resources.Load("PartionSection"));
		sectionB.transform.localScale = new Vector3(Mathf.Round(_partionSection.transform.localScale.x * (1 - randSplitAmount)),
													_partionSection.transform.localScale.y, 
													_partionSection.transform.localScale.z);
		
		sectionB.transform.position = new Vector3(sectionA.transform.position.x + sectionA.transform.localScale.x/2 + sectionB.transform.localScale.x/2, 
													_partionSection.transform.position.y,
													_partionSection.transform.position.z);
		sectionB.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);
		//tidy the pieces into a folder in the hiearcy
		sectionB.transform.parent = folderObject.transform;
		_pieceB = sectionB;
		
		_partionSection.GetComponent<Renderer>().enabled = false;
		
	}
	
	//Horizontal Node Split
	private void splitNodeHorizontal(GameObject _partionSection, out GameObject _pieceA, out GameObject _pieceB){
		float randSplitAmount = Random.Range(splitMargine,100 - splitMargine) / 100;
		
		//first sub area
		GameObject sectionA = (GameObject) GameObject.Instantiate(Resources.Load("PartionSection"));
		sectionA.transform.localScale = new Vector3(_partionSection.transform.localScale.x ,
													Mathf.Round(_partionSection.transform.localScale.y * randSplitAmount), 
													_partionSection.transform.localScale.z);
		
		sectionA.transform.position = new Vector3(	_partionSection.transform.position.x, 
													_partionSection.transform.position.y -_partionSection.transform.localScale.y/2 + sectionA.transform.localScale.y/2,
													_partionSection.transform.position.z);
		sectionA.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);
		//tidy the pieces into a folder in the hiearcy
		sectionA.transform.parent = folderObject.transform;
		_pieceA = sectionA;
		
		//second sub area
		GameObject sectionB = (GameObject) GameObject.Instantiate(Resources.Load("PartionSection"));
		sectionB.transform.localScale = new Vector3(_partionSection.transform.localScale.x,
													Mathf.Round(_partionSection.transform.localScale.y * (1 - randSplitAmount)), 
													_partionSection.transform.localScale.z);
		
		sectionB.transform.position = new Vector3(	_partionSection.transform.position.x, 
													sectionA.transform.position.y + sectionA.transform.localScale.y/2 + sectionB.transform.localScale.y/2,
													_partionSection.transform.position.z);
		sectionB.GetComponent<Renderer>().material.color = new Color(Random.Range(0,100)/100f, Random.Range(0,100)/100f, Random.Range(0,100)/100f);
		//tidy the pieces into a folder in the hiearcy
		sectionB.transform.parent = folderObject.transform;
		_pieceB = sectionB;
		
		_partionSection.GetComponent<Renderer>().enabled = false;
	}
}
