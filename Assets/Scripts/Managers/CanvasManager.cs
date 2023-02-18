using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Static")]
    public static CanvasManager main;

    [SerializeField] private TextMeshProUGUI _roundUIText;

    [Header("Currency")]
    [SerializeField] private TextMeshProUGUI _currencyUIText;
    [SerializeField] private TextMeshProUGUI _livesUIText;

    [Header("Building UI")]
    [SerializeField] private TextMeshProUGUI _wallCostText;
    [SerializeField] private TextMeshProUGUI _turretCostText;    
    [SerializeField] private TextMeshProUGUI _machineTurretCostText;
    [SerializeField] private TextMeshProUGUI _sniperTurretCostText;

    [HideInInspector] public GameObject CurrentButton;
    private Color _normalColor;
    private Color _selectedColor;

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
        _normalColor = Color.white;
        _selectedColor = Color.red;
    }

    /// <summary>
    /// Sets the costs of the different buildings
    /// </summary>
    public void SetCosts()
    {
        _wallCostText.text = "$" + BuildingManager.main.WallCost;
        _turretCostText.text = "$" + BuildingManager.main.BasicTurretCost;       
        _machineTurretCostText.text = "$" + BuildingManager.main.MachineTurretCost;
        _sniperTurretCostText.text = "$" + BuildingManager.main.SniperTurretCost;
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

    /// <summary>
    /// Updates the current round text
    /// </summary>
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
        CurrentButton.GetComponent<Image>().color = _normalColor;
        BuildingManager.main.SetCurrentBuilding(newBuilding);
        CurrentButton.GetComponent<Image>().color = _selectedColor;
    }
}
