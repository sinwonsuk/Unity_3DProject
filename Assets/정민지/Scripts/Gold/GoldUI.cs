using System.Linq;
using TMPro;
using UnityEngine;


public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Inventory inventory;
    public int showGold { get; private set; }

    private void OnEnable()
    {
        EventBus<Gold>.OnEvent += UpdateGoldUI;
        EventBus<BuyItemRequested>.OnEvent += PayAPrice;
        EventBus<GetGold>.OnEvent += SubtractGold;
    }

    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGoldUI;
        EventBus<BuyItemRequested>.OnEvent -= PayAPrice;
        EventBus<GetGold>.OnEvent -= SubtractGold;
    }

    private void UpdateGoldUI(Gold newGold)
    {
        showGold = newGold.currentGold;
        goldText.text = ($"{showGold}");
    }

    public void PayAPrice(BuyItemRequested newItem)
    {
        bool hasEmptySlot = inventory.slots.Any(slot => slot.item == null);

        if (newItem.itemData.itemType == ItemType.Potion)
        {
            bool hasSamePotion = inventory.slots.Any(slot => !slot.IsEmpty && slot.item == newItem.itemData);
            if (!hasEmptySlot && !hasSamePotion) return;
        }
        else
        {
            if (!hasEmptySlot) return;
        }

        int totalPrice = newItem.price * newItem.amount;

        if (showGold < totalPrice) return;

        showGold -= totalPrice;
        EventBus<Gold>.Raise(new Gold(showGold));
    }

    public void SubtractGold(GetGold use)
    {
        showGold += use.getGold/10;
        EventBus<Gold>.Raise(new Gold(showGold));
    }
}
