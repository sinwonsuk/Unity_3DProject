using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectionManager : NetworkBehaviour
{
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        readyButton.onClick.AddListener(ConfirmSelection);
    }

    public void SetSelectedIndex(int index)
    {
        SelectedIndex = index;
        if (index >= 0 && index < characterTypes.Length)
        {
            Debug.Log($"ĳ���� ���õ�: {characterTypes[index]} (index: {index})");
        }
        else
        {
            Debug.LogWarning($"�߸��� ĳ���� �ε���: {index}");
        }
    }

    private void ConfirmSelection()
    {
        if (IsReady || SelectedIndex < 0) return;

        IsReady = true;
        Debug.Log($"���� ĳ���� Ȯ��: {characterTypes[SelectedIndex]}");
        readyButton.interactable = false;
        MatchQueueManager.Instance.MySelectedCharacterName = SelectedCharacterName;
    }

    public void CancelSelection()
    {
        if (!IsReady && !LobbyCameraManager.Instance.IsZoomedIn) return;

        IsReady = false;
        SelectedIndex = -1;
        readyButton.interactable = true;

        Debug.Log("ĳ���� ���� ��ҵ�");

        //��Ī���
        if (AutoMatchManager.Instance != null)
        {
            AutoMatchManager.Instance.CancelMatch();
        }

        LobbyCameraManager.Instance.ReturnToOriginalPosition();
        LobbyUIManager.Instance.HideZoomUI();
    }

    [SerializeField] private Button readyButton;

    private string[] characterTypes = {
        "King_Mccully",    
        "Mage_Evan",       
        "Joker",           
        "Queen_Emily",    
        "Soldier_James"  
    };

    public int SelectedIndex { get; private set; } = -1;
    public bool IsReady { get; private set; }
    public string SelectedCharacterName => (SelectedIndex >= 0 && SelectedIndex < characterTypes.Length)
                                           ? characterTypes[SelectedIndex]
                                           : null;
    public static CharacterSelectionManager Instance { get; private set; }
}
