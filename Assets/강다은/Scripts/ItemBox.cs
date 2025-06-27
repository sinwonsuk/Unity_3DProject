using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;

public class ItemBox : NetworkBehaviour
{
    [Header("Visual & Settings")]
    [SerializeField] private GameObject visual;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private List<ItemData> possibleItems;

    private Transform player;
    private bool isOpened = false;
    private List<ItemData> selectedItems;

    private void OnEnable()
    {
        EventBus<ItemBoxUIClose>.OnEvent += OnUIClose;
    }

    private IEnumerator Start()
    {
        // 로컬 플레이어가 HasInputAuthority인 네트워크 오브젝트를 찾을 때까지 대기
        while (player == null)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
            {
                var netObj = go.GetComponent<NetworkObject>();
                if (netObj != null && netObj.HasInputAuthority)
                {
                    player = go.transform;
                    break;
                }
            }
            yield return null;
        }

        selectedItems = GetRandomItems(2);
    }

    private void Update()
    {
        if (player == null) return;

        if (Vector3.Distance(transform.position, player.position) > interactionDistance)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOpened) CloseBox();
            else OpenBox();
        }
    }

    private void OnDisable()
    {
        EventBus<ItemBoxUIClose>.OnEvent -= OnUIClose;
    }

    private void OpenBox()
    {
        isOpened = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EventBus<ItemBoxOpened>.Raise(new ItemBoxOpened(selectedItems, gameObject));
    }

    private void CloseBox()
    {
        isOpened = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        RPC_RequestDespawn();

        // 2) UI 닫힘 이벤트
        EventBus<ItemBoxUIClose>.Raise(new ItemBoxUIClose(gameObject));
    }

    // 서버(StateAuthority)에서 실행되어 네트워크 오브젝트를 제거
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestDespawn()
    {
        // 서버(StateAuthority)에서만 실행됩니다
        Runner.Despawn(Object);
    }

    private void OnUIClose(ItemBoxUIClose evt)
    {
        // 만약 직접 로컬에서 Destroy 해야 할 다른 게임오브젝트가 있으면 여기서 처리
    }

    private List<ItemData> GetRandomItems(int count)
    {
        var pool = new List<ItemData>(possibleItems);
        var result = new List<ItemData>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }
}
