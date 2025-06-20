using UnityEngine;
using System.Collections.Generic;

public class ItemBox : MonoBehaviour
{ 
	private void OnEnable()
	{
		EventBus<ItemBoxUIClose>.OnEvent += OnUIClose;
	}

	private void Start()
	{
		player = GameObject.FindWithTag("Player")?.transform;

		// ���� ���� ���� ������ 2�� ���� ����
		selectedItems = GetRandomItems(2);
	}

	private void Update()
	{
		if (player == null) return;

		float dist = Vector3.Distance(transform.position, player.position);

		if (dist < interactionDistance && Input.GetKeyDown(KeyCode.E))
		{
			if (isOpened)
			{
				EventBus<ItemBoxUIClose>.Raise(new ItemBoxUIClose(this.gameObject));
				isOpened = false;
			}
			else
			{
				OpenBox();
			}
		}

	}
	private void OnDisable()
	{
		EventBus<ItemBoxUIClose>.OnEvent -= OnUIClose;
	}

	private void OpenBox()
	{
		isOpened = true;

		EventBus<ItemBoxOpened>.Raise(new ItemBoxOpened(selectedItems, this.gameObject));

	}

	private void OnUIClose(ItemBoxUIClose evt)
	{
		if (evt.targetBox == this.gameObject)
		{
			Debug.Log("[ItemBox] UI ���� ���� �� ���� ����");
			Destroy(gameObject);
		}
	}

	private List<ItemData> GetRandomItems(int count)
	{
		List<ItemData> result = new();
		List<ItemData> pool = new(possibleItems);

		for (int i = 0; i < count && pool.Count > 0; i++)
		{
			int idx = Random.Range(0, pool.Count);
			result.Add(pool[idx]);
			pool.RemoveAt(idx);
		}
		return result;
	}

	[SerializeField] private GameObject visual;
	[SerializeField] private float interactionDistance = 3f;
	[SerializeField] private List<ItemData> possibleItems; // ���⼭ �����ϰ� 2�� ����
	//[SerializeField] private GameObject itemBoxUIPanel;
	private bool isOpened = false;
	private Transform player;
	private List<ItemData> selectedItems;
}
