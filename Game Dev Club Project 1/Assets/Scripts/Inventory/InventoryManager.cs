using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject itemCursor;

    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject equipmentSlotHolder;
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;

    [SerializeField] private ItemSlot[] startingItems;

    public ItemSlot[] items;
    public ItemSlot[] equipment;
    public Action<PlayerManager>[] equipEffects;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new ItemSlot[slots.Length];
        equipmentSlots = new GameObject[equipmentSlotHolder.transform.childCount];
        equipment = new ItemSlot[equipmentSlots.Length];
        equipEffects = new Action<PlayerManager>[equipmentSlots.Length];

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

        if (Input.GetMouseButtonDown(1))
        {
            GetEquipmentEffects();
        }

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    public void AddItem(ItemClass item, int quantity)
    {
        ItemSlot slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable)
            slot.AddQuantity(quantity);
        else
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(item, quantity);
                    break;
                }
            }
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

                equipEffects[i] = equipment[i].GetItem().GetEquipment().OnEquip;
            }
            catch
            {
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                equipmentSlots[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "";

                equipEffects[i] = null;
            }
        }
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

    private bool BeginItemMove()
    {
        originalSlot = GetClosestEquipmentSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;

        movingSlot = new ItemSlot(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        RefreshUI();
        return true;
    }

    private bool EndItemMove()
    {
        ItemSlot oldSlotBeforeMove = originalSlot;
        if (movingSlot.GetItem().GetEquipment() != null)
            originalSlot = GetClosestEquipmentSlot();
        else
            originalSlot = GetClosestSlot();

        if (originalSlot == null)
        {
            if(oldSlotBeforeMove.GetItem() != null)
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
                    RefreshUI();
                    return true;
                }
            }
            else
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
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
    private void GetEquipmentEffects()
    {
        foreach (var action in equipEffects)
        {
            if (action != null)
                action(PlayerManager.Instance);
            //
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
}
