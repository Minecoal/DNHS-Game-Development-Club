using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private InventoryManager inventory;

    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private GameObject shopItemHolder;

    [SerializeField] private Transform purchaseItemPopup;

    [SerializeField] private ShopItemClass[] shopItems;

    [SerializeField] private Slider amountSlider;
    private TMPro.TextMeshProUGUI minAmountText;
    private TMPro.TextMeshProUGUI maxAmountText;
    private TMPro.TextMeshProUGUI totalPriceText;

    [SerializeField] private TMPro.TextMeshProUGUI currencyText;

    private GameObject[] shopItemGameObjects;

    private int mostPurchasableAmount = 0;
    private int numberToBuy = 1;

    private ShopItemClass selectedShopItem;

    [SerializeField] private GameObject shopUICanvas; //Canvas
    private bool previousCursorState;
    private bool isShopOpen = false;

    void Start()
    {
        inventory = InventoryManager.Instance;

        shopItemGameObjects = new GameObject[shopItems.Length];

        for(int i = 0; i < shopItems.Length; i++)
        {
            GameObject shopItemButton = Instantiate(shopItemPrefab, shopItemHolder.transform, false);

            shopItemButton.transform.Find("Image").GetComponent<Image>().sprite = shopItems[i].shopItem.GetItem().itemIcon;
            shopItemButton.transform.Find("Item Name").GetComponent<TMPro.TextMeshProUGUI>().text = shopItems[i].shopItem.GetItem().itemName;
            shopItemButton.transform.Find("Cost").GetComponent<TMPro.TextMeshProUGUI>().text = shopItems[i].cost.ToString();
            shopItemButton.GetComponent<ShopItemUI>().SetShopItem(shopItems[i], this);

            if(inventory.currency < shopItems[i].cost)
            {
                shopItemButton.GetComponent<Button>().interactable = false;
            }

            shopItemGameObjects[i] = shopItemButton;
        }

        minAmountText = purchaseItemPopup.Find("Min Amount Purchasable").GetComponent<TMPro.TextMeshProUGUI>();
        maxAmountText = purchaseItemPopup.Find("Max Amount Purchasable").GetComponent<TMPro.TextMeshProUGUI>();
        totalPriceText = purchaseItemPopup.Find("Total Price Text").GetComponent<TMPro.TextMeshProUGUI>();

        currencyText.text = inventory.currency.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleShopUI();
        }
    }

    public void SelectShopItem(ShopItemClass shopItem)
    {
        if(inventory.currency >= shopItem.cost)
        {
            selectedShopItem = shopItem;
            numberToBuy = 1;
            amountSlider.value = 1;
            purchaseItemPopup.gameObject.SetActive(true);

            //inventory.RemoveCurrency(shopItem.cost);
            //inventory.AddItem(shopItem.shopItem.GetItem(), shopItem.shopItem.GetQuantity());


            SetSlider(shopItem);

            purchaseItemPopup.Find("Purchase Item Image").GetComponent<Image>().sprite = shopItem.shopItem.GetItem().itemIcon;
            purchaseItemPopup.Find("Output Amount Text").GetComponent<TMPro.TextMeshProUGUI>().text = (shopItem.shopItem.GetQuantity() * numberToBuy).ToString();
            totalPriceText.text = (shopItem.cost * numberToBuy).ToString();

        }
    }

    private void SetSlider(ShopItemClass shopItem)
    {
        mostPurchasableAmount = Mathf.FloorToInt(inventory.currency / shopItem.cost);

        if (mostPurchasableAmount == 1)
        {
            amountSlider.maxValue = 2;
            amountSlider.interactable = false;
        }
        else
        {
            amountSlider.maxValue = mostPurchasableAmount;
            amountSlider.interactable = true;
        }

        minAmountText.text = "1";
        maxAmountText.text = mostPurchasableAmount.ToString();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            GameObject shopItemButton = shopItemGameObjects[i];

            shopItemButton.GetComponent<Button>().interactable = inventory.currency >= shopItems[i].cost;
        }

        currencyText.text = inventory.currency.ToString();
    }

    public void OnSliderChange()
    {
        numberToBuy = (int)(amountSlider.value);
        //SelectShopItem(selectedShopItem);
        purchaseItemPopup.Find("Output Amount Text").GetComponent<TMPro.TextMeshProUGUI>().text = (selectedShopItem.shopItem.GetQuantity() * numberToBuy).ToString();
        totalPriceText.text = (selectedShopItem.cost * numberToBuy).ToString();
    }
    
    public void Buy()
    {
        inventory.RemoveCurrency(selectedShopItem.cost * numberToBuy);
        inventory.AddItem(selectedShopItem.shopItem.GetItem(), selectedShopItem.shopItem.GetQuantity() * numberToBuy);

        purchaseItemPopup.gameObject.SetActive(false);
        RefreshUI();
        numberToBuy = 1;
    }

    public void ToggleShopUI()
    {
        if (isShopOpen)
        {
            CloseShopUI();
        }
        else
        {
            OpenShopUI();
        }
    }

    public void CloseShopUI()
    {

        isShopOpen = false;
        shopUICanvas?.SetActive(false);
        Cursor.visible = previousCursorState;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void OpenShopUI()
    {
        isShopOpen = true;
        shopUICanvas?.SetActive(true);
        previousCursorState = Cursor.visible;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        RefreshUI();
    }
}
