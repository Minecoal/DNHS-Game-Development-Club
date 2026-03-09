using UnityEngine;
using UnityEngine.UI;

public class InventoryUIPresenter : MonoBehaviour
{
    private GameObject[] slots;
    private GameObject[] equipmentSlots;
    private GameObject primaryWeaponSlotGameObject;
    private GameObject secondaryWeaponSlotGameObject;
    [SerializeField] private TMPro.TextMeshProUGUI currencyText;

    private InventoryStorageService storageService;
    private EquipmentService equipmentService;
    private DropService dropService;

    public void Initialize(InventoryStorageService storage, EquipmentService equipment, DropService drop, 
        GameObject[] slotGameObjects, GameObject[] equipmentSlotGameObjects, GameObject primaryWeaponSlot, GameObject secondaryWeaponSlot)
    {
        this.storageService = storage;
        this.equipmentService = equipment;
        this.dropService = drop;
        
        this.slots = slotGameObjects;
        this.equipmentSlots = equipmentSlotGameObjects;
        this.primaryWeaponSlotGameObject = primaryWeaponSlot;
        this.secondaryWeaponSlotGameObject = secondaryWeaponSlot;

        InventoryEvents.InventoryRefreshed += RefreshUI;
        InventoryEvents.CurrencyAdded += OnCurrencyChanged;
        InventoryEvents.CurrencyRemoved += OnCurrencyChanged;
    }

    private void OnDestroy()
    {
        InventoryEvents.InventoryRefreshed -= RefreshUI;
        InventoryEvents.CurrencyAdded -= OnCurrencyChanged;
        InventoryEvents.CurrencyRemoved -= OnCurrencyChanged;
    }

    public void RefreshUI()
    {
        RefreshInventorySlots();
        RefreshEquipmentSlots();
        RefreshWeaponSlots();
        RefreshCurrency();
    }

    private void RefreshInventorySlots()
    {
        ItemSlot[] items = storageService.GetItems();

        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                if (items[i].GetItem() != null)
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;
                    if (items[i].GetItem().isStackable)
                        slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = items[i].GetQuantity() + "";
                    else
                        slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                }
                else
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                }
            }
            catch
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
    }

    private void RefreshEquipmentSlots()
    {
        ItemSlot[] equipment = equipmentService.GetEquipmentSlots();

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            try
            {
                if (equipment[i].GetItem() != null)
                {
                    equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = equipment[i].GetItem().itemIcon;
                    equipmentSlots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                }
                else
                {
                    equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                    equipmentSlots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                }
            }
            catch
            {
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                equipmentSlots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
    }

    private void RefreshWeaponSlots()
    {
        // primary weapon
        try
        {
            ItemSlot primaryWeapon = equipmentService.GetPrimaryWeapon();
            if (primaryWeapon.GetItem() != null)
            {
                primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
                primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = primaryWeapon.GetItem().itemIcon;
                primaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
            else
            {
                primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
                primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
                primaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
        catch
        {
            primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
            primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
            primaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }

        // secondary weapon
        try
        {
            ItemSlot secondaryWeapon = equipmentService.GetSecondaryWeapon();
            if (secondaryWeapon.GetItem() != null)
            {
                secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
                secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = secondaryWeapon.GetItem().itemIcon;
                secondaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
            else
            {
                secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
                secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
                secondaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }
        catch
        {
            secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
            secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
            secondaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
    }

    private void RefreshCurrency()
    {
        currencyText.text = dropService.GetCurrency().ToString();
    }

    private void OnCurrencyChanged(int amount)
    {
        RefreshCurrency();
    }
}
