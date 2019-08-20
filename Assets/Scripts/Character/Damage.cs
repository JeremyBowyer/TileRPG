
public class Damage
{
    public DamageTypes.DamageType? damageType;
    public int? damageAmount;

    public MaladyTypes.MaladyType? maladyType;
    public int? maladyAmount;

    public bool TrueDamage;

    public Damage(DamageTypes.DamageType _damageType, int _damageAmount, bool _trueDamage = false)
    {
        damageType = _damageType;
        damageAmount = _damageAmount;
        TrueDamage = _trueDamage;
    }

    public Damage(DamageTypes.DamageType _damageType, int _damageAmount, MaladyTypes.MaladyType _maladyType, int _maladyAmount, bool _trueDamage = false)
    {
        damageType = _damageType;
        damageAmount = _damageAmount;

        maladyType = _maladyType;
        maladyAmount = _maladyAmount;

        TrueDamage = _trueDamage;
    }
}
