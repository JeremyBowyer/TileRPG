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
        Movement mover = owner.currentCharacter.gameObject.GetComponent<TeleportMovement>();
        attackRange = mover.GetNodesInRange(attackAbility.AbilityRange);
        grid.SelectTiles(attackRange);
    }

    public override void Exit()
    {
        base.Exit();
        grid.DeSelectTiles(attackRange);
        attackRange = null;
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        if (e.info.tag == "Enemy")
        {
            if (attackRange.Contains(e.info.GetComponent<Tile>().node))
            {
                Player player = owner.currentCharacter as Player;
                player.PlayerAttack(e.info.GetComponent<Enemy>(), attackAbility);
                owner.ChangeState<MoveSequenceState>();
            }
            else
            {
                Debug.Log("Select an enemy in range.");
            }
        }
        else
        {
            Debug.Log("Select an enemy.");
        }
    }
}
