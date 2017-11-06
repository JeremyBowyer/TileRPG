using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public string playerName;
    public BaseAbility curAbility;

    public PlayerStats stats = new PlayerStats();

    // References
    private BattleMaster bm;
    private Pathfinding pathfinder;
    private Grid grid;
    private StatusIndicator statusIndicator;

    [System.Serializable]
    public class PlayerStats
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

    // Use this for initialization
    void Start () {
        stats.Init();
        bm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<BattleMaster>();
        pathfinder = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Pathfinding>();
        grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grid>();
        statusIndicator = transform.Find("CameraAngleTarget").Find("StatusIndicator").GetComponent<StatusIndicator>();
        curAbility = new AttackAbility();
    }
	
	// Update is called once per frame
	void Update () {
		if(this == bm.curPlayer)
        {
            transform.Find("CameraAngleTarget").Find("StatusIndicator").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("CameraAngleTarget").Find("StatusIndicator").gameObject.SetActive(true);
        }
	}

    public bool PlayerMove(Tile targetTile)
    {

        float _height = targetTile.gameObject.GetComponent<BoxCollider>().bounds.extents.z * 2;
        Vector3 _targetPos = targetTile.transform.position;
        List<Node> _path = pathfinder.FindPath(gameObject.transform.position, _targetPos, stats.moveRange);
        Node _node = grid.NodeFromWorldPoint(targetTile.transform.position);

        if (_path.Count > 0 && _path.Contains(_node) && targetTile.occupant == null)
        {
            grid.TileFromNode(grid.NodeFromWorldPoint(transform.position)).GetComponent<Tile>().occupant = null;
            targetTile.occupant = gameObject;
            transform.position = targetTile.transform.position + new Vector3(0, _height, 0);
            stats.curAP -= _node.gCost;
            bm.statusIndicator.SetAP(stats.curAP, stats.maxAP);
            if (stats.curAP <= 0)
            {
                bm.NextPlayer();
            }
            else
            {
                bm.PlayerMove();
            }
            return true;
        }
        else
        {
            return false;
        }

    }

    public void PlayerAttack(Player _target, BaseAbility _ability)
    {
        if (stats.curAP >= _ability.AbilityCost)
        {
            stats.curAP -= _ability.AbilityCost;
            bm.statusIndicator.SetAP(stats.curAP, stats.maxAP);

            _target.Damage(_ability.AbilityPower);
        }

        if (stats.curAP <= 0)
        {
            bm.NextPlayer();
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

    public void Die()
    {
        bm.players.Remove(this.gameObject);
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
