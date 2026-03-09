using UnityEngine;

public class EquipmentService
{
    private ItemSlot[] equipmentSlots;
    private ItemSlot primaryWeapon = new ItemSlot();
    private ItemSlot secondaryWeapon = new ItemSlot();

    private GameObject primaryWeaponGameObject;
    private GameObject secondaryWeaponGameObject;

    private PlayerManager playerManager;

    public EquipmentService(int equipmentSlotCount, PlayerManager playerManager)
    {
        equipmentSlots = new ItemSlot[equipmentSlotCount];
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i] = new ItemSlot();
        }
        this.playerManager = playerManager;
    }

    public void EquipEquipment(ItemSlot equipmentSlot, PlayerContext playerContext, ItemSlot oldEquipmentSlot = null)
    {
        if (equipmentSlot.GetItem() == null || equipmentSlot.GetItem().GetEquipment() == null)
            return;

        EquipmentClass equipment = equipmentSlot.GetItem().GetEquipment();
        equipment.OnEquip(playerContext);

        InventoryEvents.PublishEquipmentEquipped(equipment, playerContext);
    }

    public void UnequipEquipment(ItemSlot equipmentSlot, PlayerContext playerContext)
    {
        if (equipmentSlot.GetItem() == null || equipmentSlot.GetItem().GetEquipment() == null)
            return;

        EquipmentClass equipment = equipmentSlot.GetItem().GetEquipment();
        equipment.OnUnequip(playerContext);

        InventoryEvents.PublishEquipmentUnequipped(equipment, playerContext);
    }

    public void EquipPrimaryWeapon(ItemSlot weaponSlot)
    {
        if (weaponSlot.GetItem() == null || weaponSlot.GetItem().GetWeapon() == null)
            return;

        WeaponClass weapon = weaponSlot.GetItem().GetWeapon();

        // destroy old weapon
        if (primaryWeaponGameObject != null)
            Object.Destroy(primaryWeaponGameObject);

        //create new one
        primaryWeaponGameObject = Object.Instantiate(weapon.weaponPrefab);
        primaryWeapon = new ItemSlot(weaponSlot);

        if (playerManager != null && playerManager.PlayerScript != null)
        {
            playerManager.PlayerScript.SetPrimaryWeapon(primaryWeaponGameObject);
        }

        InventoryEvents.PublishPrimaryWeaponEquipped(weapon, primaryWeaponGameObject);
    }

    public void UnequipPrimaryWeapon()
    {
        if (primaryWeaponGameObject != null)
            Object.Destroy(primaryWeaponGameObject);

        primaryWeaponGameObject = null;
        primaryWeapon.Clear();

        if (playerManager != null && playerManager.PlayerScript != null)
        {
            playerManager.PlayerScript.SetPrimaryWeapon(null);
        }

        InventoryEvents.PublishPrimaryWeaponUnequipped();
    }

    public void EquipSecondaryWeapon(ItemSlot weaponSlot)
    {
        if (weaponSlot.GetItem() == null || weaponSlot.GetItem().GetWeapon() == null)
            return;

        WeaponClass weapon = weaponSlot.GetItem().GetWeapon();

        if (secondaryWeaponGameObject != null)
            Object.Destroy(secondaryWeaponGameObject);

        secondaryWeaponGameObject = Object.Instantiate(weapon.weaponPrefab);
        secondaryWeapon = new ItemSlot(weaponSlot);

        if (playerManager != null && playerManager.PlayerScript != null)
        {
            playerManager.PlayerScript.SetSecondaryWeapon(secondaryWeaponGameObject);
        }

        InventoryEvents.PublishSecondaryWeaponEquipped(weapon, secondaryWeaponGameObject);
    }

    public void UnequipSecondaryWeapon()
    {
        if (secondaryWeaponGameObject != null)
            Object.Destroy(secondaryWeaponGameObject);

        secondaryWeaponGameObject = null;
        secondaryWeapon.Clear();

        if (playerManager != null && playerManager.PlayerScript != null)
        {
            playerManager.PlayerScript.SetSecondaryWeapon(null);
        }

        InventoryEvents.PublishSecondaryWeaponUnequipped();
    }

    public ItemSlot GetPrimaryWeapon()
    {
        return primaryWeapon;
    }

    public ItemSlot GetSecondaryWeapon()
    {
        return secondaryWeapon;
    }

    public ItemSlot[] GetEquipmentSlots()
    {
        return equipmentSlots;
    }

    public void TryEquipItem(ItemSlot itemSlot, ItemSlot targetSlot, PlayerContext playerContext)
    {
        if (itemSlot.GetItem() == null)
            return;

        if (itemSlot.GetItem().GetEquipment() != null && IsEquipmentSlot(targetSlot))
        {
            EquipEquipment(itemSlot, playerContext);
            return;
        }

        if (itemSlot.GetItem().GetWeapon() != null && IsPrimaryWeaponSlot(targetSlot))
        {
            EquipPrimaryWeapon(itemSlot);
            return;
        }

        if (itemSlot.GetItem().GetWeapon() != null && IsSecondaryWeaponSlot(targetSlot))
        {
            EquipSecondaryWeapon(itemSlot);
            return;
        }
    }

    private bool IsEquipmentSlot(ItemSlot slot)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (slot == equipmentSlots[i])
                return true;
        }
        return false;
    }

    private bool IsPrimaryWeaponSlot(ItemSlot slot)
    {
        return slot == primaryWeapon;
    }

    private bool IsSecondaryWeaponSlot(ItemSlot slot)
    {
        return slot == secondaryWeapon;
    }
}
