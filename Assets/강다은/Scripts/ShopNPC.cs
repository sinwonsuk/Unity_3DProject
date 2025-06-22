using UnityEngine;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour
{
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
			shopUI.Hide(); // UI ����
            bigInventoryUI.OpenOrClose(false);
            resellButton.OpenOrCloseResellButton(false);
        }
	}

	[SerializeField] private string npcId;
	[SerializeField] private ShopUI shopUI;
	[SerializeField] private BigInventoryUI bigInventoryUI;
	[SerializeField] private Resell resellButton;
}
