using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [SerializeField] private EconomyBalanceSO balance;

    public int SoftCurrency { get; private set; }
    public int HardCurrency { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SoftCurrency = balance != null ? balance.startingSoftCurrency : 100;
        HardCurrency = balance != null ? balance.startingHardCurrency : 10;
    }

    public void AddSoft(int amount)
    {
        SoftCurrency += amount;
        GameEvents.OnCurrencyChanged?.Invoke(SoftCurrency, HardCurrency);
    }

    public bool SpendSoft(int amount)
    {
        if (SoftCurrency < amount) return false;
        SoftCurrency -= amount;
        GameEvents.OnCurrencyChanged?.Invoke(SoftCurrency, HardCurrency);
        return true;
    }

    public void AddHard(int amount)
    {
        HardCurrency += amount;
        GameEvents.OnCurrencyChanged?.Invoke(SoftCurrency, HardCurrency);
    }
}
