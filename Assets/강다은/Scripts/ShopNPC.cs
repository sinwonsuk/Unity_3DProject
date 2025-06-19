using UnityEngine;

public class ShopNPC : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log($"[ShopNpc] {npcId} ���� ����");
			shopUI.Show();
			shopUI.OpenShop(npcId);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			shopUI.Hide(); // UI ����
		}
	}

	[SerializeField] private string npcId;
	[SerializeField] private ShopUI shopUI;
}
