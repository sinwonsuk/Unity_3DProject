using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShopNPC : NetworkBehaviour
{
    [Header("NPC ����")]
    [SerializeField] private string npcId;

    // UI ����
    private ShopUI shopUI;
    private BigInventoryUI bigInventoryUI;
    private Resell resellButton;

    public override void Spawned()
    {
        base.Spawned();
        var uiGO = GameObject.Find("ShopUICanvas");
        if (uiGO == null)
        {
            Debug.LogError("[ShopNPC] ShopUICanvas�� ã�� ���߽��ϴ�!");
        }
        else
        {
            Debug.Log("[ShopNPC] ShopUICanvas�� ã�Ҵ���~");
            shopUI = uiGO.GetComponent<ShopUI>();
            // �ʱ⿡�� ���� �α�
            uiGO.SetActive(false);
        }

        // ���� ��ġ�� InGameUI ���� BigInventoryUI, Resell ��ư ã��
        var ig = GameObject.Find("InGameUI");
        if (ig == null)
        {
            Debug.LogError("[ShopNPC] InGameUI�� ã�� ���߽��ϴ�!");
        }
        else
        {
            bigInventoryUI = ig.GetComponentInChildren<BigInventoryUI>(true);
            resellButton = ig.GetComponentInChildren<Resell>(true);

            if (bigInventoryUI == null) Debug.LogError("[ShopNPC] BigInventoryUI�� �����ϴ�!");
            if (resellButton == null) Debug.LogError("[ShopNPC] Resell ��ư�� �����ϴ�!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        var root = other.transform.root.gameObject;
        if (!root.CompareTag("Player"))
            return;

        // NetworkObject �˻�
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

        // ���� �÷��̾��� ��� ��� UI �ݱ�
        if (pobj.HasInputAuthority)
        {
            if (shopUI != null) shopUI.Hide();
            if (bigInventoryUI != null) bigInventoryUI.OpenOrClose(false);
            if (resellButton != null) resellButton.OpenOrCloseResellButton(false);
        }

        // ������ ��� RPC�ε� �ٸ� �� �˸�
        if (Runner.IsServer)
            Rpc_CloseShop(pobj.InputAuthority);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void Rpc_OpenShop(PlayerRef target, string npcId)
    {
        // �� �ڵ�� ���� �ش� �÷��̾� Ŭ���̾�Ʈ���� ����˴ϴ�
        if (shopUI != null)
            shopUI.Show();
        if (bigInventoryUI != null)
            bigInventoryUI.OpenOrClose(true);
        if (resellButton != null)
            resellButton.OpenOrCloseResellButton(true);

        // (�ʿ��ϴٸ� OpenShop RPC�ε� ȣ���� �� �ֽ��ϴ�)
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
