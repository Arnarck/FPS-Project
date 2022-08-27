public enum ItemType
{
    NONE,

    // Weapons
    KNIFE,
    PISTOL,
    SHOTGUN,
    ASSAULT_RIFLE,
    SUBMACHINE_GUN,

    // Ammo Type
    PISTOL_AMMO,
    SHOTGUN_AMMO,
    ASSAULT_RIFLE_AMMO,
    SUBMACHINE_GUN_AMMO,

    // Consumables
    HEALTH_PILL,
    STAMINA_PILL,
    ANTI_TERROR_PILL,
    ANTI_OVERDOSE_PILL,

    // Repair
    GUN_REPAIR_KIT,

    // Important

    COUNT
}

// @Arnarck move this to GI
public static class InventoryData
{
    public static string[] name = { };
    public static string[] description = { };
    public static int[] max_capacity = { 0, 1, 1, 1, 1, 1, 30, 16, 60, 90, 1, 1, 1, 1, 5, 0 };
}
