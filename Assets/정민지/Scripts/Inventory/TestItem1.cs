using UnityEngine;

public class TestItem1 : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private BigInventoryUI bigInventoryUI;

    public void OnClickAndSellItem()
    {
        inventory.SellItem(bigInventoryUI.GetBigSelectedItem());
    }

}
