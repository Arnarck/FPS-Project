using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    bool can_shoot = true, is_reloading, has_ammo, has_started;
    int current_ammo_amount;
    float last_shot_t, last_reload_t;

    AudioSource _audioSource;
    Transform m_transform;

    [SerializeField] ParticleSystem blood_splash_vfx = default;

    [Header("Muzzle")]
    [SerializeField] ParticleSystem muzzle_flash_vfx = default;
    [SerializeField] ParticleSystem muzzle_smoke_vfx = default;

    [Header("Basic Atributes")]
    public bool is_collected;
    [Tooltip("The damage the gun will do to the enemy for each shot it hits.")]
    [SerializeField] int damage = 30;
    [SerializeField] float headshot_multiplier = 1.5f;
    [Tooltip("The range of the gun shot.")]
    [SerializeField] float range = 100f;
    [Tooltip("The cooldown time to shoot again, after a shot.")]
    [SerializeField] float time_to_shoot = .15f;
    [Tooltip("Will the gun fire continuously while the Fire key is pressed?")]
    [SerializeField] bool is_automatic = true;

    [Header("Ammo Clip")]
    public AmmoType ammo_type;
    [SerializeField] int max_ammo_amount = 30;
    [SerializeField] float reload_time = 1f;
    [SerializeField] int initial_ammo_amount = 30;

    [Header("Recoil")]
    [SerializeField] Vector3 recoil;
    [SerializeField] float snappiness;
    [SerializeField] float return_speed;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        m_transform = transform;
    }

    void OnEnable()
    {
        if (has_started)
        {
            GI.ammo_display.display_ammo_in_clip(current_ammo_amount);
            //GI.ammo_display.display_ammo_in_holster(GI.ammo_holster.current_ammo[(int)ammo_type]);
        }
    }

    void Start()
    {
        has_started = true;
        current_ammo_amount = Mathf.Clamp(initial_ammo_amount, 0, max_ammo_amount); // Clamps the initial ammo amount.
        has_ammo = current_ammo_amount > 0 ? true : false;

        GI.ammo_display.display_ammo_in_clip(current_ammo_amount);
        //GI.ammo_display.display_ammo_in_holster(GI.ammo_holster.current_ammo[(int)ammo_type]);
    }

    void OnDisable()
    {
        // Cancel reload process so the gun won't reenable in reloading state.
        if (is_reloading)
        {
            last_reload_t = 0f;
            is_reloading = false;
        }

        // Cancel shoot cooldown if gun is switched.
        //if (!can_shoot)
        //{
        //    can_shoot = true;
        //}
    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Process Shoot Input
            if ( (Input.GetKey(KeyCode.Mouse0) && is_automatic) || (Input.GetKeyDown(KeyCode.Mouse0) && !is_automatic) ) // Checks if is able to shoot
            {
                if (can_shoot && !is_reloading && has_ammo)
                {
                    GI.gun_recoil.add_recoil(recoil, snappiness, return_speed);

                    // Starts the cooldown to shoot
                    last_shot_t = 0f;
                    can_shoot = false;

                    // Reduce ammo
                    current_ammo_amount--;
                    if (current_ammo_amount < 1)
                    {
                        current_ammo_amount = 0;
                        has_ammo = false;
                    }
                    GI.ammo_display.display_ammo_in_clip(current_ammo_amount);

                    // Plays shot VFX
                    muzzle_flash_vfx.Play();
                    muzzle_smoke_vfx.Play();
                    _audioSource.Play();

                    // Process Raycast. Checks if the gun hits anything
                    RaycastHit hit;
                    bool hasHitColliders;
                    hasHitColliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, range); 

                    // TODO change the VFX played based on what the player hits
                    // Hits the enemy
                    if (hasHitColliders && hit.collider.gameObject.layer == 10)
                    {
                        // Process Damage Drop over distance
                        float distance_from_enemy = Util.distance(GI.fp_camera.transform.position, hit.collider.transform.position);
                        switch (ammo_type)
                        {
                            case AmmoType.Pistol:
                                {
                                    if (distance_from_enemy <= 5f) damage = 12;
                                    else if (distance_from_enemy <= 15f) damage = 10;
                                    else damage = 8;
                                }
                                break;

                            case AmmoType.Shotgun:
                                {
                                    if (distance_from_enemy <= 5f) damage = 100;
                                    else if (distance_from_enemy <= 10f) damage = 75;
                                    else damage = 30;
                                }
                                break;

                            case AmmoType.AssaultRifle:
                                {
                                    if (distance_from_enemy <= 15f) damage = 30;
                                    else if (distance_from_enemy <= 25f) damage = 23;
                                    else damage = 18;
                                }
                                break;

                            case AmmoType.SubmachineGun:
                                {
                                    if (distance_from_enemy <= 7f) damage = 18;
                                    else if (distance_from_enemy <= 15f) damage = 15;
                                    else damage = 12;
                                }
                                break;

                            default:
                                Debug.LogError("Ammo Type not found!");
                                break;
                        }
                        Debug.Log("Distance from enemy: " + distance_from_enemy + "  Damage: " + damage);
                        
                        // TODO OPTIMIZE THIS.
                        // When add Enemy Attack Manager, keep in it references from all enemies in scene.
                        if (hit.collider.CompareTag("EnemyHead"))
                        {
                            hit.collider.transform.parent.GetComponent<EnemyAI>().take_damage(damage * headshot_multiplier);
                            Debug.Log("DAMAGE: " + damage + " | HEADSHOT MULTIPLIER: " + headshot_multiplier);
                        }
                        else hit.collider.GetComponent<EnemyAI>().take_damage(damage);

                        // Plays hit VFX
                        // The rotation needed to make the object look at normalDirection.
                        Quaternion lookAtNormalDirection = Quaternion.LookRotation(hit.normal);

                        blood_splash_vfx.transform.position = hit.point;
                        blood_splash_vfx.transform.rotation = lookAtNormalDirection;

                        blood_splash_vfx.Play();
                    }
                }
            }
        }

        { // Process fire rate
            if (!can_shoot)
            {
                if (last_shot_t >= time_to_shoot) can_shoot = true;
                else last_shot_t += Time.deltaTime;
            }
        }

        { // Process Reload Input
            if (Input.GetKeyDown(KeyCode.R) && !is_reloading && current_ammo_amount < max_ammo_amount && GI.player_inventory.get_total_item_count(GI.player_inventory.get_ammo_type_of(type)) > 0)
            {
                is_reloading = true;
                last_reload_t = 0f;
            }
        }

        { // Process time to reload
            if (is_reloading)
            {
                if (last_reload_t >= reload_time)
                {
                    is_reloading = false;
                    has_ammo = true;

                    // Remove ammo from holster and reload clip
                    ItemType ammo = GI.player_inventory.get_ammo_type_of(type);
                    int spent_ammo = max_ammo_amount - current_ammo_amount;
                    int ammo_in_holster = GI.player_inventory.get_total_item_count(ammo);

                    if (ammo_in_holster >= spent_ammo)
                    {
                        current_ammo_amount = max_ammo_amount;
                        //GI.ammo_holster.store_or_remove(ammo_type, -spent_ammo);
                        GI.player_inventory.remove_item(ammo, spent_ammo);
                    }
                    else
                    {
                        current_ammo_amount += ammo_in_holster;
                        //GI.ammo_holster.store_or_remove(ammo_type, -ammo_in_holster);
                        GI.player_inventory.remove_item(ammo, ammo_in_holster);
                    }

                    GI.ammo_display.display_ammo_in_clip(current_ammo_amount);
                    GI.player_inventory.display_total_ammo_of_equiped_weapon();
                }
                else last_reload_t += Time.deltaTime;
            }
        }
    }
}
