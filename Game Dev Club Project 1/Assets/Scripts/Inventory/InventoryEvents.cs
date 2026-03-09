using System;
using UnityEngine;

public static class InventoryEvents
{
    public static event Action<ItemClass, int> ItemAdded;
    public static event Action<ItemClass, int> ItemRemoved;
    public static event Action InventoryRefreshed;

    public static event Action<EquipmentClass, PlayerContext> EquipmentEquipped;
    public static event Action<EquipmentClass, PlayerContext> EquipmentUnequipped;

    public static event Action<WeaponClass, GameObject> PrimaryWeaponEquipped;
    public static event Action<WeaponClass, GameObject> SecondaryWeaponEquipped;
    public static event Action PrimaryWeaponUnequipped;
    public static event Action SecondaryWeaponUnequipped;

    public static event Action<int> CurrencyAdded;
    public static event Action<int> CurrencyRemoved;

    public static event Action<ItemSlot, Vector3> ItemDropped;
    public static event Action<int, Vector3> CurrencyDropped;

    public static void PublishItemAdded(ItemClass item, int quantity)
    {
        ItemAdded?.Invoke(item, quantity);
    }

    public static void PublishItemRemoved(ItemClass item, int quantity)
    {
        ItemRemoved?.Invoke(item, quantity);
    }

    public static void PublishInventoryRefreshed()
    {
        InventoryRefreshed?.Invoke();
    }

    public static void PublishEquipmentEquipped(EquipmentClass equipment, PlayerContext context)
    {
        EquipmentEquipped?.Invoke(equipment, context);
    }

    public static void PublishEquipmentUnequipped(EquipmentClass equipment, PlayerContext context)
    {
        EquipmentUnequipped?.Invoke(equipment, context);
    }

    public static void PublishPrimaryWeaponEquipped(WeaponClass weapon, GameObject instance)
    {
        PrimaryWeaponEquipped?.Invoke(weapon, instance);
    }

    public static void PublishSecondaryWeaponEquipped(WeaponClass weapon, GameObject instance)
    {
        SecondaryWeaponEquipped?.Invoke(weapon, instance);
    }

    public static void PublishPrimaryWeaponUnequipped()
    {
        PrimaryWeaponUnequipped?.Invoke();
    }

    public static void PublishSecondaryWeaponUnequipped()
    {
        SecondaryWeaponUnequipped?.Invoke();
    }

    public static void PublishCurrencyAdded(int amount)
    {
        CurrencyAdded?.Invoke(amount);
    }

    public static void PublishCurrencyRemoved(int amount)
    {
        CurrencyRemoved?.Invoke(amount);
    }

    public static void PublishItemDropped(ItemSlot item, Vector3 position)
    {
        ItemDropped?.Invoke(item, position);
    }

    public static void PublishCurrencyDropped(int amount, Vector3 position)
    {
        CurrencyDropped?.Invoke(amount, position);
    }
}
