using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private string npcId;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private GameObject ingameUI;

    private BigInventoryUI bigInventoryUI;
    private Resell resellButton; // 버튼용 스크립트

    private void Awake()
    {
        bigInventoryUI = ingameUI.GetComponentInChildren<BigInventoryUI>(true);
        resellButton = ingameUI.GetComponentInChildren<Resell>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[ShopNpc] {npcId} 상점 열림");
            shopUI.Show();
            shopUI.OpenShop(npcId);
            bigInventoryUI.OpenOrClose(true);
            resellButton.OpenOrCloseResellButton(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopUI.Hide();
            bigInventoryUI.OpenOrClose(false);
            resellButton.OpenOrCloseResellButton(false);
        }
    }
}
