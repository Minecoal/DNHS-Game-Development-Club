using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private GameObject itemCursor;

    [Header("Inventory Slots")]
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject equipmentSlotHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private ItemSlot[] startingItems;

    public ItemSlot[] items;
    public ItemSlot[] equipment;
    public GameObject[] slots;
    public GameObject[] equipmentSlots;

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

    [SerializeField] private GameObject droppedItemPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Disable when parented to a DonDestoryOnLoad object
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

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
    }

    public void AddItem(ItemClass item, int quantity)
    {
        bool itemsAdded = false;
        int quantityLeft = quantity;

        ItemSlot slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
            slot.AddQuantity(quantity);
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
                    }
                    else if(quantityLeft == 0)
                    {
                        itemsAdded = true;
                        break;
                    }
                }
            }
            
        }

        if (!itemsAdded)
        {
            //drop items(quantityLeft)
            if(quantityLeft != 0)
                DropItem(new ItemSlot(item, quantityLeft));
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
                if (temp.GetQuantity() > quantity)
                    temp.SubQuantity(quantity);
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
        }
        else
        {
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
        originalSlot = GetClosestEquipmentSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;

        movingSlot = new ItemSlot(originalSlot);

        if (IsEquipmentItemAndSlot(originalSlot, originalSlot.GetItem()))
        {
            originalSlot.GetItem().GetEquipment().OnUnequip();
        }

        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();

        
        return true;
    }

    private bool EndItemMove()
    {
        if (!IsMouseOverUI())
        {
            DropItem(new ItemSlot(movingSlot.GetItem(), movingSlot.GetQuantity()));
            movingSlot.Clear();
        }
        else
        {
            ItemSlot oldSlotBeforeMove = originalSlot;

            if (movingSlot.GetItem().GetEquipment() != null)
                originalSlot = GetClosestEquipmentSlot();
            else
                originalSlot = GetClosestSlot();

            if (originalSlot == null)
            {
                //not click on a slot
                if (oldSlotBeforeMove.GetItem() != null)
                    AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                else
                    oldSlotBeforeMove.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());

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
                        RefreshUI();
                        return true;
                    }
                }
                else
                {
                    //no item in slot
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    if (IsEquipmentItemAndSlot(originalSlot, movingSlot.GetItem()))
                    {
                        originalSlot.GetItem().GetEquipment().OnEquip();
                    }
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
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 35) // half of slot size + half of padding size
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
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 35) // half of slot size + half of padding size
            {
                return items[i];
            }
        }

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (Vector2.Distance(equipmentSlots[i].transform.position, Input.mousePosition) <= 35) // half of slot size + half of padding size
            {
                return equipment[i];
            }
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
        if (originalSlot != null)
        {
            if (originalSlot.GetItem() != null)
                AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
            else
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());

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

    private void DropItem(ItemSlot itemToDrop)
    {
        GameObject droppedItem = Instantiate(droppedItemPrefab, PlayerManager.Instance.Player.transform.position, Quaternion.identity);
        droppedItem.GetComponent<DroppedItem>().SetDroppedItem(itemToDrop);
    }
}
