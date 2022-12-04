using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    Vector3 current_rotation, target_rotation;
    float m_snappiness, m_return_speed;

    public Transform weapon_holster;

    void Awake()
    {
        GI.gun_recoil = this;
    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        target_rotation = Vector3.Lerp(target_rotation, Vector3.zero, m_return_speed * Time.deltaTime); // Always try to reset
        current_rotation = Vector3.Slerp(current_rotation, target_rotation, m_snappiness * Time.deltaTime); // Always tries to go to target_rotation
        transform.localRotation = Quaternion.Euler(current_rotation);
        weapon_holster.localRotation = Quaternion.Euler(current_rotation); // Remove this line
        //weapon_holster.localPosition = current_rotation;
    }

    public void add_recoil(Vector3 recoil, float snappiness, float return_speed, int sequence_shots)
    {
        m_snappiness = snappiness;
        m_return_speed = return_speed;

        target_rotation = new Vector3(recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z)) * sequence_shots; // Keep this line (just remove the if statement)
    }
}
