using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Game Instances
public static class GI
{
    public static Player player;
    public static Camera fp_camera;
    public static AmmoHolster ammo_holster;
    public static AmmoDisplay ammo_display;
    public static GunSwitcher gun_switcher;
    public static RigidbodyFirstPersonController fp_controller;
    public static PlayerInventory player_inventory;
    public static EnemyManager enemy_manager;
    public static Flashlight player_flashlight;
    public static GunRecoil gun_recoil;

    public static int[] max_ammo = { 75, 16, 60, 90 }; // Pistol, Shotgun, AR, SMG
}
