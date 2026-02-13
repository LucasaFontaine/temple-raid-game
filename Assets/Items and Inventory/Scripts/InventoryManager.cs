using UnityEngine;
using Photon.Pun;

public class InventoryManager : MonoBehaviourPun
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 5;
    
    private Item[] inventorySlots;
    private int currentSelectedSlot = 0;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChanged;

    private void Awake()
    {
        // Only create instance for the local player
        if (photonView.IsMine)
        {
            if (Instance == null)
            {
                Instance = this;
                inventorySlots = new Item[maxSlots];
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Disable this script on remote players
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleInventoryInput();
        }
    }

    private void HandleInventoryInput()
    {
        // Number keys 1-5 to select slots
        for (int i = 0; i < maxSlots; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }

        // Use selected item
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            UseSelectedItem();
        }

        // Drop selected item (changed to G key)
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropSelectedItem();
        }
    }

    public bool AddItem(Item item)
    {
        // Find first empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = item;
                currentSelectedSlot = i;
                onInventoryChanged?.Invoke();
                Debug.Log($"Added {item.itemName} to slot {i + 1}");
                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            inventorySlots[slotIndex] = null;
            onInventoryChanged?.Invoke();
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            currentSelectedSlot = slotIndex;
            onInventoryChanged?.Invoke();
            
            if (inventorySlots[slotIndex] != null)
            {
                Debug.Log($"Selected slot {slotIndex + 1}: {inventorySlots[slotIndex].itemName}");
            }
        }
    }

    public void UseSelectedItem()
    {
        Item item = GetSelectedItem();
        if (item != null)
        {
            item.Use();
            // Optional: Remove item after use
            // RemoveItem(currentSelectedSlot);
        }
    }

    public void DropSelectedItem()
    {
        Item item = GetSelectedItem();
        if (item != null)
        {
            // Position it in front of the player
            Camera mainCam = Camera.main;
            Vector3 dropPosition = transform.position + transform.forward * 2f;
            
            if (mainCam != null)
            {
                dropPosition = mainCam.transform.position + mainCam.transform.forward * 2f;
            }

            // Use networked drop if item supports it
            PickupableItem pickupable = item as PickupableItem;
            if (pickupable != null)
            {
                pickupable.NetworkedDrop(dropPosition);
            }
            else
            {
                // Fallback for non-networked items
                item.gameObject.SetActive(true);
                item.transform.position = dropPosition;
            }

            RemoveItem(currentSelectedSlot);
            Debug.Log($"Dropped {item.itemName}");
        }
    }

    public Item GetSelectedItem()
    {
        return inventorySlots[currentSelectedSlot];
    }

    public Item GetItemAtSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
        {
            return inventorySlots[slotIndex];
        }
        return null;
    }

    public int GetCurrentSelectedSlot()
    {
        return currentSelectedSlot;
    }

    public int GetMaxSlots()
    {
        return maxSlots;
    }
}
