using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEffect : MonoBehaviour
{
    public CharController target;
    BattleController bc;

    public abstract void TurnTick(CharController currentCharacter);
    public abstract void RoundTick();
    public abstract void ApplyEffect(CharController _target);
    public abstract void RefreshEffect();

    public virtual void RemoveEffect()
    {
        bc.onUnitChange -= TurnTick;
        bc.onRoundChange -= RoundTick;
        Destroy(this);
    }

    public virtual void Awake()
    {
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
    }

    public virtual void Init(CharController _target)
    {
        target = _target;
        bc.onUnitChange += TurnTick;
        bc.onRoundChange += RoundTick;
    }
}
