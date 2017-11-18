using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    public string playerName;
    public BaseAbility curAbility;
    public Movement moveAbility = new TeleportMovement();

    // References
    private BattleController bc;
    private StatusIndicator statusIndicator;

    // Use this for initialization
    void Start () {
        stats.Init();
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        statusIndicator = transform.Find("CameraAngleTarget").Find("StatusIndicator").GetComponent<StatusIndicator>();
        curAbility = new AttackAbility();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void PlayerAttack(Character _target, BaseAbility _ability)
    {
        if (stats.curAP >= _ability.AbilityCost)
        {
            stats.curAP -= _ability.AbilityCost;
            _target.Damage(_ability.AbilityPower);
        }

        if (stats.curAP <= 0)
        {
            
        }
    }

    public override void Damage(int amt)
    {
        stats.curHealth -= amt;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        if (stats.curHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        bc.players.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    public void fillAP(int amt)
    {
        stats.curAP = amt;
    }

    public void fillAP()
    {
        stats.curAP = stats.maxAP;
    }
}
