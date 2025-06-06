using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button readyButton;
    //[SerializeField] private Image readyButtonImage;
    //[SerializeField] private Sprite readyOffSprite;
    //[SerializeField] private Sprite readyOnSprite;

    [Networked] public int SelectedIndex { get; private set; }
    [Networked] public bool IsReady { get; private set; }

    private string[] characterTypes = { "Warrior", "Mage", "Archer" }; 

    private void Start()
    {
        leftButton.onClick.AddListener(() => ChangeCharacter(-1));
        rightButton.onClick.AddListener(() => ChangeCharacter(1));
        readyButton.onClick.AddListener(ConfirmSelection);

        UpdateCharacterVisual();
    }

    private void ChangeCharacter(int direction)
    {
        if (IsReady) return;

        SelectedIndex += direction;

        if (SelectedIndex < 0) SelectedIndex = characterSprites.Length - 1;
        if (SelectedIndex >= characterSprites.Length) SelectedIndex = 0;

        UpdateCharacterVisual();
    }

    private void UpdateCharacterVisual()
    {
        if (SelectedIndex >= 0 && SelectedIndex < characterSprites.Length)
        {
            profileImage.sprite = characterSprites[SelectedIndex];
        }
    }

    private void ConfirmSelection()
    {
        if (IsReady) return;

        IsReady = true;

        Debug.Log($"캐릭터 선택 완료: {characterTypes[SelectedIndex]} (index: {SelectedIndex})");

        leftButton.interactable = false;
        rightButton.interactable = false;
        readyButton.interactable = false;

       // if (readyButtonImage != null && readyOnSprite != null)
        //    readyButtonImage.sprite = readyOnSprite;

    }
}
