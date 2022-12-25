using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    float stagger_t;
    public bool is_alive = true, is_provoked, can_attack = true, is_stagged;
    [HideInInspector] public float time_to_attack = 2f, base_speed;

    [HideInInspector] public Transform target;
    [HideInInspector] public NavMeshAgent nav_mesh_agent;
    [HideInInspector] public Animator animator;

    public float look_speed = 10f;
    public float health = 100f;
    public float damage = 25f;
    public float terror_damage = 15f;
    public float terror_reduced_when_killed = -10f;

    [Header("Detection Settings")]
    public float detection_range = 4f;
    public Transform detection_point;

    [Header("Attack")]
    public Transform attack_point;
    public LayerMask player_layer_mask;
    public float attack_range = .5f;
    public float base_time_to_attack = 2f;
    public float time_to_attack_on_terror_level_1 = 1.7f;
    public float time_to_attack_on_terror_level_2 = 1.4f;
    public float time_to_attack_on_terror_level_3 = 1.1f;

    [Header("Movement")]
    public float speed_on_terror_level_1 = 2.95f;
    public float speed_on_terror_level_2 = 3.05f;
    public float speed_on_terror_level_3 = 3.15f;


    void Awake()
    {
        animator = GetComponent<Animator>();
        nav_mesh_agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        target = GI.player.transform;
        base_speed = nav_mesh_agent.speed;
    }

    void Update()
    {
        if (!is_alive) return;
        if (GI.pause_game.game_paused) return;

        bool is_attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"); // Gets the name of the current playing animation

        { // AI Behaviour
            Vector3 relative_position = target.position - transform.position;
            float distance_from_target = Vector3.Distance(transform.position, target.position);
            float target_distance_from_detection_point = Vector3.Distance(detection_point.position, target.position);

            if (is_provoked && distance_from_target <= nav_mesh_agent.stoppingDistance) // Makes the enemy attack
            {
                // Makes the enemy play Idle animation if it's not attacking
                animator.SetBool("isWalking", false);

                // Makes the enemy look at the player if it's not attacking
                if (!is_attacking)
                {
                    Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
                    Quaternion look_rotation = Quaternion.LookRotation(look_position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);
                }

                // Attack
                if (can_attack && !is_stagged && GI.enemy_manager.can_attack())
                {
                    can_attack = false;
                    animator.SetTrigger("Attack");

                    switch (GI.player.current_terror_level)
                    {
                        case 0: time_to_attack = base_time_to_attack; break;
                        case 1: time_to_attack = time_to_attack_on_terror_level_1; break;
                        case 2: time_to_attack = time_to_attack_on_terror_level_2; break;
                        case 3: time_to_attack = time_to_attack_on_terror_level_3; break;
                        default: Debug.Assert(false); break;
                    }
                }
            }
            else if (is_provoked && !is_attacking) // Makes the enemy chase the player
            {
                // @TODO: Only makes the enemy look at the player if the raycast takes the player
                Vector3 look_position = new Vector3(relative_position.x, 0f, relative_position.z); // Allows the enemy to look only on X and Z axis.
                Quaternion look_rotation = Quaternion.LookRotation(look_position);

                transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, look_speed * Time.deltaTime);
                nav_mesh_agent.SetDestination(target.position);

                switch (GI.player.current_terror_level)
                {
                    case 0: nav_mesh_agent.speed = base_speed; break;
                    case 1: nav_mesh_agent.speed = speed_on_terror_level_1; break;
                    case 2: nav_mesh_agent.speed = speed_on_terror_level_2; break;
                    case 3: nav_mesh_agent.speed = speed_on_terror_level_3; break;
                    default: Debug.Assert(false); break;
                }

                animator.SetBool("isWalking", true);
            }
            else if (target_distance_from_detection_point <= detection_range) // Checks if the target is close enough
            {
                // Checks if there is an wall between player and enemy
                // @TODO: Launches the raycast from the enemy's head instead of the middle of he's body, so the raycast don't collides with small props
                RaycastHit hit;
                bool has_hit_colliders = Physics.Raycast(transform.position, relative_position.normalized, out hit, distance_from_target);
                Debug.DrawRay(transform.position, relative_position, Color.green);

                if (has_hit_colliders && hit.collider.CompareTag("Player")) is_provoked = true;
            }
        }

        { // Attack Cooldown
            if (!can_attack && !is_attacking)
            {
                time_to_attack -= Time.deltaTime;
                if (time_to_attack <= 0f) { can_attack = true; }
            }
        }

        { // Udpates stagger time
            stagger_t -= Time.deltaTime;
            if (stagger_t <= 0f) is_stagged = false;
        }
    }

    public void attack_player()
    {
        if (!is_alive) return;

        // Creates an overlap sphere to be the attack hit area
        Collider[] hit_colliders = new Collider[1];
        int colliders_found = Physics.OverlapSphereNonAlloc(attack_point.position, attack_range, hit_colliders, player_layer_mask);

        if (colliders_found > 0 && hit_colliders[0].CompareTag("Player"))
        {
            GI.player.change_health_amount(-damage);
            GI.player.change_terror_amount(terror_damage);
        }
        else Debug.Log("Player not found");
    }

    public void take_damage(float value, float stagging_time = 0f)
    {
        if (!is_alive) return;

        Debug.Log("Damage received: " + value);

        if (stagging_time > 0f) { stagger_t = stagging_time; is_stagged = true; }
        health -= value;
        if (health < Mathf.Epsilon)
        {
            nav_mesh_agent.speed = 0f;
            GetComponent<CapsuleCollider>().isTrigger = true;
            GI.player.change_terror_amount(terror_reduced_when_killed);
            is_alive = false;
            gameObject.SetActive(false); // Using in Sandbox
        }
        else
        {
            // Tries to find the player. Start chasing him if it finds him
            RaycastHit hit;
            Vector3 relative_position = target.position - transform.position;
            bool has_hit_colliders = Physics.Raycast(transform.position, relative_position.normalized.normalized, out hit, Mathf.Infinity);
            Debug.DrawRay(transform.position, relative_position, Color.green);

            if (has_hit_colliders && hit.collider.CompareTag("Player")) is_provoked = true;
        }
    }

    void OnDrawGizmos()
    {
        // Draws Detection range.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detection_point.position, detection_range);

        // Draws attack hit area
        Gizmos.DrawWireSphere(attack_point.position, attack_range);
    }

    //private void OnGUI()
    //{
    //    GUI.Label(new Rect(25, 25, 200, 20), "Enemy is attacking: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
    //}
}
