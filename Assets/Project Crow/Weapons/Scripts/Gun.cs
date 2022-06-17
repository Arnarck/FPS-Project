using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    bool _canShoot = true;

    Transform _FPCamera;
    AudioSource _audioSource;

    public bool IsAuto { get => isAuto; }
    public bool CanShoot { get => _canShoot; }

    [SerializeField] ParticleSystem bloodSplash = default;

    [Header("Muzzle")]
    [SerializeField] ParticleSystem muzzleFlash = default;
    [SerializeField] ParticleSystem muzzleSmoke = default;

    [Header("Atributes")]
    [Tooltip("The damage the gun will do to the enemy for each shot it hits.")]
    [SerializeField] int damage = 30;
    [Tooltip("The range of the gun shot.")]
    [SerializeField] float range = 100f;
    [Tooltip("The cooldown time to shoot again, after a shot.")]
    [SerializeField] float timeToShoot = .15f;
    [Tooltip("Will the gun fire continuously while the Fire key is pressed?")]
    [SerializeField] bool isAuto = true;

    void Awake()
    {
        _FPCamera = Camera.main.transform;
        _audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        PlayShotFX();
        ProcessRaycast();
        StartCoroutine(CooldownToShoot());
    }

    void PlayShotFX()
    {
        muzzleFlash.Play();
        muzzleSmoke.Play();
        _audioSource.Play();
    }

    // Checks if the gun hit anything.
    void ProcessRaycast()
    {
        RaycastHit hit;
        bool hasHitColliders;
        hasHitColliders = Physics.Raycast(_FPCamera.position, _FPCamera.forward, out hit, range);

        if (hasHitColliders)
        {
            PlayHitVFX(hit.point, hit.normal);
        }
    }

    void PlayHitVFX(Vector3 position, Vector3 normalDirection)
    {
        // The rotation needed to make the object look at normalDirection.
        Quaternion lookAtNormalDirection = Quaternion.LookRotation(normalDirection);

        bloodSplash.transform.position = position;
        bloodSplash.transform.rotation = lookAtNormalDirection;

        bloodSplash.Play();
    }

    // Disables the gun from firing for a period of time.
    IEnumerator CooldownToShoot()
    {
        _canShoot = false;
        yield return new WaitForSeconds(timeToShoot);
        _canShoot = true;
    }
}
