using System;

public class Damage : ICloneable
{
    public IDamageSource source;

    public BaseAbility ability;

    public DamageTypes.DamageType? damageType;
    public int? damageAmount;

    public MaladyTypes.MaladyType? maladyType;
    public int? maladyAmount;

    public bool TrueDamage;

    public Damage(IDamageSource _source, DamageTypes.DamageType _damageType, int _damageAmount, bool _trueDamage = false, BaseAbility _ability = null, Malady _malady = null)
    {
        source = _source;

        damageType = _damageType;
        damageAmount = _damageAmount;
        TrueDamage = _trueDamage;
    }

    public Damage(IDamageSource _source, DamageTypes.DamageType _damageType, int _damageAmount, MaladyTypes.MaladyType _maladyType, int _maladyAmount, bool _trueDamage = false, BaseAbility _ability = null, Malady _malady = null)
    {
        source = _source;

        damageType = _damageType;
        damageAmount = _damageAmount;

        maladyType = _maladyType;
        maladyAmount = _maladyAmount;

        TrueDamage = _trueDamage;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
