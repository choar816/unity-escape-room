using System;
using System.Collections.Generic;
using System.Linq;

public class ItemCombinationTable
{
    private Dictionary<List<EnumItem>, EnumItem> combinationMap;

    public ItemCombinationTable()
    {
        combinationMap = new Dictionary<List<EnumItem>, EnumItem>(new ListComparer<EnumItem>());
        combinationMap.Add(new List<EnumItem> { EnumItem.Clock_Part_1, EnumItem.Clock_Part_2, EnumItem.Clock_Part_3 }, EnumItem.Clock_Complete);
    }

    public bool CanCombineItems(List<EnumItem> selectedItems, out EnumItem resultItem)
    {
        resultItem = EnumItem.None;

        // 선택된 아이템이 없음
        if (selectedItems == null || selectedItems.Count == 0)
        {
            return false;
        }

        // 선택된 아이템 목록을 정렬하여 Dictionary에서 검색 가능한 키로 사용
        selectedItems.Sort();

        if (combinationMap.TryGetValue(selectedItems, out EnumItem combinedItem))
        {
            resultItem = combinedItem; // 합성 결과 아이템
            return true;
        }

        return false; // 합성 불가능
    }
}

// List<EnumItem>을 비교하기 위한 커스텀 Comparer 클래스
public class ListComparer<T> : IEqualityComparer<List<T>>
{
    public bool Equals(List<T> x, List<T> y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Count == y.Count && x.All(item => y.Contains(item));
    }

    public int GetHashCode(List<T> obj)
    {
        if (obj == null)
        {
            return 0;
        }

        int hashCode = 0;
        foreach (T item in obj)
        {
            hashCode ^= item.GetHashCode();
        }
        return hashCode;
    }
}

// 합성 실패 이유
public enum CombineFailureReason
{
    OneItem,
    NoCombination,
}

public class CombineFailureReasonMessages
{
    public Dictionary<CombineFailureReason, string> Messages { get; }

    public CombineFailureReasonMessages()
    {
        Messages = new Dictionary<CombineFailureReason, string>
        {
            { CombineFailureReason.OneItem, "합성은 2개 이상의 아이템으로 할 수 있습니다." },
            { CombineFailureReason.NoCombination, "합성할 수 없는 아이템입니다." },
        };
    }

    public string GetMessage(CombineFailureReason reason)
    {
        if (Messages.TryGetValue(reason, out string message))
        {
            return message;
        }
        else
        {
            return "알 수 없는 실패 이유입니다.";
        }
    }
}