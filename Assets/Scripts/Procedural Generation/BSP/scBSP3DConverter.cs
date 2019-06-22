using UnityEngine;
using System.Collections;

public class scBSP3DConverter{
	
	private scGrid levelGrid;
	
	private float floorY;
	
	public scBSP3DConverter(){
		
		GameObject aFloor = GameObject.FindGameObjectWithTag("Ground");
		floorY = aFloor.transform.position.y;
		
		findGridSize();
		//addWalls();
		
	}
	
	private void findGridSize(){
		GameObject[] allWalls = GameObject.FindGameObjectsWithTag("Ground");
		
		int lowX = 0;
		int lowY = 0;
		int lowZ = 0;
		
		int highX = 0;
		int highY = 0;
		int highZ = 0;
		
		bool first = true;
		
		foreach(GameObject aWall in allWalls){
			
			if (first){
				lowX = (int) Mathf.Round(aWall.transform.position.x)-1;	
				highX = (int) Mathf.Round(aWall.transform.position.x)+1;
				lowZ = (int) Mathf.Round(aWall.transform.position.z)-1;
				highZ = (int)  Mathf.Round(aWall.transform.position.z)+1;
					
				first = false;
			}else{
				if (aWall.transform.position.x < lowX){
					lowX = (int) Mathf.Round(aWall.transform.position.x)-1;	
				}
				
				if (aWall.transform.position.x > highX){
					highX = (int) Mathf.Round(aWall.transform.position.x)+1;	
				}
				
				if (aWall.transform.position.z < lowZ){
					lowZ = (int) Mathf.Round(aWall.transform.position.z)-1;	
				}
				
				if (aWall.transform.position.z > highZ){
					highZ = (int)  Mathf.Round(aWall.transform.position.z)+1;	
				}
			}
			
		}

		int width = highX - lowX;
		int height = highZ - lowZ;
		
		//make the grid abit bigger than the floor size because walls need to get added around floors
		createGrid(width+4, height+4,lowX-2, lowZ-2);
	}
	
	private void createGrid(int _width, int _height, int lowX, int lowZ){
		levelGrid = new scGrid(_width, _height, lowX, lowZ);

		GameObject[] allWalls = GameObject.FindGameObjectsWithTag("BaseWall");
		GameObject[] allGrounds = GameObject.FindGameObjectsWithTag("Ground");
		
		foreach(GameObject aWall in allWalls){
			int index = (int) (aWall.transform.position.x - lowX);
			int index1 = (int) (aWall.transform.position.z - lowZ);
			
			levelGrid.setCell(index, index1 , 2);
		}
		
		foreach(GameObject aGround in allGrounds){
			int index = (int) ((aGround.transform.position.x - lowX));
			int index1 = (int) ((aGround.transform.position.z - lowZ));
			
			levelGrid.setCell(index, index1 , 1);
		}

		GameObject theHasher = (GameObject) GameObject.FindGameObjectWithTag("CheckCollider");
		theHasher.GetComponent<scCheckCollider>().setWorldGrid(levelGrid);
		
		foreach(GameObject aWall in allWalls){
			aWall.GetComponent<scWall>().autoTile();	
		}
			
	}
	
	public void addWalls(){
		for (int i = 0; i < levelGrid.getWidth(); i++){
			for(int j = 0; j < levelGrid.getHeight(); j++){
				
				if (levelGrid.getCell(i,j) == 0){
					
					if (levelGrid.getCell(i-1,j) == 1 ||
						levelGrid.getCell(i+1,j) == 1 ||
						levelGrid.getCell(i,j-1) == 1 ||
						levelGrid.getCell(i,j+1) == 1 ||
						levelGrid.getCell(i-1,j-1) == 1 ||
						levelGrid.getCell(i-1,j+1) == 1 ||
						levelGrid.getCell(i+1,j-1) == 1 ||
						levelGrid.getCell(i+1,j+1) == 1)
					{
						levelGrid.setCell(i,j,2);
						
						GameObject aWall =(GameObject) GameObject.Instantiate(Resources.Load("Wall"));
						
						float wX = aWall.GetComponent<Renderer>().bounds.size.x;
						float wY = aWall.GetComponent<Renderer>().bounds.size.y;
						float wZ = aWall.GetComponent<Renderer>().bounds.size.z;
						
						aWall.transform.position = new Vector3(( levelGrid.getLowX() + i )+0.5f,floorY +(wY/2), levelGrid.getLowZ() + j + 0.5f);
						
					}else{
						GameObject aRoof =(GameObject) GameObject.Instantiate(Resources.Load("TileSets/Roof"));
						
						aRoof.transform.position = new Vector3((int) ( levelGrid.getLowX() + i  ) +0.5f,1.633101f, levelGrid.getLowZ() + j +0.5f);
					}
				}
				
			}
		}
		
		GameObject[] allWalls = GameObject.FindGameObjectsWithTag("BaseWall");
		
		foreach(GameObject aWall in allWalls){
			aWall.GetComponent<scWall>().autoTile();
		}
	}
	
	public scGrid getLevelGrid(){
		return levelGrid;
	}
	
	public float getFloorY(){
		return floorY;	
	}
}
