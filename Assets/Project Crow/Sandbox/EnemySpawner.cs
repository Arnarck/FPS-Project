using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    float spawn_t;

    public float time_to_spawn = .1f;
    public int max_enemies_in_pool = 100;
    public EnemyAI enemy_prefab;

    public EnemyAI[] enemies_spawned;

    // Start is called before the first frame update
    void Start()
    {
        spawn_t = time_to_spawn;
        enemies_spawned = new EnemyAI[max_enemies_in_pool];

        for (int i = 0; i < max_enemies_in_pool; i++)
        {
            EnemyAI enemy = Instantiate(enemy_prefab);
            enemy.transform.SetParent(this.transform);
            enemy.is_provoked = true;
            enemy.gameObject.SetActive(false);
            enemies_spawned[i] = enemy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        spawn_t -= Time.deltaTime;
        if (spawn_t <= 0f)
        {
            spawn_t = time_to_spawn;
            spawn_enemy();
        }
    }

    public void spawn_enemy()
    {
        foreach (EnemyAI enemy in enemies_spawned)
        {
            if (!enemy.gameObject.activeSelf)
            {
                enemy.transform.localPosition = Vector3.zero;
                enemy.transform.localRotation = Quaternion.identity;
                enemy.gameObject.SetActive(true);

                enemy.health = 100f;
                enemy.nav_mesh_agent.speed = 3.5f;
                enemy.loot_area.SetActive(false);
                enemy.GetComponent<CapsuleCollider>().isTrigger = false;
                enemy.is_alive = true;
                return;
            }
        }
    }
}
