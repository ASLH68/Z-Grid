using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("Static")]
    public static CanvasManager main;

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI _currencyUIText;
    [SerializeField] private TextMeshProUGUI _livesUIText;
    [SerializeField] private TextMeshProUGUI _buildingUIText;
    [SerializeField] private TextMeshProUGUI _wallCostText;
    [SerializeField] private TextMeshProUGUI _turretCostText;
    [SerializeField] private TextMeshProUGUI _roundUIText;

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
        SetCosts();
    }

    /// <summary>
    /// Sets the costs of the different buildings
    /// </summary>
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
    /// Updates the lives amount text
    /// </summary>
    public void UpdateLivesText()
    {
        _livesUIText.text = GameManager.main.Health + " Lives";
    }

    public void UpdateBuildingText()
    {
        _buildingUIText.text = BuildingManager.main._currentBuilding.ToString();
    }

    public void UpdateRoundText()
    {
        _roundUIText.text = "Round " + GameManager.main.CurrentRound;
    }

    /// <summary>
    /// Sets the current building to newBuilding
    /// </summary>
    /// <param name="newBuilding">name of new building type </param>
    public void SetCurrentBuilding(string newBuilding)
    {
        BuildingManager.main.SetCurrentBuilding(newBuilding);
    }
}
