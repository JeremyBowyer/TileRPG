using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{

    public string cName;
    public string cClass;
    public CharacterStats stats = new CharacterStats();

    public class CharacterStats
    {
        public int maxHealth = 100;
        public int maxAP = 100;
        public int maxMP = 100;

        public int experience;
        public int level;

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

            experience = 0;
            level = 1;
        }
    }


}
