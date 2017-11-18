using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

    public Tile tile;

    public CharacterStats stats = new CharacterStats();

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

    public void Place (Tile targetTile, int cost)
    {
        stats.curAP -= cost;
        float _height = targetTile.gameObject.GetComponent<BoxCollider>().bounds.extents.z * 2;
        Vector3 _targetPos = targetTile.transform.position;
        transform.position = targetTile.transform.position + new Vector3(0, _height, 0);
        tile = targetTile;
    }

    public abstract void Damage(int dmg);

}
