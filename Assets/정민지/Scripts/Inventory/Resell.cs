using UnityEngine;

public class Resell : MonoBehaviour
{
    [SerializeField] private BigInventoryUI bigInventoryUI;
    [SerializeField] private Inventory inventory;

    public void OpenOrCloseResellButton(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }
    public void OnClickResellButton()
    {
        inventory.SellItem(bigInventoryUI.GetBigSelectedItem());
    }
}
