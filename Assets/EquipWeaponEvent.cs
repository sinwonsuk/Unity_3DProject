
using RPGCharacterAnims;

public struct EquipWeaponEvent : IEvent
{
    public int weaponId;
    public EquipWeaponEvent(int weaponId)
    {
        this.weaponId = weaponId;
    }
}

public struct UnEquipWeaponEvent : IEvent
{
    public int weaponId;
    public UnEquipWeaponEvent(int weaponId)
    {
        this.weaponId = weaponId;
    }
}
