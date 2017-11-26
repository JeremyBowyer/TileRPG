using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetState : BattleState
{
    List<Tile> tiles;
    List<Node> attackRange;
    public static BaseAbility attackAbility;

    public override void Enter()
    {
        base.Enter();
        attackRange = pathfinder.FindRange(owner.currentCharacter.transform.position, attackAbility.AbilityRange, attackAbility.diag, true);
        grid.SelectRange(attackRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectRange(attackRange);
        attackRange = null;
    }

    protected override void AddListeners()
    {
        base.AddListeners();
        UserInputController.mouseLayer = LayerMask.NameToLayer("Character");
    }


    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        if (e.info.tag == "Enemy")
        {
            if (attackRange.Contains(e.info.GetComponent<Enemy>().tile.node))
            {
                Player player = owner.currentCharacter as Player;
                player.Attack(e.info.GetComponent<Enemy>(), attackAbility);
                owner.ChangeState<MoveSequenceState>();
            }
            else
            {
                Debug.Log("Select an enemy in range.");
            }
        }
        else
        {
            Debug.Log(e.info.tag);
            Debug.Log("Select an enemy.");
        }
    }
}
