using UnityEngine;
using System.Collections;
public abstract class BattleState : State
{
    protected BattleController owner;
    public CameraController cameraRig { get { return owner.cameraRig; } }
    public Grid grid { get { return owner.grid; } }
    public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; } }
    public Node node { get { return owner.node; } set { owner.node = value; } }
    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();
    }

    protected override void AddListeners()
    {
        UserInputController.clickEvent += OnClick;
    }

    protected override void RemoveListeners()
    {
        UserInputController.clickEvent -= OnClick;
    }
    protected virtual void OnClick(object sender, InfoEventArgs<Node> e)
    {

    }

    /*
    protected virtual void SelectTile(Tile t)
    {
        if (pos == t)
            return;
        pos = t;
        tileSelectionIndicator.localPosition = board.tiles[p].center;
    }
    */
}