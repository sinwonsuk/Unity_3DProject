using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectionManager : NetworkBehaviour
{

    private void Start()
    {
        readyButton.onClick.AddListener(ConfirmSelection);
    }


    private void ConfirmSelection()
    {
        if (IsReady) return;

        IsReady = true;

        Debug.Log($"ĳ���� ���� �Ϸ�: {characterTypes[SelectedIndex]} (index: {SelectedIndex})");

        readyButton.interactable = false;

    }

    public void CancelSelection()
    {
        if (!IsReady && !LobbyCameraManager.Instance.IsZoomedIn) return;

        IsReady = false;
        Debug.Log("ĳ���� ���� ��ҵ�");

        readyButton.interactable = true;
        SelectedIndex = 0;

        LobbyCameraManager.Instance.ReturnToOriginalPosition();
        LobbyUIManager.Instance.HideZoomUI();
    }

    [SerializeField] private Button readyButton;

    public int SelectedIndex { get; private set; }
    public bool IsReady { get; private set; }

    private string[] characterTypes = { "Warrior", "Mage", "Archer" };
}
