using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemComposition : MonoBehaviour
{
    [SerializeField] private List<CombinationSlotUI> slots = new List<CombinationSlotUI>();
    [SerializeField] private List<CombinationRecipe> recipes;
    [SerializeField] private CombinationSlotUI centerSlot;

    private void OnEnable()
    {
        EventBus<SendItem>.OnEvent += TryComposition;
    }

    private void OnDisable()
    {
        EventBus<SendItem>.OnEvent -= TryComposition;
    }

    private void TryComposition(SendItem newItem)
    {
        foreach(var slot in slots)
        {
            if(slot.cItem==null)
            {
                //SoundManager.GetInstance().SfxPlay(SoundManager.sfx.itemDrop, false);
                slot.cItem = newItem.item;
                slot.cImage.sprite = newItem.item.itemIcon;
                break;
            }
        }
        // 슬롯이 전부 찼는지 확인
        bool allFilled = slots.TrueForAll(s => s.cItem != null);

        if (allFilled)
        {
            Debug.Log("슬롯이 모두 찼습니다. 조합을 시도합니다.");
            ComposeItems();
        }
    }

    private void ComposeItems()
    {
        List<ItemData> inputItems = slots
            .Where(s => s.cItem != null) // null 방어
            .Select(s => s.cItem)
            .ToList();

        if (inputItems.Count != slots.Count || centerSlot.cItem != null)
        {
            Debug.Log("실패");
            return;
        }


        foreach (var recipe in recipes)
        {
            if (IsRecipeMatch(recipe, inputItems))
            {
                Debug.Log($"[조합 성공] 결과 아이템: {recipe.result.itemName}");

                if (centerSlot != null)
                {
                    SoundManager.GetInstance().SfxPlay(SoundManager.sfx.itemUpgrade, false);
                    centerSlot.cItem = recipe.result;
                    centerSlot.cImage.sprite = recipe.result.itemIcon;
                }

                ClearSlots();
                return;
            }
        }

        Debug.Log("[조합 실패] 레시피 없음");

    }


    private bool IsRecipeMatch(CombinationRecipe recipe, List<ItemData> inputItems)
    {
        if (recipe.ingredients.Count != inputItems.Count)
            return false;

        var sortedRecipe = recipe.ingredients.OrderBy(i => i.itemId).ToList();
        var sortedInput = inputItems.OrderBy(i => i.itemId).ToList();

        for (int i = 0; i < sortedInput.Count; i++)
        {
            if (sortedRecipe[i] != sortedInput[i])
                return false;
        }

        return true;
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            slot.CSlotClear();
        }
    }
}
