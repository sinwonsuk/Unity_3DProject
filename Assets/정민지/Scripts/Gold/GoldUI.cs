using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    public int showGold { get; private set; }

    private void OnEnable()
    {
        EventBus<Gold>.OnEvent += UpdateGoldUI;
        EventBus<BuyItemRequested>.OnEvent += AddGold; 
    }

    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGoldUI;
        EventBus<BuyItemRequested>.OnEvent -= AddGold;
    }

    private void UpdateGoldUI(Gold newGold)
    {
        showGold = newGold.currentGold;
        goldText.text = ($"{showGold}");
    }

    public void AddGold(BuyItemRequested newItem)
    {
        int totalPrice = newItem.price * newItem.amount;

        if (showGold < totalPrice)
        {
            Debug.Log("°ñµå ºÎÁ·");
            return;
        }

        showGold -= totalPrice;
        EventBus<Gold>.Raise(new Gold(showGold));
    }

    public void SubtractGold(int price)
    {
        showGold += price/10;
        EventBus<Gold>.Raise(new Gold(showGold));
    }
}
