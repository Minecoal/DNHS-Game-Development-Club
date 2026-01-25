using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ShopManager shopKeeper;

    private ShopItemClass shopItem;


    public void OnPointerClick(PointerEventData eventData)
    {
        shopKeeper.SelectShopItem(shopItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //CraftingManager.Instance.SelectRecipe(recipe);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void SetShopItem(ShopItemClass shopItem, ShopManager shopKeeper)
    {
        this.shopItem = shopItem;
        this.shopKeeper = shopKeeper;
    }

    public ShopItemClass GetRecipe()
    {
        return shopItem;
    }
}
