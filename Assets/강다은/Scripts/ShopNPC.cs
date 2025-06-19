using UnityEngine;

public class ShopNPC : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log($"[ShopNpc] {npcId} 상점 열림");
			shopUI.Show();
			shopUI.OpenShop(npcId);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			shopUI.Hide(); // UI 끄기
		}
	}

	[SerializeField] private string npcId;
	[SerializeField] private ShopUI shopUI;
}
