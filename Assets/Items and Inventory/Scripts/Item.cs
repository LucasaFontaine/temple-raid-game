using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName;
    public Sprite itemIcon;
    [TextArea]
    public string itemDescription;
    
    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
    }
}