using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public Tile tile;
    public CharacterStats stats = new CharacterStats();
    public BaseAbility attackAbility;
    public Movement movementAbility;

    // References
    public GameController gc;
    public StatusIndicator statusIndicator;

    public float height;

    [System.Serializable]
    public class CharacterStats
    {
        public int maxHealth = 100;
        public int maxAP = 1000;
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
            get { return _curAP / 2; }
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
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        height = GetComponent<BoxCollider>().bounds.extents.y * 2;
    }

    public void Place(Tile targetTile)
    {

        float _height = targetTile.gameObject.GetComponent<BoxCollider>().bounds.extents.y + gameObject.GetComponent<BoxCollider>().bounds.extents.y;

        Vector3 _targetPos = targetTile.transform.position;
        transform.position = targetTile.transform.position + new Vector3(0, _height, 0);

        tile = targetTile;
        targetTile.occupant = gameObject;
    }

    public void Move (Tile targetTile)
    {
        gc.pathfinder.FindPath(tile.node, targetTile.node, stats.moveRange, movementAbility.diag, false, movementAbility.ignoreUnwalkable); // This assigns proper costs to tiles
        if (stats.curAP >= targetTile.node.gCost)
            StartCoroutine(movementAbility.Traverse(targetTile));
            stats.curAP -= targetTile.node.gCost;
            //statusIndicator.SetAP(stats.curAP, stats.maxAP);

            if(tile != null)
                tile.occupant = null;

            targetTile.occupant = gameObject;
            tile = targetTile;

            if (stats.curAP <= 0)
            {
                movementAbility.nextTurn = true;
            }

    }

    public void Attack(Character _target, BaseAbility _ability)
    {
        if (stats.curAP >= _ability.AbilityCost)
        {
            stats.curAP -= _ability.AbilityCost;
            StartCoroutine(_ability.Initiate(_target));
        }

        if (stats.curAP < attackAbility.AbilityCost)
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

    public virtual void Die()
    {
        gc.CheckEndCondition();
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
