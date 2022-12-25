using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    float evade_cooldown_t, evading_t;

    public Transform attack_area;
    public Vector2 evade_direction;
    public float time_to_evade = .5f;
    public float attack_radius = .5f;
    public float evading_time = .1f;
    public float evade_force = 50f;
    public float stagging_time = 1f;
    public float stamina_cost_to_evade = 26f;
    public bool is_evading, can_evade;

    void Awake()
    {
        knife = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Process attack input
            if (Input.GetKeyDown(KeyCode.Mouse0) && can_attack)
            {
                can_attack = false;
                attack_t = time_to_attack;

                Collider[] hits = new Collider[3];
                int colliders_found = Physics.OverlapSphereNonAlloc(attack_area.position, attack_radius, hits);

                if (colliders_found > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i] != null && hits[i].gameObject.CompareTag("Enemy")) hits[i].GetComponent<EnemyAI>().take_damage(damage, stagging_time);

                    }
                }
            }
        }

        if (Input.GetAxisRaw("Vertical") < 0f || Input.GetAxisRaw("Horizontal") != 0f)
        {
            if (Input.GetAxisRaw("Vertical") <= 0 && Input.GetKeyDown(KeyCode.LeftShift) && !is_evading && can_evade && GI.player.stamina >= stamina_cost_to_evade && GI.player.can_run)
            {
                GI.player.change_stamina_amount(-stamina_cost_to_evade);
                evade_direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                is_evading = true;
                can_evade = false;
                evading_t = evading_time;
                evade_cooldown_t = time_to_evade;
            }
        }

        { // Updates evading time
            if (is_evading)
            {
                evading_t -= Time.deltaTime;
                if (evading_t <= 0f) is_evading = false;
            }
        }

        { // Evade cooldown
            if (!is_evading && !can_evade)
            {
                evade_cooldown_t -= Time.deltaTime;
                if (evade_cooldown_t <= 0f) can_evade = true;
            }
        }

        { // Attack Cooldown
            if (attack_t > 0f)
            {
                attack_t -= Time.deltaTime;
                if (attack_t <= 0f) can_attack = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack_area.position, attack_radius);
    }
}
