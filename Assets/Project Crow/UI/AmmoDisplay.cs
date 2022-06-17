using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    public static AmmoDisplay Instance { get; private set; }

    [SerializeField] TextMeshProUGUI ammoInClip;
    [SerializeField] TextMeshProUGUI ammoInHolster;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateAmmoInClip(int ammoAmount)
    {
        if (ammoInClip == null) return;
        ammoInClip.text = ammoAmount.ToString();
    }

    public void UpdateAmmoInHolster(int ammoAmount)
    {
        if (ammoInHolster == null) return;
        ammoInHolster.text = ammoAmount.ToString();
    }
}
