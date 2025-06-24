using Fusion;
using UnityEngine;

public class ShopNPC : NetworkBehaviour
{
    [SerializeField] private string npcId;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private GameObject ingameUI;

    private BigInventoryUI bigInventoryUI;
    private Resell resellButton; // 버튼용 스크립트

    //private void Awake()
    //{
    //    bigInventoryUI = ingameUI.GetComponentInChildren<BigInventoryUI>(true);
    //    resellButton = ingameUI.GetComponentInChildren<Resell>(true);
    //}

    public override void Spawned()
    {
        if (Object.HasInputAuthority || Object.HasStateAuthority) // 로컬에서만
        {
            GameObject ingameUI = GameObject.Find("IngameUI"); // 씬 안의 오브젝트
            if (ingameUI != null)
                LinkUI(ingameUI);
        }
    }

    public void LinkUI(GameObject ingameUI)
    {
        shopUI = ingameUI.GetComponentInChildren<ShopUI>(true);
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
