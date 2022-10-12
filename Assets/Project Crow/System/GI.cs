using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// Game Instances
public static class GI
{
    public static Player player;
    public static Camera fp_camera;
    public static AmmoHolster ammo_holster;
    public static AmmoDisplay ammo_display;
    public static RigidbodyFirstPersonController fp_controller;
    public static PlayerInventory player_inventory;
    public static EnemyManager enemy_manager;
    public static Flashlight player_flashlight;
    public static GunRecoil gun_recoil;
    public static PauseGame pause_game;
    public static HUD hud;

    public static class Config
    {
        public static float health_restored_by_pill = 50f;
        public static float overdose_increased_by_health_pill = 60f;

        public static float stamina_restored_by_pill = 50f;
        public static float overdose_increased_by_stamina_pill = 35f;

        public static float terror_decreased_by_pill = 50f;
        public static float overdose_increased_by_terror_pill = 70f;

        public static float overdosed_multiplier = 1.5f;


        public static float recoil_multiplier_on_terror_level_1 = 1.5f;
        public static float recoil_multiplier_on_terror_level_2 = 3f;
        public static float recoil_multiplier_on_terror_level_3 = 6f;


        public static float gun_integrity_restored_by_repair_kit = 70f;
    }
}
