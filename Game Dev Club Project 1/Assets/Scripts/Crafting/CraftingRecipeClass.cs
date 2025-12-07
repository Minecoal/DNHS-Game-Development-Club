using UnityEngine;

[CreateAssetMenu(fileName ="newCraftingRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipeClass : ScriptableObject
{
    [Header("Crafting Recipe")]
    public ItemSlot[] inputItems;
    public ItemSlot outputItem;

    public bool CanCraft()
    {
        InventoryManager inventory = InventoryManager.Instance;

        for(int i = 0; i < inputItems.Length; i++)
        {
            if(!inventory.Contains(inputItems[i].GetItem(), inputItems[i].GetQuantity()))
            {
                return false;
            }
        }

        return true;
    }

    public void Craft(int amountMultiplier)
    {
        InventoryManager inventory = InventoryManager.Instance;

        for (int i = 0; i < inputItems.Length; i++)
        {
            inventory.RemoveItem(inputItems[i].GetItem(), inputItems[i].GetQuantity() * amountMultiplier);
        }

        inventory.AddItem(outputItem.GetItem(), outputItem.GetQuantity() * amountMultiplier);
    }
}
