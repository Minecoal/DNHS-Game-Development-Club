using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : PersistentGenericSingleton<InventoryManager>
{
    [SerializeField] private GameObject itemCursor;

    [Header("Inventory Slots")]
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject equipmentSlotHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private ItemSlot[] startingItems;

    [SerializeField] private GameObject primaryWeaponSlotGameObject;
    [SerializeField] private GameObject secondaryWeaponSlotGameObject;

    public ItemSlot[] items;
    public ItemSlot[] equipment;
    public GameObject[] slots;
    public GameObject[] equipmentSlots;

    private ItemSlot primaryWeapon = new ItemSlot();
    private ItemSlot secondaryWeapon = new ItemSlot();

    private ItemSlot movingSlot;
    private ItemSlot tempSlot;
    private ItemSlot originalSlot;
    private bool isMovingItem;

    [SerializeField] private GameObject inventoryUIRoot; //Canvas
    [SerializeField] private KeyCode toggleKey = KeyCode.E;
    private bool previousCursorState;
    private bool isInventoryOpen = false;

    [Header("Currency")]
    public int currency = 0;
    [SerializeField] private TMPro.TextMeshProUGUI currencyText;
    [SerializeField] private CurrencyInfo[] coinData;

    public GameObject droppedItemPrefab;


    private GameObject primaryWeaponGameObject;
    private GameObject secondaryWeaponGameObject;

    private int padding = 35;// half of slot size + half of padding size

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new ItemSlot[slots.Length];
        equipmentSlots = new GameObject[equipmentSlotHolder.transform.childCount];
        equipment = new ItemSlot[equipmentSlots.Length];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new ItemSlot();
        }

        for (int i = 0; i < equipment.Length; i++)
        {
            equipment[i] = new ItemSlot();
        }

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new ItemSlot();
        }

        for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
        }

        for (int i = 0; i < slotHolder.transform.childCount; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < equipmentSlotHolder.transform.childCount; i++)
        {
            equipmentSlots[i] = equipmentSlotHolder.transform.GetChild(i).gameObject;
        }

        foreach (var item in equipment)
        {
            if (item.GetItem() != null)
                if (item.GetItem().GetEquipment() != null)
                    item.GetItem().GetEquipment().OnEquip();
        }

        //AddItem(itemToAdd, 1);
        RemoveItem(itemToRemove);

        RefreshUI();
    }

    // Update is called once per frame
    void Update()
    {
        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (isMovingItem)
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

        if (Input.GetMouseButtonDown(0))
        {
            if (isMovingItem)
            {
                EndItemMove();
            }
            else
                BeginItemMove();
        }


        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }

        if (Input.GetMouseButtonDown(2))
        {
            GetComponent<DropItem>().DropItems(new ItemSlot(startingItems[0]), PlayerManager.Instance.Player.transform.position);
            GetComponent<DropItem>().DropItems(new ItemSlot(startingItems[1]), PlayerManager.Instance.Player.transform.position);
            GetComponent<DropItem>().DropItems(new ItemSlot(startingItems[2]), PlayerManager.Instance.Player.transform.position);
        }
    }

    public void AddItem(ItemClass item, int quantity)
    {
        bool itemsAdded = false;
        int quantityLeft = quantity;

        ItemSlot slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
        {
            slot.AddQuantity(quantity);
            itemsAdded = true;
        }
        else
        {
            if (item.isStackable)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null)
                    {
                        items[i].AddItem(item, quantity);
                        itemsAdded = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null && quantityLeft > 0)
                    {
                        items[i].AddItem(item, 1);
                        quantityLeft--;
                        if (quantityLeft <= 0)
                        {
                            itemsAdded = true;
                            break;
                        }
                    }
                }
            }
            
        }

        if (!itemsAdded)
        {
            //drop items(quantityLeft)
            if(quantityLeft > 0)
                GetComponent<DropItem>().DropItems(new ItemSlot(item, quantityLeft), PlayerManager.Instance.Player.transform.position);
            Debug.Log("Drop items: " + quantityLeft + " | " + item.itemName);
        }

        RefreshUI();
    }

    public void RemoveItem(ItemClass item)
    {

        ItemSlot temp = Contains(item);
        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
                temp.SubQuantity(1);
            else
            {
                int slotToRemoveIndex = 0;

                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                items[slotToRemoveIndex].Clear();
            }
        }
        RefreshUI();
    }

    public void RemoveItem(ItemClass item, int quantity)
    {
        if (item.isStackable)
        {
            ItemSlot temp = Contains(item);
            if (temp != null)
            {
                //remove num of items from stack
                if (temp.GetQuantity() > quantity)
                    temp.SubQuantity(quantity);
                else
                {
                    //remove full stack
                    int slotToRemoveIndex = 0;

                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].GetItem() == item)
                        {
                            slotToRemoveIndex = i;
                            break;
                        }
                    }
                    items[slotToRemoveIndex].Clear();
                }
            }
        }
        else
        {
            //remove items 1 at a time if not stackable
            int quantityLeft = quantity;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == item)
                {
                    items[i].Clear();
                    quantityLeft--;
                }
                if(quantityLeft <= 0)
                {
                    break;
                }
            }
        }


        RefreshUI();
    }

    public int CanAddItem(ItemClass item, int quantity)
    {
        //returns -1 if can add item, else returns amount that can add
        bool itemsAdded = false;
        int quantityLeft = quantity;
        int quantityAdded = 0;

        ItemSlot slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
            return -1;
        else
        {
            if (item.isStackable)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null)
                    {
                        itemsAdded = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].GetItem() == null && quantityLeft > 0)
                    {
                        quantityLeft--;
                        quantityAdded++;
                    }
                    else if (quantityLeft == 0)
                    {
                        itemsAdded = true;
                        break;
                    }
                }
            }

        }

        if (!itemsAdded)
        {
            return quantityAdded;
        }

        return -1;
    }

    public void RefreshUI()
    {
        //refresh main inventory
        for (int i = 0; i < slots.Length; i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;
                if (items[i].GetItem().isStackable)
                    slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = items[i].GetQuantity() + "";
                else
                    slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
            catch
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
            }
        }

        //refresh equipment slots
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            try
            {
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = equipment[i].GetItem().itemIcon;
                equipmentSlots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";

            }
            catch
            {
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                equipmentSlots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";

            }
        }

        //refresh primary weapon slot
        try
        {
            primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
            primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = primaryWeapon.GetItem().itemIcon;
            primaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
        catch
        {
            primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
            primaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
            primaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }

        //refresh secondary weapon slot
        try
        {
            secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = true;
            secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = secondaryWeapon.GetItem().itemIcon;
            secondaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }
        catch
        {
            secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().sprite = null;
            secondaryWeaponSlotGameObject.transform.GetChild(0).GetComponent<Image>().enabled = false;
            secondaryWeaponSlotGameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";
        }

        //refresh currency
        currencyText.text = currency.ToString();
    }

    public ItemSlot Contains(ItemClass item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item)
                return items[i];
        }

        return null;
    }

    public bool Contains(ItemClass item, int quantity)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].GetItem() == item && items[i].GetQuantity() >= quantity)
                return true;
        }

        return false;
    }

    public int ContainAmount(ItemClass item)
    {
        if (item.isStackable)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == item)
                    return items[i].GetQuantity();
            }
        }
        else
        {
            int totalAmount = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == item)
                    totalAmount += items[i].GetQuantity();
            }

            return totalAmount;
        }

        return 0;
    }

    private bool BeginItemMove()
    {
        originalSlot = GetAllClosestItemSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;

        movingSlot = new ItemSlot(originalSlot);

        if (IsEquipmentItemAndSlot(originalSlot, originalSlot.GetItem()))
        {
            originalSlot.GetItem().GetEquipment().OnUnequip();
        }
        else if (isPrimaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
        {
            if (primaryWeaponGameObject != null)
                Destroy(primaryWeaponGameObject);
            primaryWeaponGameObject = null;
            PlayerManager.instance.PlayerScript.SetPrimaryWeapon(null);
        }
        else if (isSecondaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
        {
            if (secondaryWeaponGameObject != null)
                Destroy(secondaryWeaponGameObject);
            secondaryWeaponGameObject = null;
            PlayerManager.instance.PlayerScript.SetSecondaryWeapon(null);
        }


        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();

        
        return true;
    }

    private bool EndItemMove()
    {
        //drop item if click outside inventory
        if (!IsMouseOverUI())
        {
            GetComponent<DropItem>().DropItems(new ItemSlot(movingSlot.GetItem(), movingSlot.GetQuantity()), PlayerManager.Instance.Player.transform.position);
            movingSlot.Clear();
        }
        else
        {
            ItemSlot oldSlotBeforeMove = originalSlot;

            //gets available slots according to item type
            if (movingSlot.GetItem().GetEquipment() != null)
                originalSlot = GetClosestEquipmentSlot();
            else if (movingSlot.GetItem().GetWeapon() != null)
                originalSlot = GetClosestWeaponSlot();
            else
                originalSlot = GetClosestSlot();

            if (originalSlot == null)
            {
                //not click on a slot
                if (oldSlotBeforeMove.GetItem() != null)
                    AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                else
                {
                    oldSlotBeforeMove.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());

                    //add if statements checkjing for equipment/weapon
                    TryEquipEquipmentWeapon(oldSlotBeforeMove, movingSlot.GetItem());
                }
                    

                movingSlot.Clear();
            }
            else
            {
                if (originalSlot.GetItem() != null)
                {
                    if (originalSlot.GetItem() == movingSlot.GetItem())//same item so stack
                    {
                        if (originalSlot.GetItem().isStackable)
                        {
                            originalSlot.AddQuantity(movingSlot.GetQuantity());
                            movingSlot.Clear();
                        }
                        else
                            return false;
                    }
                    else //swap item with the one in hand
                    {
                        tempSlot = new ItemSlot(originalSlot);
                        originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                        movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());

                        if (IsEquipmentItemAndSlot(originalSlot, movingSlot.GetItem()))
                        {
                            originalSlot.GetItem().GetEquipment().OnEquip();
                            movingSlot.GetItem().GetEquipment().OnUnequip();
                        }
                        else if (isPrimaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
                        {
                            if (primaryWeaponGameObject != null)
                                Destroy(primaryWeaponGameObject);
                            primaryWeaponGameObject = Instantiate(originalSlot.GetItem().GetWeapon().weaponPrefab);
                            PlayerManager.instance.PlayerScript.SetPrimaryWeapon(primaryWeaponGameObject);
                        }
                        else if (isSecondaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
                        {
                            if (secondaryWeaponGameObject != null)
                                Destroy(secondaryWeaponGameObject);
                            secondaryWeaponGameObject = Instantiate(originalSlot.GetItem().GetWeapon().weaponPrefab);
                            PlayerManager.instance.PlayerScript.SetSecondaryWeapon(secondaryWeaponGameObject);
                        }

                        RefreshUI();
                        return true;
                    }
                }
                else
                {
                    //no item in slot
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());

                    /*
                    if (IsEquipmentItemAndSlot(originalSlot, movingSlot.GetItem()))
                    {
                        originalSlot.GetItem().GetEquipment().OnEquip();
                    }
                    else if(isPrimaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
                    {
                        if (primaryWeaponGameObject != null)
                            Destroy(primaryWeaponGameObject);
                        primaryWeaponGameObject = Instantiate(movingSlot.GetItem().GetWeapon().weaponPrefab);
                        PlayerManager.instance.PlayerScript.SetPrimaryWeapon(primaryWeaponGameObject);
                    }
                    else if(isSecondaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
                    {
                        if (secondaryWeaponGameObject != null)
                            Destroy(secondaryWeaponGameObject);
                        secondaryWeaponGameObject = Instantiate(movingSlot.GetItem().GetWeapon().weaponPrefab);
                        PlayerManager.instance.PlayerScript.SetSecondaryWeapon(secondaryWeaponGameObject);
                    }*/
                    TryEquipEquipmentWeapon(originalSlot, movingSlot.GetItem());


                    movingSlot.Clear();
                }
            }
        }


        isMovingItem = false;
        RefreshUI();
        return true;
    }

    private ItemSlot GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
            {
                return items[i];
            }
        }

        return null;
    }

    private ItemSlot GetClosestEquipmentSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= padding) 
            {
                return items[i];
            }
        }

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (Vector2.Distance(equipmentSlots[i].transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
            {
                return equipment[i];
            }
        }

        return null;
    }

    private ItemSlot GetClosestWeaponSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= padding)
            {
                return items[i];
            }
        }

        if (movingSlot.GetItem().GetWeapon().weaponType == WeaponClass.WeaponType.Primary)
        {
            if (Vector2.Distance(primaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
            {
                return primaryWeapon;
            }
        }
        else
        {
            if (Vector2.Distance(secondaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
            {
                return secondaryWeapon;
            }
        }

        return null;
    }

    private ItemSlot GetAllClosestItemSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= padding)
            {
                return items[i];
            }
        }

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (Vector2.Distance(equipmentSlots[i].transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
            {
                return equipment[i];
            }
        }

        if (Vector2.Distance(primaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
        {
            return primaryWeapon;
        }

        if (Vector2.Distance(secondaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding) // half of slot size + half of padding size
        {
            return secondaryWeapon;
        }

        return null;
    }

    //run this to apply effects
    //i might remove this and put everything in the onequip and uneqiup function
    public void GetEquipmentEffects()
    {
        //reset playerdata to base (implemnt later)
        foreach (var item in equipment)
        {
            if(item.GetItem() != null)
                if(item.GetItem().GetEquipment() != null)
                {
                    item.GetItem().GetEquipment().EquipmentEffect();
                }
                    
        }
    }

    private bool IsEquipmentItemAndSlot(ItemSlot itemSlot, ItemClass item)
    {
        for (int i = 0; i < equipment.Length; i++)
        {
            if(itemSlot == equipment[i])
            {
                if(item.GetEquipment() != null)
                    return true;
            }
        }
        return false;
    }

    //add restrictions on primary vs secondary later
    private bool isPrimaryWeaponItemAndSlot(ItemSlot itemSlot, ItemClass item)
    {
        //temp for testing, remove secondary weapon part when seperating
        if(itemSlot == primaryWeapon)
        {
            if(item.GetWeapon() != null)
            {
                return true;
            }
        }
        return false;
    }
    private bool isSecondaryWeaponItemAndSlot(ItemSlot itemSlot, ItemClass item)
    {
        if (itemSlot == secondaryWeapon)
        {
            if (item.GetWeapon() != null)
            {
                return true;
            }
        }
        return false;
    }

    private void TryEquipEquipmentWeapon(ItemSlot itemSlot, ItemClass item)
    {
        if (IsEquipmentItemAndSlot(itemSlot, item))
        {
            originalSlot.GetItem().GetEquipment().OnEquip();
        }
        else if (isPrimaryWeaponItemAndSlot(itemSlot, item))
        {
            if (primaryWeaponGameObject != null)
                Destroy(primaryWeaponGameObject);
            primaryWeaponGameObject = Instantiate(movingSlot.GetItem().GetWeapon().weaponPrefab);
            PlayerManager.instance.PlayerScript.SetPrimaryWeapon(primaryWeaponGameObject);
        }
        else if (isSecondaryWeaponItemAndSlot(itemSlot, movingSlot.GetItem()))
        {
            if (secondaryWeaponGameObject != null)
                Destroy(secondaryWeaponGameObject);
            secondaryWeaponGameObject = Instantiate(movingSlot.GetItem().GetWeapon().weaponPrefab);
            PlayerManager.instance.PlayerScript.SetSecondaryWeapon(secondaryWeaponGameObject);
        }
    }

    public void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void CloseInventory()
    {
        //close inv when holding item
        if (isMovingItem)
        {
            
            if (originalSlot.GetItem() != null && movingSlot.GetItem() != null)
                AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
            else
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                //run checks for equipment/weapons
                TryEquipEquipmentWeapon(originalSlot, movingSlot.GetItem());
            }
                

            movingSlot.Clear();
        }
        isMovingItem = false;


        isInventoryOpen = false;
        inventoryUIRoot?.SetActive(false);
        Cursor.visible = previousCursorState;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
        inventoryUIRoot?.SetActive(true);
        previousCursorState = Cursor.visible;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        RefreshUI();
    }

    private bool IsMouseOverUI()
    {
        var data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        //has to be 1 cus cursor counts
        return results.Count > 1;
    }
}
