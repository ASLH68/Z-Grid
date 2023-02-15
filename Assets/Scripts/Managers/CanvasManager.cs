using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI _currencyUIText;
    [SerializeField] private TextMeshProUGUI _wallCostText;
    [SerializeField] private TextMeshProUGUI _turretCostText;

    private void Start()
    {
        SetCosts();
    }

    private void SetCosts()
    {
        _wallCostText.text = "$" + BuildingManager.main.WallCost;
        _turretCostText.text = "$" + BuildingManager.main.TurretCost;
    }

    /// <summary>
    /// Updates the currency text on screen
    /// </summary>
    public void UpdateCurrencyText()
    {
        _currencyUIText.text = "$" +  PlayerManager.main.Currency;
    }

    /// <summary>
    /// Sets the current building to newBuilding
    /// </summary>
    /// <param name="newBuilding">name of new building type </param>
    public void SetCurrentBuilding(string newBuilding)
    {
        BuildingManager.main.SetCurrentBuilding(newBuilding);
    }

    /// <summary>
    /// 
    /// </summary>
    public void TempButton()
    {
        GameManager.main.EndRound1();
    }
}
