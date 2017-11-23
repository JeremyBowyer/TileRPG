using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {

    public string playerName;
    public BaseAbility curAbility;
    public Movement moveAbility;

    // Use this for initialization
    void Start () {
        stats.Init();
        curAbility = new AttackAbility();

        if (moveAbility == null)
            Debug.LogError("No Movement Ability assigned to " + gameObject.name);
    }

    public override void Die()
    {
        bc.players.Remove(this.gameObject);
        bc.characters.Remove(this);
        Destroy(this.gameObject);
    }
}
