using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Slot Components")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI slotNumberText;
    [SerializeField] private Image selectionOutline;

    private int slotNumber;

    public void SetSlotNumber(int number)
    {
        slotNumber = number;
        if (slotNumberText != null)
        {
            slotNumberText.text = number.ToString();
        }
    }

    public void UpdateSlot(Item item, bool isSelected, Color normalColor, Color selectedColor, Color emptyColor)
    {
        // Update item icon
        if (item != null && item.itemIcon != null)
        {
            itemIconImage.sprite = item.itemIcon;
            itemIconImage.color = Color.white;
            itemIconImage.enabled = true;
            backgroundImage.color = normalColor;
        }
        else
        {
            itemIconImage.enabled = false;
            backgroundImage.color = emptyColor;
        }

        // Update selection indicator
        if (selectionOutline != null)
        {
            selectionOutline.enabled = isSelected;
            if (isSelected)
            {
                selectionOutline.color = selectedColor;
            }
        }
        else
        {
            // If no outline, change background color when selected
            if (isSelected && item != null)
            {
                backgroundImage.color = selectedColor;
            }
        }
    }
}