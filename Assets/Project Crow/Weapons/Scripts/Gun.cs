using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    public bool can_shoot = true, is_reloading, has_ammo;
    bool has_started;
    float time_to_shoot, time_to_reload, time_to_decay_sequence_shots, current_crosshair_range, target_crosshair_range, crosshair_recoil_range;

    AudioSource _audioSource;
    Transform m_transform;

    [HideInInspector] public Vector3 start_position, start_rotation;

    // @Arnarck Create an dictionary and store all vfx into it
    [Header("Visual Effects")]
    public ParticleSystem muzzle_flash_vfx = default;
    public ParticleSystem muzzle_smoke_vfx = default;
    public ParticleSystem blood_splash_vfx = default;

    [Header("Shoot")]
    public LayerMask layer_mask;
    public float headshot_multiplier = 1.5f;
    public float range = 100f;
    public bool is_automatic = true;

    [Header("Damage over distance")]
    public float distance_1 = 5f;
    public float damage_distance_1 = 23;

    [Header("Integrity Debuff")]
    public float time_to_shoot_with_low_integrity;
    public float time_to_shoot_with_no_integrity;

    [Header("Ammo Clip")]
    public AmmoType ammo_type;
    public float reload_time = 1f;
    public int max_ammo_amount = 30;
    public int ammo_amount = 30;

    [Header("Recoil")]
    public int current_shots_in_sequence;
    public int max_shots_in_sequence = 8;
    public float sequence_shots_decay_time = .4f;
    public float snappiness;
    public float return_speed;
    public Vector3 recoil;

    [Header("Crosshair")]
    public float aiming_crosshair_range = 0f;
    public float idle_crosshair_range = .05f; // Percentage of the screen size
    public float walking_crosshair_range;
    public float running_crosshair_range;
    public float crosshair_range_increased_per_shot_in_screen_percentage;
    public float crosshair_snappiness;
    public float crosshair_return_speed;

    [Header("Aiming")]
    public float fov_when_aiming = 40f;
    public float fov_speed = 10f; // How fast the fov will change when turning on / off the aim.
    public float recoil_percentage_when_aiming = .3f;
    public Vector2 aiming_sensitivity;
    public Vector3 aiming_position;
    public Vector3 aiming_rotation;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        m_transform = transform;
        gun = this;
    }

    void OnEnable()
    {
        // Resets the crosshair whenever this gun is equiped.
        current_crosshair_range = idle_crosshair_range;
        target_crosshair_range = current_crosshair_range;
        crosshair_recoil_range = current_crosshair_range;

        if (has_started)
        {
            GI.hud.ammo_display.SetActive(true);
            GI.hud.display_ammo_in_clip(ammo_amount);
            GI.hud.display_crosshair_range(current_crosshair_range);
        }
    }

    void Start()
    {
        has_started = true;
        has_ammo = ammo_amount > 0 ? true : false;

        GI.hud.ammo_display.SetActive(true);
        GI.hud.display_ammo_in_clip(ammo_amount);
        GI.hud.display_crosshair_range(current_crosshair_range);

        start_position = m_transform.localPosition;
        start_rotation = transform.localRotation.eulerAngles;
    }

    void OnDisable()
    {
        // Cancel reload process so the gun won't reenable in reloading state.
        if (is_reloading)
        {
            time_to_reload = 0f;
            is_reloading = false;
        }

        current_shots_in_sequence = 0;
        time_to_decay_sequence_shots = 0;
    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Shoot
            if ((Input.GetKey(KeyCode.Mouse0) && is_automatic) || (Input.GetKeyDown(KeyCode.Mouse0) && !is_automatic)) // Checks if is able to shoot
            {
                if (can_shoot && !is_reloading && has_ammo)
                {
                    // Sequence Shots
                    if (current_shots_in_sequence < max_shots_in_sequence) current_shots_in_sequence += 1;
                    time_to_decay_sequence_shots = sequence_shots_decay_time;

                    // Recoil
                    GI.gun_recoil.add_recoil(get_recoil(), snappiness, return_speed, current_shots_in_sequence);

                    // Integrity
                    integrity -= integrity_reduced_per_attack;
                    integrity = Mathf.Clamp(integrity, 0, max_integrity);

                    // Time to shoot
                    if (integrity <= 0f) time_to_shoot = time_to_shoot_with_no_integrity;
                    else if (integrity < low_integrity) time_to_shoot = time_to_shoot_with_low_integrity;
                    else time_to_shoot = time_to_attack;
                    can_shoot = false;

                    // Reduce ammo
                    ammo_amount--;
                    if (ammo_amount < 1)
                    {
                        ammo_amount = 0;
                        has_ammo = false;
                    }
                    GI.hud.display_ammo_in_clip(ammo_amount);

                    // Plays shot VFX
                    muzzle_flash_vfx.Play();
                    muzzle_smoke_vfx.Play();
                    _audioSource.Play();

                    // Crosshair recoil
                    /*if (!GI.player.is_aiming)*/ add_crosshair_recoil();

                    // Spawn Gun Shot
                    // Generates a random point on screen to "spawn the gun shot".
                    // The random point is generated inside a range in a circular space.
                    float radius = Screen.height * current_crosshair_range;
                    float random_x = Random.Range(-radius, radius);
                    float max_y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(random_x, 2)); // x^2 + y^2 = r^2 -> Equation to project a circle on a cartesian plane
                    float random_y = Random.Range(-max_y, max_y);
                    Vector3 spawn_position = new Vector3(random_x + Screen.width * .5f, random_y + Screen.height * .5f, 0f);

                    // Debug to show the shot position on screen.
                    RectTransform debug_image = Instantiate(GI.hud.debug_image, GI.hud.canvas, false).GetComponent<RectTransform>();
                    float a = spawn_position.x / Screen.width;
                    float b = spawn_position.y / Screen.height;

                    debug_image.anchoredPosition = new Vector3(1920 * a, 1080 * b);

                    // Checks if the gun shot hits anything
                    RaycastHit hit;
                    Ray ray = GI.fp_camera.ScreenPointToRay(spawn_position);
                    bool has_hit_colliders = Physics.Raycast(ray, out hit, range, layer_mask);
                    if (has_hit_colliders)
                    {
                        blood_splash_vfx.transform.position = hit.point;
                        blood_splash_vfx.Play();
                    }

                    // @TODO: Change the VFX played based on what the player hits
                    // Hits the enemy
                    if (has_hit_colliders && hit.collider.gameObject.layer == 10)
                    {
                        // Damage Drop over distance
                        float distance_from_enemy = Vector3.Distance(GI.fp_camera.transform.position, hit.collider.transform.position);
                        float actual_damage = damage;

                        if (distance_from_enemy > distance_1) actual_damage = damage_distance_1;
                        Debug.Log("Distance from enemy: " + distance_from_enemy + "  Damage: " + actual_damage);

                        // @Arnarck OPTIMIZE THIS.
                        // When add Enemy Attack Manager, keep in it references from all enemies in scene.
                        if (hit.collider.CompareTag("EnemyHead"))
                        {
                            hit.collider.transform.parent.GetComponent<EnemyAI>().take_damage(actual_damage * headshot_multiplier);
                            Debug.Log($"SHOT DISTANCE: {distance_from_enemy} | RAW DAMAGE: {actual_damage} | HEADSHOT MULTIPLIER: {headshot_multiplier} | ACTUAL DAMAGE: {actual_damage * headshot_multiplier}");
                        }
                        else
                        {
                            hit.collider.GetComponent<EnemyAI>().take_damage(actual_damage);
                            Debug.Log($"SHOT DISTANCE: {distance_from_enemy} | DAMAGE: {actual_damage}");
                        }

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

        { // Fire rate
            if (!can_shoot)
            {
                time_to_shoot -= Time.deltaTime;
                if (time_to_shoot <= 0f) can_shoot = true;
            }
        }

        { // Reload
            if (Input.GetKeyDown(KeyCode.R) && !is_reloading && ammo_amount < max_ammo_amount && GI.player_inventory.get_total_item_count(GI.player_inventory.get_ammo_type_of(type)) > 0)
            {
                is_reloading = true;
                time_to_reload = reload_time;
            }
        }

        { // Reload time
            if (is_reloading)
            {
                if (time_to_reload <= 0f) // Reload finished
                {
                    is_reloading = false;
                    has_ammo = true;

                    // Remove ammo from holster and reload clip
                    ItemType ammo = GI.player_inventory.get_ammo_type_of(type);
                    int spent_ammo = max_ammo_amount - ammo_amount;
                    int ammo_in_holster = GI.player_inventory.get_total_item_count(ammo);

                    if (ammo_in_holster >= spent_ammo)
                    {
                        ammo_amount = max_ammo_amount;
                        //GI.ammo_holster.store_or_remove(ammo_type, -spent_ammo);
                        GI.player_inventory.remove_item(ammo, spent_ammo);
                    }
                    else
                    {
                        ammo_amount += ammo_in_holster;
                        //GI.ammo_holster.store_or_remove(ammo_type, -ammo_in_holster);
                        GI.player_inventory.remove_item(ammo, ammo_in_holster);
                    }

                    GI.hud.display_ammo_in_clip(ammo_amount);
                    GI.player_inventory.display_total_ammo_of_equiped_weapon();
                }
                else time_to_reload -= Time.deltaTime;
            }
        }

        { // Sequence shots
            if (current_shots_in_sequence > 0)
            {
                time_to_decay_sequence_shots -= Time.deltaTime;
                if (time_to_decay_sequence_shots <= 0f)
                {
                    current_shots_in_sequence = 0;
                    time_to_decay_sequence_shots = 0f;
                }
            }
        }

        //{ // Crosshair recoil
        //    if ((target_crosshair_range != start_crosshair_range || current_crosshair_range != start_crosshair_range) && !GI.player.is_aiming)
        //    {
        //        target_crosshair_range = Mathf.Lerp(target_crosshair_range, start_crosshair_range, Time.deltaTime * crosshair_snappiness);
        //        current_crosshair_range = Mathf.Lerp(current_crosshair_range, target_crosshair_range, Time.deltaTime * crosshair_return_speed);
        //        GI.hud.display_gun_crosshair(current_crosshair_range);
        //    }
        //}

        //@TODO: Adapt the crosshair recoil to the "sequence shots"
        //@TODO: Test if it needs to adjust the animation speed (maybe is too smooth for now).
        //@TODO: Add a condition here to restrict the UI from updating every frame.
        { // Crosshair
            if (GI.player.is_aiming) target_crosshair_range = 0f;
            else if (GI.fp_controller.Running) target_crosshair_range = running_crosshair_range;
            else if (GI.fp_controller.Moving) target_crosshair_range = walking_crosshair_range;
            else target_crosshair_range = idle_crosshair_range;

            crosshair_recoil_range = Mathf.Lerp(crosshair_recoil_range, target_crosshair_range, crosshair_return_speed * Time.deltaTime);
            current_crosshair_range = Mathf.Lerp(current_crosshair_range, crosshair_recoil_range, Time.deltaTime * crosshair_snappiness);
            GI.hud.display_crosshair_range(current_crosshair_range);
        }
    }

    public Vector3 get_recoil()
    {
        if (GI.player.is_aiming) return recoil * GI.player.recoil_multiplier_based_on_terror * recoil_percentage_when_aiming;
        else return recoil * GI.player.recoil_multiplier_based_on_terror;
    }

    // Increases crosshair range
    public void add_crosshair_recoil()
    {
        //target_crosshair_range = idle_crosshair_range + (crosshair_range_increased_per_shot_in_screen_percentage * current_shots_in_sequence);
        crosshair_recoil_range = current_crosshair_range * 5f;
    }

    public void lerp_aim_position(float percentage)
    {
        Vector3 from = GI.player.is_aiming ? start_position : aiming_position;
        Vector3 to = GI.player.is_aiming ? aiming_position : start_position;
        m_transform.localPosition = Vector3.Lerp(from, to, percentage);
    }

    public void lerp_aim_rotation(float percentage)
    {
        Quaternion from = GI.player.is_aiming ? Quaternion.Euler(start_rotation) : Quaternion.Euler(aiming_rotation);
        Quaternion to = GI.player.is_aiming ? Quaternion.Euler(aiming_rotation) : Quaternion.Euler(start_rotation);
        m_transform.localRotation = Quaternion.Lerp(from, to, percentage);
    }

    public bool finished;
    public void lerp_crosshair_range(float percentage)
    {
        float from = GI.player.is_aiming ? idle_crosshair_range : aiming_crosshair_range;
        float to = GI.player.is_aiming ? aiming_crosshair_range : idle_crosshair_range;
        current_crosshair_range = Mathf.Lerp(from, to, percentage);
        GI.hud.display_crosshair_range(current_crosshair_range);
        if (percentage == 1f) finished = true;
        else finished = false;
    }
}
