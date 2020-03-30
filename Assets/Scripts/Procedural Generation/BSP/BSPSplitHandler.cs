using UnityEngine;
using System.Collections;
/// <summary>
/// Handles the randomise splitting of BSP Partion areas
/// </summary>
public class BSPSplitHandler {
	
    private int cnt;
    private int minSplitSize;
    private float maxAspectRatio;

	private GameObject folderObject;
	
	public BSPSplitHandler(GameObject _folder, int _minSplitSize, float _maxAspectRatio)
    {
        folderObject = _folder;
        cnt = 0;
        minSplitSize = _minSplitSize;
        maxAspectRatio = _maxAspectRatio;
	}
	
	public void split(GameObject _mainPiece, out GameObject _pieceA, out GameObject _pieceB)
    {
        if (_mainPiece.transform.localScale.y > _mainPiece.transform.localScale.x){
			splitNodeHorizontal(_mainPiece, out _pieceA, out _pieceB);
		}else if (_mainPiece.transform.localScale.y < _mainPiece.transform.localScale.x){
			splitNodeVertical(_mainPiece, out _pieceA, out _pieceB);
		}else{
			//randomise which way the split happens
			int choice = Random.Range(0,2);
			
			if (choice == 0){
				splitNodeVertical(_mainPiece, out _pieceA, out _pieceB);
			}else{
				splitNodeHorizontal(_mainPiece,out _pieceA, out _pieceB);
			}
		}
	}
	
	//Vertical Node Split
	private void splitNodeVertical(GameObject _mainPiece, out GameObject _pieceA, out GameObject _pieceB){
		
        float mainScaleX = _mainPiece.transform.localScale.x;
        float mainScaleY = _mainPiece.transform.localScale.y;
        float mainScaleZ = _mainPiece.transform.localScale.z;
        float mainPosX = _mainPiece.transform.position.x;
        float mainPosY = _mainPiece.transform.position.y;
        float mainPosZ = _mainPiece.transform.position.z;

        if (Mathf.Round(mainScaleX / 2) < minSplitSize){
            _pieceA = null;
            _pieceB = null;
            return;
        }

        // Determine split amount
        // lower bound for random split amount can't be smaller than:
        // 1. An amount that would result in an unallowed aspect ratio
        // 2. Minimum room size
        //float splitLower = Mathf.Max(new float[] { mainScaleY / maxAspectRatio, minRoomSize + roomBufferBounds.y * 2 });
        float splitLower = Mathf.Max(new float[] { mainScaleY / maxAspectRatio, minSplitSize });
        // Upper bound for random split amount can't be smaller than:
        // 1. An amount that would result in an unallowed aspect ratio
        // 2. that room's Y minus minimum room size.
        //float splitUpper = Mathf.Min(new float[] { mainScaleX - mainScaleY / maxAspectRatio, mainScaleX - (minRoomSize + roomBufferBounds.y * 2) }); ;
        float splitUpper = Mathf.Min(new float[] { mainScaleX - mainScaleY / maxAspectRatio, mainScaleX - minSplitSize }); ;

        if (splitLower > splitUpper)
        {
            _pieceA = null;
            _pieceB = null;
            return;
        }

        float randSplitAmount = Random.Range(splitLower, splitUpper);

        //first sub area
        GameObject sectionA = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		sectionA.transform.localScale = new Vector3(Mathf.Round(randSplitAmount), mainScaleY, mainScaleZ);
		sectionA.transform.position = new Vector3(mainPosX - mainScaleX / 2 + sectionA.transform.localScale.x/2, mainPosY, mainPosZ);

		//tidy the pieces into a folder in the hiearcy
		sectionA.transform.parent = folderObject.transform;
        sectionA.name = "Node " + ++cnt;
		_pieceA = sectionA;
		
		//second sub area
		GameObject sectionB = (GameObject) GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
		sectionB.transform.localScale = new Vector3(Mathf.Round(mainScaleX - randSplitAmount), mainScaleY, mainScaleZ);
		sectionB.transform.position = new Vector3(sectionA.transform.position.x + sectionA.transform.localScale.x/2 + sectionB.transform.localScale.x/2, mainPosY, mainPosZ);

		//tidy the pieces into a folder in the hiearcy
		sectionB.transform.parent = folderObject.transform;
        sectionB.name = "Node " + ++cnt;
        _pieceB = sectionB;
	}

    //Horizontal Node Split
    private void splitNodeHorizontal(GameObject _mainPiece, out GameObject _pieceA, out GameObject _pieceB)
    {
        float mainScaleX = _mainPiece.transform.localScale.x;
        float mainScaleY = _mainPiece.transform.localScale.y;
        float mainScaleZ = _mainPiece.transform.localScale.z;
        float mainPosX = _mainPiece.transform.position.x;
        float mainPosY = _mainPiece.transform.position.y;
        float mainPosZ = _mainPiece.transform.position.z;

        if (Mathf.Round(mainScaleY / 2) < minSplitSize)
        {
            _pieceA = null;
            _pieceB = null;
            return;
        }

        // Determine split amount
        // lower bound for random split amount can't be smaller than:
        // 1. An amount that would result in an unallowed aspect ratio
        // 2. Minimum room size
        //float splitLower = Mathf.Max(new float[] { mainScaleX / maxAspectRatio, minRoomSize + roomBufferBounds.y * 2 });
        float splitLower = Mathf.Max(new float[] { mainScaleX / maxAspectRatio, minSplitSize});
        // Upper bound for random split amount can't be smaller than:
        // 1. An amount that would result in an unallowed aspect ratio
        // 2. that room's Y minus minimum room size.
        //float splitUpper = Mathf.Min(new float[] { mainScaleY - mainScaleX / maxAspectRatio, mainScaleY - minRoomSize + roomBufferBounds.y * 2) }); ;
        float splitUpper = Mathf.Min(new float[] { mainScaleY - mainScaleX / maxAspectRatio, mainScaleY - minSplitSize }); ;

        if (splitLower > splitUpper)
        {
            _pieceA = null;
            _pieceB = null;
            return;
        }

        float randSplitAmount = Random.Range(splitLower, splitUpper);

        //first sub area
        GameObject sectionA = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
        sectionA.transform.localScale = new Vector3(mainScaleX, Mathf.Round(randSplitAmount), mainScaleZ);
        sectionA.transform.position = new Vector3(mainPosX, mainPosY - mainScaleY / 2 + sectionA.transform.localScale.y / 2, mainPosZ);

        //tidy the pieces into a folder in the hiearcy
        sectionA.transform.parent = folderObject.transform;
        sectionA.name = "Node " + ++cnt;
        _pieceA = sectionA;

        //second sub area
        GameObject sectionB = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Procedural Generation/BSP/PartitionSection"));
        sectionB.transform.localScale = new Vector3(mainScaleX, Mathf.Round(mainScaleY - randSplitAmount), mainScaleZ);
        sectionB.transform.position = new Vector3(mainPosX, sectionA.transform.position.y + sectionA.transform.localScale.y / 2 + sectionB.transform.localScale.y / 2, mainPosZ);

        //tidy the pieces into a folder in the hiearcy
        sectionB.transform.parent = folderObject.transform;
        sectionB.name = "Node " + ++cnt;
        _pieceB = sectionB;
    }
}
