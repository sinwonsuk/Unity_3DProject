using Fusion;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class InventorySlotUI : NetworkBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

    private int index;
    private BigInventoryUI bigInventoryUI;

    private Transform originalParent;
    private DraggedIcon draggedIcon;
    private InventorySlot slot;
    private bool wasSelected = false; // 이전 선택 상태
    private PlayerWeaponChanged changed;

    Coroutine coroutine;

    public void SetSlot(InventorySlot slot, bool isSelected = false)
    {
        this.slot = slot;

        if (slot.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = slot.item.itemIcon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }

        // 선택 여부 갱신
        highlightImage.enabled = isSelected;

        // 선택 상태 변경 감지
        if (wasSelected != isSelected)
        {
            wasSelected = isSelected;
            OnSelectionChanged(isSelected); // 여기서 원하는 함수 실행
        }
    }

    public void Initialize(BigInventoryUI ownerUI, int slotIndex, DraggedIcon icon)
    {
        bigInventoryUI = ownerUI;
        index = slotIndex;
        draggedIcon = icon;
        changed =FindLocalPlayerWeaponChanged();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bigInventoryUI.OnSlotClicked(index);
        if (eventData.button == PointerEventData.InputButton.Right && slot.item != null)
        {
            EventBus<SendItem>.Raise(new SendItem(slot.item));
            slot.item = null;
            bigInventoryUI.UpdateSlotUI(index);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!iconImage.enabled) return;

        eventData.pointerDrag = gameObject;
        draggedIcon.gameObject.SetActive(true);
        draggedIcon.SetIcon(iconImage.sprite);
        draggedIcon.StartDrag(iconImage.sprite);
    }
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 아이콘이 자동으로 마우스를 따라감 (Update에서 처리됨)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedIcon.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI draggedSlotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlotUI != null && draggedSlotUI != this)
        {
            bigInventoryUI.SwapSlots(draggedSlotUI.index, this.index);
        }
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RPC_ChangeWeapon(ItemState state, RpcInfo info = default)
    //{
    //    EventBus<WeaponChange>.Raise(new WeaponChange(state));
    //}

    IEnumerator Cor()
    {
        while (true)
        {
            if (changed == null)
                changed = FindLocalPlayerWeaponChanged();
            else
                yield break;


            yield return null;

        }
    }


    private void OnSelectionChanged(bool isSelected)
    {

        if(coroutine == null)
            StartCoroutine(Cor());


        if (changed == null)
            return;

        if (isSelected)
        {
            Debug.Log($"[{index}] 슬롯이 선택됨: {slot.item?.itemName}");

            if (slot.item == null)
            {
                //RPC_ChangeWeapon(ItemState.none);
                Debug.Log("선택된 아이템이 없음");
            }

            else if (slot.item.weaponType==WeaponType.Sword)
            {
                changed.ChangeWeapon(ItemState.Sword);

            }
            else if(slot.item.weaponType==WeaponType.Bow)
            {
                changed.ChangeWeapon(ItemState.Bow);

            }
            else if(slot.item.weaponType==WeaponType.Axe)
            {
                changed.ChangeWeapon(ItemState.Harberd);

            }
            else if(slot.item.potionType!=PotionType.NONE)
            {
               changed.ChangeWeapon(ItemState.none);

            }
            else if(slot.item.magicType==MagicType.Fire)
            {
                changed.ChangeWeapon(ItemState.FireMagic);
 
            }
            else if (slot.item.magicType == MagicType.Ice)
            {
                changed.ChangeWeapon(ItemState.IceMagic);

            }
            else if (slot.item.magicType == MagicType.Lightning)
            {
                changed.ChangeWeapon(ItemState.ElectricMagic);

            }

        }
        else
        {
            Debug.Log($"[{index}] 슬롯 선택 해제");
        }
    }

    public PlayerWeaponChanged FindLocalPlayerWeaponChanged()
    {
        PlayerWeaponChanged[] players = FindObjectsByType<PlayerWeaponChanged>(FindObjectsSortMode.None);
        // ���� �����ϴ� ��� PlayerWeaponController �� ���� ���� ��ü ����
        foreach (var playerWeapon in players)
        {
            if (playerWeapon.Object != null && playerWeapon.Object.HasInputAuthority)
                return playerWeapon;
        }
        return null;
    }

}
