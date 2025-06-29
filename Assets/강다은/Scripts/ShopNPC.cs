using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShopNPC : NetworkBehaviour
{
    [Header("NPC 설정")]
    [SerializeField] private string npcId;

    // UI 참조
    private ShopUI shopUI;
    private BigInventoryUI bigInventoryUI;
    private Resell resellButton;

    public override void Spawned()
    {
        base.Spawned();
        var uiGO = GameObject.Find("ShopUICanvas");
        if (uiGO == null)
        {
            Debug.LogError("[ShopNPC] ShopUICanvas를 찾지 못했습니다!");
        }
        else
        {
            Debug.Log("[ShopNPC] ShopUICanvas를 찾았다잉~");
            shopUI = uiGO.GetComponent<ShopUI>();
            // 초기에는 숨겨 두기
            uiGO.SetActive(false);
        }

        // 씬에 배치된 InGameUI 에서 BigInventoryUI, Resell 버튼 찾기
        var ig = GameObject.Find("InGameUI");
        if (ig == null)
        {
            Debug.LogError("[ShopNPC] InGameUI를 찾지 못했습니다!");
        }
        else
        {
            bigInventoryUI = ig.GetComponentInChildren<BigInventoryUI>(true);
            resellButton = ig.GetComponentInChildren<Resell>(true);

            if (bigInventoryUI == null) Debug.LogError("[ShopNPC] BigInventoryUI가 없습니다!");
            if (resellButton == null) Debug.LogError("[ShopNPC] Resell 버튼이 없습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        var root = other.transform.root.gameObject;
        if (!root.CompareTag("Player"))
            return;

        // NetworkObject 검색
        var pobj = other.GetComponentInParent<NetworkObject>();
        if (pobj == null)
            return;
        if (pobj.HasInputAuthority)
        {
            if (shopUI != null)
            {
                shopUI.Show();
                shopUI.OpenShop(npcId);
            }
            if (bigInventoryUI != null)
                bigInventoryUI.OpenOrClose(true);
            if (resellButton != null)
                resellButton.OpenOrCloseResellButton(true);
        }

        if (Runner.IsServer)
            Rpc_OpenShop(pobj.InputAuthority, npcId);
    }

    private void OnTriggerExit(Collider other)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        var root = other.transform.root.gameObject;
        if (!root.CompareTag("Player"))
            return;

        var pobj = other.GetComponentInParent<NetworkObject>();
        if (pobj == null)
            return;

        // 로컬 플레이어인 경우 즉시 UI 닫기
        if (pobj.HasInputAuthority)
        {
            if (shopUI != null) shopUI.Hide();
            if (bigInventoryUI != null) bigInventoryUI.OpenOrClose(false);
            if (resellButton != null) resellButton.OpenOrCloseResellButton(false);
        }

        // 서버인 경우 RPC로도 다른 쪽 알림
        if (Runner.IsServer)
            Rpc_CloseShop(pobj.InputAuthority);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void Rpc_OpenShop(PlayerRef target, string npcId)
    {
        // 이 코드는 오직 해당 플레이어 클라이언트에서 실행됩니다
        if (shopUI != null)
            shopUI.Show();
        if (bigInventoryUI != null)
            bigInventoryUI.OpenOrClose(true);
        if (resellButton != null)
            resellButton.OpenOrCloseResellButton(true);

        // (필요하다면 OpenShop RPC로도 호출할 수 있습니다)
        shopUI?.OpenShop(npcId);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void Rpc_CloseShop(PlayerRef target)
    {
        if (shopUI != null)
            shopUI.Hide();
        if (bigInventoryUI != null)
            bigInventoryUI.OpenOrClose(false);
        if (resellButton != null)
            resellButton.OpenOrCloseResellButton(false);
    }
}
