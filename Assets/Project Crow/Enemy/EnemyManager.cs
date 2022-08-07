using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    EnemyAI[] enemies;
    float last_attack_t;
    bool is_enemy_started_attacking;

    public int enemies_attacking;
    public float time_to_attack = .1f;

    void Awake()
    {
        GI.enemy_manager = this;
        enemies = new EnemyAI[transform.childCount];
        for (int i = 0; i < enemies.Length; i++) enemies[i] = transform.GetChild(i).GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GI.pause_game.game_paused) return;

        if (is_enemy_started_attacking)
        {
            if (last_attack_t >= time_to_attack) is_enemy_started_attacking = false;
            else last_attack_t += Time.deltaTime;
        }
    }

    public bool can_attack()
    {
        //if (enemies_attacking >= 3 || is_enemy_started_attacking) return false;
        if (is_enemy_started_attacking) return false;
        //else enemies_attacking++;

        is_enemy_started_attacking = true;
        last_attack_t = 0f;
        return true;
    }
}
