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

public class InventorySlotUI : NetworkBehaviour
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

    void Update()
    {
        HandleRightClick();
        
    }
    public void HandleRightClick()
    {
        if (draggedIcon == null || draggedIcon.transform == null)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (slot.item != null)
            {
                bigInventoryUI.OnSlotClicked(index);
                bigInventoryUI.UpdateSlotUI(index);
                isRightDragging = true;
                draggedIcon.gameObject.SetActive(true);
                draggedIcon.SetIcon(iconImage.sprite);
                Debug.Log($"canCombi : {canCombi}");
            }

            // 조합 처리
            if (slot.item != null && canCombi)
            {
                bigInventoryUI.OnSlotClicked(index);
                EventBus<SendItem>.Raise(new SendItem(slot.item));
                slot.item = null;
                bigInventoryUI.UpdateSlotUI(index);
            }
        }

        if (isRightDragging && Input.GetMouseButton(1) && slot.item != null)
        {
            draggedIcon.transform.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1) && isRightDragging)
        {
            isRightDragging = false;
            draggedIcon.gameObject.SetActive(false);

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> rayResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, rayResults);

            foreach (RaycastResult result in rayResults)
            {
                InventorySlotUI targetSlot = result.gameObject.GetComponent<InventorySlotUI>();
                if (targetSlot != null && targetSlot != this)
                {
                    bigInventoryUI.SwapSlots(index, targetSlot.index);
                    break;
                }
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
