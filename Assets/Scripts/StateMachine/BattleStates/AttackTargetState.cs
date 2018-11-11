using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTargetState : BattleState
{
    List<Node> attackRange;
    public static BaseAbility attackAbility;

    public override void Enter()
    {
        base.Enter();
        attackAbility = gc.currentCharacter.attackAbility;
        attackRange = pathfinder.FindRange(gc.currentCharacter.tile.node, attackAbility.AbilityRange, attackAbility.diag, true, true);
        grid.HighlightNodes(attackRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.UnHighlightNodes(attackRange);
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
                Player player = gc.currentCharacter as Player;
                player.Attack(e.info.GetComponent<Enemy>(), attackAbility);
                gc.ChangeState<CommandSelectionState>();
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

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }
}
