using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{

    public void ShowInventoryUI()
    {
        Inventory.Instance.UpdateItemSlots();
        InventoryUI.SetActive(true);
        if (!hasInventoryOpened)
        {
            hasInventoryOpened = true;
            Inventory.Instance.SelectFirstItem();
        }
        PushUIStack(InventoryUI);
    }

    public void ShowItemInfoUI()
    {
        ItemInfoUI.SetActive(true);
    }

    public void HideItemInfoUI()
    {
        ItemInfoUI.SetActive(false);
    }

    public void ShowItemFoundPopupUI()
    {
        ItemFoundPopupUI.SetActive(true);
        isItemFoundPopupUIOpen = true;
    }

    public void HideItemFoundPopupUI()
    {
        ItemFoundPopupUI.SetActive(false);
        isItemFoundPopupUIOpen = false;
    }

    public void ShowItemFoundFullUI()
    {
        ItemFoundFullUI.SetActive(true);
    }

    public void HideItemFoundFullUI()
    {
        ItemFoundFullUI.SetActive(false);
    }

    public void ShowCompoundItemUI()
    {
        PushUIStack(ItemCompoundTryUI);
    }

    public void ShowCompoundSuccessUI(string spriteName, string itemName)
    {
        ResultItemSprite.spriteName = spriteName;
        ResultItemName.text = itemName;
        ItemCompoundSuccessUI.SetActive(true);
    }

    public void ShowCompoundFailureUI(CombineFailureReason reason)
    {
        CombineFailureReasonMessages failureMessages = new CombineFailureReasonMessages();
        CompoundFailureLabel.text = failureMessages.GetMessage(reason);
        ItemCompoundFailureUI.SetActive(true);
    }

    public void HideCompoundResultUI()
    {
        ItemCompoundTryUI.SetActive(false);
        ItemCompoundSuccessUI.SetActive(false);
        ItemCompoundFailureUI.SetActive(false);
    }

    public void ShowItemRemoveConfirmUI()
    {
        ItemRemoveConfirmUI.SetActive(true);
    }

    public void HideItemRemoveConfirmUI()
    {
        ItemRemoveConfirmUI.SetActive(false);
    }
}
