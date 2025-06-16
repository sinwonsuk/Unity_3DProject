using UnityEngine;

public class ShopNPC : MonoBehaviour
{
	[SerializeField] private string npcId;           
	[SerializeField] private ShopUI shopUI;         

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
}
