using RPGCharacterAnims;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<EquipWeaponEvent>.OnEvent += EquipWeapon;
        EventBus<UnEquipWeaponEvent>.OnEvent += UnequipWeapon;
    }

    private void OnDisable()
    {
        EventBus<EquipWeaponEvent>.OnEvent -= EquipWeapon;
        EventBus<UnEquipWeaponEvent>.OnEvent -= UnequipWeapon;
    }

    public void EquipWeapon(EquipWeaponEvent equipWeaponEvent)
    {
        currentWeapon = equipWeaponEvent.weaponId;

        StartCoroutine(ikHands._BlendIK(true, 0f, 0.3f, currentWeapon));
    }

    public void UnequipWeapon(UnEquipWeaponEvent equipWeaponEvent)
    {
        StartCoroutine(ikHands._BlendIK(false, 0f, 0.3f, currentWeapon));
    }

    public IKHands ikHands;
    public int currentWeapon;
}