using UnityEngine;

public enum WeaponType
{
    NONE,

    KNIFE,
    PISTOL,
    SHOTGUN,
    ASSAULT_RIFLE,
    SUBMACHINE_GUN,

    COUNT
}

// Base class for Guns, Knives, etc
public class Weapon : MonoBehaviour
{
    [HideInInspector] public float attack_t;
    [HideInInspector] public Gun gun;
    [HideInInspector] public Knife knife;

    public bool can_attack;
    public WeaponType type;

    [Header("Attack")]
    public float damage = 30;
    public float time_to_attack = .15f;

    [Header("Integrity")]
    public float integrity;
    public float max_integrity = 100f;
    public float integrity_reduced_per_attack = 30f;
    public float low_integrity = 30f;

    public void increase_integrity(float value)
    {
        integrity += value;
        integrity = Mathf.Clamp(integrity, 0, max_integrity);
    }
}
