using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    public string enemyName;
    public BaseAbility curAbility;

    public EnemyStats stats = new EnemyStats();

    // References
    private BattleController bc;
    private StatusIndicator statusIndicator;

    [System.Serializable]
    public class EnemyStats
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
    void Start()
    {
        stats.Init();
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
        statusIndicator = transform.Find("CameraAngleTarget").Find("StatusIndicator").GetComponent<StatusIndicator>();
        curAbility = new AttackAbility();
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
        bc.enemies.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
