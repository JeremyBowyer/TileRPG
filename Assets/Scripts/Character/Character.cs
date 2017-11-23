using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public Tile tile;
    public CharacterStats stats = new CharacterStats();

    // References
    public BattleController bc;
    public StatusIndicator statusIndicator;

    [System.Serializable]
    public class CharacterStats
    {
        public int maxHealth = 100;
        public int maxAP = 100;
        public int maxMP = 100;

        private int _curHealth;
        public int curHealth
        {
            get { return _curHealth; }
            set { _curHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        private int _curAP;
        public int curAP
        {
            get { return _curAP; }
            set { _curAP = Mathf.Clamp(value, 0, maxAP); }
        }

        private int _curMP;
        public int curMP
        {
            get { return _curMP; }
            set { _curMP = Mathf.Clamp(value, 0, maxMP); }
        }

        public int moveRange
        {
            get { return _curAP; }
        }

        public void Init()
        {
            _curHealth = maxHealth;
            _curAP = maxAP;
            _curMP = maxMP;
        }
    }

    public void Awake()
    {
        if (bc == null)
            Debug.LogError("Battle controller not assigned to " + gameObject.name);

        if (statusIndicator == null)
            Debug.LogError("Status Indicator not assigned to " + gameObject.name);
    }

    public void Place (Tile targetTile, int cost)
    {
        stats.curAP -= cost;
        statusIndicator.SetAP(stats.curAP, stats.maxAP);
        float _height = targetTile.gameObject.GetComponent<BoxCollider>().bounds.extents.z * 2;
        Vector3 _targetPos = targetTile.transform.position;
        transform.position = targetTile.transform.position + new Vector3(0, _height, 0);
        if(tile != null)
            tile.occupant = null;
        targetTile.occupant = gameObject;
        tile = targetTile;
        
        if (stats.curAP <= 0)
        {
            bc.ChangeState<SelectUnitState>();
        }
    }

    public void Attack(Character _target, BaseAbility _ability)
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

    public void Damage(int amt)
    {
        stats.curHealth -= amt;
        statusIndicator.SetHealth(stats.curHealth, stats.maxHealth);
        if (stats.curHealth <= 0)
        {
            Die();
        }
    }

    public abstract void Die();

    public void fillAP(int amt)
    {
        stats.curAP = amt;
    }

    public void fillAP()
    {
        stats.curAP = stats.maxAP;
    }

}
