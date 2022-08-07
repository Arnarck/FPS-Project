using System.Collections;
using System.Collections.Generic;
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
    public WeaponType type;
}
