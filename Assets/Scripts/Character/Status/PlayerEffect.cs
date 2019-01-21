using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEffect : MonoBehaviour
{
    CharController character;
    GameController gc;

    public abstract void Tick(CharController currentCharacter);
    public abstract void ApplyEffect(CharController _target);

    public virtual void Awake()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void Init()
    {
        gc.onUnitChange += Tick;
    }
}
