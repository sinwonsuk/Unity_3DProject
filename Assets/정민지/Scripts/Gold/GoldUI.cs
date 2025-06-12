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
    }

    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGoldUI;
    }

    private void UpdateGoldUI(Gold newGold)
    {
        showGold = newGold.currentGold;
        goldText.text = ($"{showGold}");
    }

    public void AddGold(int amount)
    {
        showGold += amount;
        EventBus<Gold>.Raise(new Gold(showGold));
    }

    public void SubtractGold(int amount)
    {
        showGold -= amount;
        EventBus<Gold>.Raise(new Gold(showGold));
    }
    
}
