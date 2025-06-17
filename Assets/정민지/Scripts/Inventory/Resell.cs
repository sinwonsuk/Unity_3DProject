using UnityEngine;

public class Resell : MonoBehaviour
{
    [SerializeField] private BigInventoryUI bigInventoryUI;
    [SerializeField] private Inventory inventory;

    public void OnClickResellButton()
    {
        inventory.SellItem(bigInventoryUI.GetBigSelectedItem());
    }
}
