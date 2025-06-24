using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [SerializeField] private string npcId;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private GameObject ingameUI;

    private BigInventoryUI bigInventoryUI;
    private Resell resellButton; // ��ư�� ��ũ��Ʈ

    private void Awake()
    {
        bigInventoryUI = ingameUI.GetComponentInChildren<BigInventoryUI>(true);
        resellButton = ingameUI.GetComponentInChildren<Resell>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[ShopNpc] {npcId} ���� ����");
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
