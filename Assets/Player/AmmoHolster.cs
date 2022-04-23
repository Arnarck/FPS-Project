using UnityEngine;

public class AmmoHolster : MonoBehaviour
{
    public static AmmoHolster Instance { get; private set; }

    [SerializeField] AmmoSlot[] slots;

    // Shows the content of the class, in the inspector
    [System.Serializable]
    private class AmmoSlot
    {
        int currentAmount;
        public int CurrentAmount { get => currentAmount; set => currentAmount = value; }

        public AmmoType ammoType;
        [Range(0, 9999)] public int maxAmount;
        [Range(0, 9999)] public int initialAmount;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        foreach (AmmoSlot slot in slots)
        {
            slot.initialAmount = Mathf.Clamp(slot.initialAmount, 0, slot.maxAmount);
            slot.CurrentAmount = slot.initialAmount;
        }
    }

    public int GetCurrentAmmo(AmmoType type)
    {
        AmmoSlot slot = FindAmmoSlot(type);

        if (slot != default)
        {
            return slot.CurrentAmount;
        }

        return 0;
    }

    public void IncreaseAmmo(AmmoType type, int amount)
    {
        AmmoSlot slot = FindAmmoSlot(type);
        if (slot != default)
        {
            slot.CurrentAmount = Mathf.Clamp(slot.CurrentAmount + amount, 0, slot.maxAmount);
        }

        AmmoDisplay.Instance.UpdateAmmoInHolster(slot.CurrentAmount);
    }

    public void ReduceAmmo(AmmoType type, int amount)
    {
        AmmoSlot slot = FindAmmoSlot(type);
        if (slot != default)
        {
            slot.CurrentAmount = Mathf.Clamp(slot.CurrentAmount - amount, 0, slot.maxAmount);
        }

        AmmoDisplay.Instance.UpdateAmmoInHolster(slot.CurrentAmount);
    }

    public bool IsAmmoInHolsterEmpty(AmmoType type)
    {
        AmmoSlot slot = FindAmmoSlot(type);

        if (slot == default) { return true; }

        if (slot.CurrentAmount < 1)
        {
            return true;
        }

        return false;
    }

    AmmoSlot FindAmmoSlot(AmmoType type)
    {
        foreach (AmmoSlot slot in slots)
        {
            if (slot.ammoType.Equals(type))
            {
                return slot;
            }
        }

        return default;
    }
}
