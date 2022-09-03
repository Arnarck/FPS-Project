using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{
    float evading_t;
    float last_evade_t;

    public Transform attack_area;
    public Vector2 evade_direction;
    public float time_to_evade = .5f;
    public float attack_radius = .5f;
    public float evade_time = 1f;
    public float evade_force = 50f;
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
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Collider[] hits = new Collider[3];
                int colliders_found = Physics.OverlapSphereNonAlloc(attack_area.position, attack_radius, hits);

                if (colliders_found > 0)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i] != null && hits[i].gameObject.CompareTag("Enemy")) hits[i].GetComponent<EnemyAI>().take_damage(damage);

                    }
                }
            }
        }

        if (Input.GetAxisRaw("Vertical") < 0f || Input.GetAxisRaw("Horizontal") != 0f)
        {
            if (Input.GetAxisRaw("Vertical") <= 0 && Input.GetKeyDown(KeyCode.LeftShift) && !is_evading && can_evade)
            {
                evade_direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                is_evading = true;
                can_evade = false;
                evading_t = 0f;
                last_evade_t = 0f;
            }
        }

        { // Evading time countdown
            if (is_evading)
            {
                if (evading_t >= evade_time) is_evading = false;
                else evading_t += Time.deltaTime;
            }
        }

        { // Evade cooldown countdown
            if (!is_evading && !can_evade)
            {
                if (last_evade_t >= time_to_evade) can_evade = true;
                else last_evade_t += Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack_area.position, attack_radius);
    }
}
