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
            Debug.Log($"캐릭터 선택됨: {characterTypes[index]} (index: {index})");
        }
        else
        {
            Debug.LogWarning($"잘못된 캐릭터 인덱스: {index}");
        }
    }

    private void ConfirmSelection()
    {
        if (IsReady || SelectedIndex < 0) return;

        IsReady = true;
        Debug.Log($"최종 캐릭터 확정: {characterTypes[SelectedIndex]}");
        readyButton.interactable = false;
        MatchQueueManager.Instance.MySelectedCharacterName = SelectedCharacterName;
    }

    public void CancelSelection()
    {
        if (!IsReady && !LobbyCameraManager.Instance.IsZoomedIn) return;

        IsReady = false;
        SelectedIndex = -1;
        readyButton.interactable = true;

        Debug.Log("캐릭터 선택 취소됨");

        //매칭취소
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
