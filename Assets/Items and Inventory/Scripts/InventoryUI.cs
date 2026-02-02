using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0f, 1f);
    [SerializeField] private Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();

    private void Start()
    {
        CreateSlots();
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged += UpdateUI;
        }

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged -= UpdateUI;
        }
    }

    private void CreateSlots()
    {
        if (InventoryManager.Instance == null) return;

        int maxSlots = InventoryManager.Instance.GetMaxSlots();

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            
            if (slotUI != null)
            {
                slotUI.SetSlotNumber(i + 1);
                slotUIList.Add(slotUI);
            }
        }
    }

    private void UpdateUI()
    {
        if (InventoryManager.Instance == null) return;

        int selectedSlot = InventoryManager.Instance.GetCurrentSelectedSlot();

        for (int i = 0; i < slotUIList.Count; i++)
        {
            Item item = InventoryManager.Instance.GetItemAtSlot(i);
            bool isSelected = i == selectedSlot;

            slotUIList[i].UpdateSlot(item, isSelected, normalColor, selectedColor, emptyColor);
        }
    }
}