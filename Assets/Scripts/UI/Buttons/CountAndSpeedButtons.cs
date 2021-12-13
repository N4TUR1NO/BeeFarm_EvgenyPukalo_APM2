using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountAndSpeedButtons : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Button speedButton;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button countButton;

    private Beehive _currentBeehive;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        speedButton.onClick.AddListener(UpgradeSpeed);
        countButton.onClick.AddListener(UpgradeCount);
    }
    
    #endregion

    #region OnEnable/OnDisable

    private void OnEnable()
    {
        Beehive.OnSwarmEnter += InitButtons;
        Beehive.OnSwarmExit += DeactivateButtons;
        Beehive.OnAmountChanged += UpdateAccessibility;
    }

    private void OnDisable()
    {
        Beehive.OnSwarmEnter -= InitButtons;
        Beehive.OnSwarmExit -= DeactivateButtons;
        Beehive.OnAmountChanged -= UpdateAccessibility;
    }

    #endregion

    #region Methods
    
    private void UpgradeSpeed()
    {
        _currentBeehive.UpgradeSpeed();
    }

    private void UpgradeCount()
    {
        _currentBeehive.UpgradeCount();
    }
    
    private void InitButtons(Beehive beehive)
    {
        _currentBeehive = beehive;
        
        UpdatePrices(beehive.CountUpCost, beehive.SpeedUpCost);
        UpdateAccessibility();
        
        ActivateButtons();
    }

    private void UpdateAccessibility()
    {
        countButton.interactable = _currentBeehive.AmountOfPollen >= _currentBeehive.CountUpCost;
        speedButton.interactable = _currentBeehive.AmountOfPollen >= _currentBeehive.SpeedUpCost;
    }
    
    private void UpdatePrices(float countUpCost, float speedUpCost)
    {
        speedText.text = "Speed\n" + speedUpCost;
        countText.text = "Count\n" + countUpCost;
    }
    
    private void ActivateButtons()
    {
        speedButton.gameObject.SetActive(true);
        countButton.gameObject.SetActive(true);
    }
    
    private void DeactivateButtons()
    {
        speedButton.gameObject.SetActive(false);
        countButton.gameObject.SetActive(false);
    }
    
    #endregion
}
