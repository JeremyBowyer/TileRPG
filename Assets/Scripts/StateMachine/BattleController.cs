using UnityEngine;
using System.Collections;
public class BattleController : StateMachine
{
    public CameraController cameraRig;
    public Grid grid;
    //public LevelData levelData;
    public Transform tileSelectionIndicator;
    public Node node;
    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        ChangeState<InitBattleState>();
    }
}