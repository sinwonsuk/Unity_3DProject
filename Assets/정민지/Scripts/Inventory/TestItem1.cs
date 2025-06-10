using UnityEngine;

public class TestItem1 : MonoBehaviour
{
    public ItemData sword1;
    public ItemData sword2;
    public ItemData sword3;
    [SerializeField] private Inventory inventory;

    public void OnClickButtonOne()
    {
        inventory.AddItem(sword1);
    }

    public void OnClickButtonTwo()
    {
        inventory.AddItem(sword2);
    }

    public void OnClickButtonThree()
    {
        inventory.AddItem(sword3);
    }

}
