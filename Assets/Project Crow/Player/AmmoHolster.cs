using UnityEngine;
public enum AmmoType
{
    Pistol,
    Shotgun,
    AssaultRifle,
    SubmachineGun,

    COUNT
}

public class AmmoHolster : MonoBehaviour
{
    // 0- Pistol; 1- Shotgun; 2- Assalt Rifle; 3- Submachine Gun;
    int[] expanded_ammo = { 15, 8, 30, 45 };
    int[] max_ammo = { 15, 8, 30, 45 };
    public int[] current_ammo = { 0, 0, 0, 0 };

    void Awake()
    {
        GI.ammo_holster = this;
    }

    public bool store_or_remove(AmmoType type, int amount)
    {
        int slot = (int)type;

        if (amount > 0 && current_ammo[slot] >= max_ammo[slot]) return false;
        if (amount < 0 && current_ammo[slot] < 0) return false;
        
        current_ammo[slot] = Mathf.Clamp(current_ammo[slot] + amount, 0, max_ammo[slot]);

        //if (GI.gun_switcher.get_current_gun().ammo_type == type) GI.ammo_display.display_ammo_in_holster(current_ammo[slot]);

        return true;
    }

    public void expand_max_ammo()
    {
        for (int i = 0; i < max_ammo.Length; i++)
        {
            max_ammo[i] += expanded_ammo[i];
            Debug.Log((AmmoType)i + " expanded to " + max_ammo[i]);
        }
    }
}
