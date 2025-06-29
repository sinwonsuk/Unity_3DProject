using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class InventorySlotUI : NetworkBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
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
    private bool isRightDragging = false;
    private bool canCombi=false;

    Coroutine coroutine;

    public void SetSlot(InventorySlot slot, bool isSelected = false)
    {
        this.slot = slot;

        Debug.Log($"[SetSlot] {index}번 슬롯 - 선택됨: {isSelected}, 이전 상태: {wasSelected}");

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
            Debug.Log($"[SetSlot] 선택 상태 변경 감지 -> OnSelectionChanged 호출");
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

    private void OnEnable()
    {
        EventBus<YesCombi>.OnEvent += UpdateCanCombi;
    }

    private void OnDisable()
    {
        EventBus<YesCombi>.OnEvent -= UpdateCanCombi;
    }

    private void UpdateCanCombi(YesCombi evt)
    {
        this.canCombi = evt.canCombi;
    }
    public void RightClick()
    {
       

            bigInventoryUI.OnSlotClicked(index);
            bigInventoryUI.UpdateSlotUI(index);

            if(canCombi)
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
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null || slot.item == null)
            return;

        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    // 좌클릭 → 선택 상태 변경 요청 (bigInventoryUI에게)
        //    bigInventoryUI.OnSlotClicked(index);
        //}
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 우클릭 → 조합 이벤트 발송 (조합 가능할 때만)
            if (canCombi)
            {
                EventBus<SendItem>.Raise(new SendItem(slot.item));
                slot.item = null;
                bigInventoryUI.UpdateSlotUI(index);
            }
            else
            {
                bigInventoryUI.OnSlotClicked(index);
                bigInventoryUI.UpdateUI();
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 아이콘이 자동으로 마우스를 따라감 (Update에서 처리됨)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.itemDrop, false);
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
        Debug.Log("플레이어 찾는 코루틴 시작");
        while (true)
        {
            if (changed == null)
                changed = FindLocalPlayerWeaponChanged();
            else
            {
                Debug.Log("플레이어를 찾았음. 코루틴 종료");
                yield break;
            }
                


            yield return null;

        }
    }

    public void ForceSelect()
    {
        OnSelectionChanged(true);
        highlightImage.enabled = true;
        wasSelected = true;
    }

    private void OnSelectionChanged(bool isSelected)
    {

        if (coroutine == null)
            StartCoroutine(Cor());


        if (changed == null)
            return;

        if (isSelected)
        {
            //Debug.Log($"[{index}] 슬롯이 선택됨: {slot.item?.itemName}");

            if (slot.item == null)
            {
                //RPC_ChangeWeapon(ItemState.none);
               // Debug.Log("선택된 아이템이 없음");
            }

            else if (slot.item.weaponType == WeaponType.Sword)
            {
                changed.ChangeWeapon(ItemState.Sword, slot.item.itemGrade);

            }
            else if (slot.item.weaponType == WeaponType.Bow)
            {
                changed.ChangeWeapon(ItemState.Bow, slot.item.itemGrade);

            }
            else if (slot.item.weaponType == WeaponType.Axe)
            {
                changed.ChangeWeapon(ItemState.Harberd, slot.item.itemGrade);

            }
            else if (slot.item.magicType == MagicType.Fire)
            {
                changed.ChangeWeapon(ItemState.FireBall, slot.item.itemGrade);
            }
            else if (slot.item.magicType == MagicType.Ice)
            {
                changed.ChangeWeapon(ItemState.IceBall, slot.item.itemGrade);

            }
            else if (slot.item.magicType == MagicType.Lightning)
            {
                changed.ChangeWeapon(ItemState.ElectricBall, slot.item.itemGrade);

            }


            else
            {
               // Debug.Log($"[{index}] 슬롯 선택 해제");
            }
        }
    }

    

    public PlayerWeaponChanged FindLocalPlayerWeaponChanged()
    {
        PlayerWeaponChanged[] players = FindObjectsByType<PlayerWeaponChanged>(FindObjectsSortMode.None);

        foreach (var playerWeapon in players)
        {
            if (playerWeapon.Object != null && playerWeapon.Object.HasInputAuthority)
                return playerWeapon;
        }
        return null;
    }

}
