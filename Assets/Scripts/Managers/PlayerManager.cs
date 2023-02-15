using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Static")]
    public static PlayerManager main;

    [Header("Dependencies")]
    [SerializeField] private CanvasManager _canvasManager;

    [Header("Currency")]
    [SerializeField] private int _startingAmount;
    private int _currency;    

    public int Currency => _currency;

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SetInitialCurrency();
    }

    /// <summary>
    /// Modifies amt of currency player has
    /// </summary>
    /// <param name="amount"> amount of currency being added/removed </param>
    public void ModifyCurrency(int amount)
    {
        if (amount > 0 || HasEnoughCurrency(amount))
        {
            _currency += amount;
            _canvasManager.UpdateCurrencyText();
        }
        else
        {
            print("Not enough currency");
        }
        
    }

    /// <summary>
    /// Returns whether the play can afford to make the purchase
    /// </summary>
    /// <param name="amount"> amount of currency being compared </param>
    /// <returns></returns>
    public bool HasEnoughCurrency(int amount)
    {
        return _currency >= amount;
    }

    /// <summary>
    /// Sets the players starting amount of currency
    /// </summary>
    public void SetInitialCurrency()
    {
        ModifyCurrency(_startingAmount);
    }
}
