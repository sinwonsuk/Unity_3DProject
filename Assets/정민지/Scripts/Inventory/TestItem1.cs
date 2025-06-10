using UnityEngine;

public class TestItem1 : MonoBehaviour
{
    public ItemData sword1;
    public ItemData sword2;
    public ItemData sword3;
    [SerializeField] private Inventory inventory;

    public void OnClickButtonOne()
    {
        inventory.BuyItem(sword1);
        Debug.Log("기본검 구매");
    }

    public void OnClickButtonTwo()
    {
        inventory.BuyItem(sword2);
    }

    public void OnClickButtonThree()
    {
        inventory.BuyItem(sword3);
    }

}
