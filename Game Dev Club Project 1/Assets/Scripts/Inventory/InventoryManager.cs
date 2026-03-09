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
    [SerializeField] private ItemSlot[] startingItems;
    [SerializeField] private GameObject primaryWeaponSlotGameObject;
    [SerializeField] private GameObject secondaryWeaponSlotGameObject;

    [SerializeField] private GameObject inventoryUIRoot; //canvas
    [SerializeField] private KeyCode toggleKey = KeyCode.E;
    [SerializeField] private GameObject droppedItemPrefab;
    
    [Header("Currency")]
    [SerializeField] private CurrencyInfo[] coinData;

    private InventoryStorageService storageService;
    private EquipmentService equipmentService;
    private DropService dropService;
    private InventoryUIPresenter uiPresenter;

    private ItemSlot movingSlot;
    private ItemSlot tempSlot;
    private ItemSlot originalSlot;
    private bool isMovingItem;

    private bool previousCursorState;
    private bool isInventoryOpen = false;
    private int padding = 35;// half of slot size + half of padding size

    private PlayerManager playerManager;
    private DropItem dropItemComponent;
    
    public ItemSlot[] items => storageService.GetItems();
    public ItemSlot[] equipment => equipmentService.GetEquipmentSlots();
    public int currency => dropService.GetCurrency();
    public GameObject[] slots { get; private set; }
    public GameObject[] equipmentSlots { get; private set; }
    public GameObject DroppedItemPrefab => droppedItemPrefab;

    override protected void Awake()
    {
        base.Awake();
        playerManager = PlayerManager.Instance;
        dropItemComponent = GetComponent<DropItem>();

        SetupSlotReferences();
        InitializeServices();

        uiPresenter = GetComponent<InventoryUIPresenter>();
        if (uiPresenter != null)
        {
            uiPresenter.Initialize(storageService, equipmentService, dropService, 
                slots, equipmentSlots, primaryWeaponSlotGameObject, secondaryWeaponSlotGameObject);
        }

        SetupStartingItems();
    }

    private void SetupSlotReferences()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        equipmentSlots = new GameObject[equipmentSlotHolder.transform.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i] = equipmentSlotHolder.transform.GetChild(i).gameObject;
        }
    }

    private void InitializeServices()
    {
        storageService = new InventoryStorageService(slots.Length, dropItemComponent, playerManager.Player.transform);
        equipmentService = new EquipmentService(equipmentSlots.Length, playerManager);
        dropService = new DropService(dropItemComponent);
    }

    private void SetupStartingItems()
    {
        // initialize starting items
        for (int i = 0; i < startingItems.Length; i++)
        {
            if (startingItems[i].GetItem() != null)
            {
                storageService.SetItemAt(i, startingItems[i]);
            }
        }

        // equip any starting equipment
        ItemSlot[] equipmentSlots = equipmentService.GetEquipmentSlots();
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].GetItem() != null && equipmentSlots[i].GetItem().GetEquipment() != null)
            {
                equipmentService.EquipEquipment(equipmentSlots[i], playerManager.PlayerScript.playerContext);
            }
        }
    }

    void Update()
    {
        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (isMovingItem && movingSlot.GetItem() != null)
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;

        if (Input.GetMouseButtonDown(0))
        {
            if (isMovingItem)
                EndItemMove();
            else
                BeginItemMove();
        }

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }

        if (Input.GetMouseButtonDown(2))
        {
            dropItemComponent.DropCurrency(10, playerManager.Player.transform.position);
        }
    }

    public void AddItem(ItemClass item, int quantity)
    {
        storageService.AddItem(item, quantity);
    }

    public void RemoveItem(ItemClass item)
    {
        storageService.RemoveItem(item);
    }

    public void RemoveItem(ItemClass item, int quantity)
    {
        storageService.RemoveItem(item, quantity);
    }

    public int CanAddItem(ItemClass item, int quantity)
    {
        return storageService.CanAddItem(item, quantity);
    }

    public ItemSlot Contains(ItemClass item)
    {
        return storageService.Contains(item);
    }

    public bool Contains(ItemClass item, int quantity)
    {
        return storageService.Contains(item, quantity);
    }

    public int ContainAmount(ItemClass item)
    {
        return storageService.ContainAmount(item);
    }

    public void AddCurrency(int amount)
    {
        dropService.AddCurrency(amount);
    }

    public void RemoveCurrency(int amount)
    {
        dropService.RemoveCurrency(amount);
    }

    public void RefreshUI()
    {
        if (uiPresenter != null)
        {
            uiPresenter.RefreshUI();
        }
    }

    //item movement
    private bool BeginItemMove()
    {
        originalSlot = GetAllClosestItemSlot();
        if (originalSlot == null || originalSlot.GetItem() == null)
            return false;

        movingSlot = new ItemSlot(originalSlot);

        // Handle unequipping
        if (IsEquipmentItemAndSlot(originalSlot, originalSlot.GetItem()))
        {
            equipmentService.UnequipEquipment(originalSlot, playerManager.PlayerScript.playerContext);
        }
        else if (isPrimaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
        {
            equipmentService.UnequipPrimaryWeapon();
        }
        else if (isSecondaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
        {
            equipmentService.UnequipSecondaryWeapon();
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
            dropService.DropItems(movingSlot, playerManager.Player.transform.position);
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
                    TryEquipEquipmentWeapon(oldSlotBeforeMove, movingSlot.GetItem());
                    oldSlotBeforeMove.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
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
                            equipmentService.EquipEquipment(originalSlot, playerManager.PlayerScript.playerContext);
                            equipmentService.UnequipEquipment(movingSlot, playerManager.PlayerScript.playerContext);
                        }
                        else if (isPrimaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
                        {
                            equipmentService.EquipPrimaryWeapon(originalSlot);
                        }
                        else if (isSecondaryWeaponItemAndSlot(originalSlot, movingSlot.GetItem()))
                        {
                            equipmentService.EquipSecondaryWeapon(originalSlot);
                        }

                        RefreshUI();
                        return true;
                    }
                }
                else
                {
                    //no item in slot
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    TryEquipEquipmentWeapon(originalSlot, movingSlot.GetItem());
                    movingSlot.Clear();
                }
            }
        }

        isMovingItem = false;
        RefreshUI();
        return true;
    }

    //helpers
    private ItemSlot GetClosestSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= padding)
            {
                return storageService.GetItemAt(i);
            }
        }
        return null;
    }

    private ItemSlot GetClosestEquipmentSlot()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (Vector2.Distance(equipmentSlots[i].transform.position, Input.mousePosition) <= padding)
            {
                return equipmentService.GetEquipmentSlots()[i];
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
                return storageService.GetItemAt(i);
            }
        }

        if (movingSlot.GetItem().GetWeapon().weaponType == WeaponClass.WeaponType.Primary)
        {
            if (Vector2.Distance(primaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding)
            {
                return equipmentService.GetPrimaryWeapon();
            }
        }
        else
        {
            if (Vector2.Distance(secondaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding)
            {
                return equipmentService.GetSecondaryWeapon();
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
                return storageService.GetItemAt(i);
            }
        }

        ItemSlot[] equipSlots = equipmentService.GetEquipmentSlots();
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (Vector2.Distance(equipmentSlots[i].transform.position, Input.mousePosition) <= padding)
            {
                return equipSlots[i];
            }
        }

        if (Vector2.Distance(primaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding)
        {
            return equipmentService.GetPrimaryWeapon();
        }

        if (Vector2.Distance(secondaryWeaponSlotGameObject.transform.position, Input.mousePosition) <= padding)
        {
            return equipmentService.GetSecondaryWeapon();
        }

        return null;
    }

    private bool IsEquipmentItemAndSlot(ItemSlot itemSlot, ItemClass item)
    {
        ItemSlot[] equipSlots = equipmentService.GetEquipmentSlots();
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (itemSlot == equipSlots[i])
            {
                if (item.GetEquipment() != null)
                    return true;
            }
        }
        return false;
    }

    private bool isPrimaryWeaponItemAndSlot(ItemSlot itemSlot, ItemClass item)
    {
        if (itemSlot == equipmentService.GetPrimaryWeapon())
        {
            if (item.GetWeapon() != null)
            {
                return true;
            }
        }
        return false;
    }

    private bool isSecondaryWeaponItemAndSlot(ItemSlot itemSlot, ItemClass item)
    {
        if (itemSlot == equipmentService.GetSecondaryWeapon())
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
            equipmentService.EquipEquipment(itemSlot, playerManager.PlayerScript.playerContext);
        }
        else if (isPrimaryWeaponItemAndSlot(itemSlot, item))
        {
            equipmentService.EquipPrimaryWeapon(itemSlot);
        }
        else if (isSecondaryWeaponItemAndSlot(itemSlot, item))
        {
            equipmentService.EquipSecondaryWeapon(itemSlot);
        }
    }

    public void ToggleInventory()
    {
        if (isInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
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
