using UnityEngine;

public enum isDir
{
    Right,
    Left,
}

public class WeaponManager
{
    private WeaponsConfig config;
    private Transform rightHandSocket;
    private Transform leftHandSocket;
    private GameObject currentWeapon;
    private ItemState currentWeaponState;

    public WeaponManager(WeaponsConfig config, Transform rightHandSocket, Transform leftHandSocket)
    {
        this.config = config;

        this.rightHandSocket = rightHandSocket;
        this.leftHandSocket = leftHandSocket;
    }

    public void Equip(ItemState state,isDir Dir)
    {

        GameObject prefab = config.GetWeapon(state);

        if (prefab != null && Dir == isDir.Right)
        {
            currentWeapon = GameObject.Instantiate(prefab, rightHandSocket);
        }
        else if(prefab != null && Dir == isDir.Left)
        {
            currentWeapon = GameObject.Instantiate(prefab, leftHandSocket);
        } 
        else
        {
            Debug.LogWarning("무기 없음");
        }

        currentWeaponState = state;
    }

    public GameObject CreateArrow()
    {
        if (currentWeaponState != ItemState.Bow)
            return null;

        return GameObject.Instantiate (config.GetWeapon(ItemState.Arrow), currentWeapon.GetComponent<Bow>().Rope.transform);     
    }


    public void Unequip()
    {
        if (currentWeapon != null)
        {
            GameObject.Destroy(currentWeapon);
            currentWeapon = null;
        }
    }
    public GameObject GetCurrentWeapon() => currentWeapon;
}