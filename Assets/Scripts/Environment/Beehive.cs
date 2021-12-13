using System;
using UnityEngine;

public class Beehive : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private int countUpCost;
    [SerializeField] private int speedUpCost;
    [SerializeField] private int amountOfPollen;
    [SerializeField] private float collectingTime;

    private TextMesh _pollenText;
    private Swarm _currentSwarm;

    public static Action<Beehive> OnSwarmEnter;
    public static Action OnSwarmExit;
    public static Action OnAmountChanged;
    
    #endregion
    
    #region Properties
    
    public float CollectingTime => collectingTime;
    public int AmountOfPollen => amountOfPollen;
    public int CountUpCost => countUpCost;
    public int SpeedUpCost => speedUpCost;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        _pollenText = GetComponentInChildren<TextMesh>();
        UpdateText();
    }

    #endregion

    #region Methods

    public void IncPollenAmount()
    {
        ChangeAmount(1);
        UpdateText();
    }

    public void UpgradeSpeed()
    {
        ChangeAmount(-speedUpCost);
        _currentSwarm.UpgradeMultiplier();
        UpdateText();
    }

    public void UpgradeCount()
    {
        ChangeAmount(-countUpCost);
        _currentSwarm.CreateBee();
        UpdateText();
    }

    private void ChangeAmount(int delta)
    {
        amountOfPollen += delta;
        OnAmountChanged?.Invoke();
    }
    
    private void UpdateText()
    {
        _pollenText.text = amountOfPollen.ToString();
    }

    #endregion
    
    #region Events
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Swarm>(out var swarm))
        {
            _currentSwarm = swarm;
            OnSwarmEnter?.Invoke(this);
            swarm.StartGiveAway(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Swarm>(out var swarm))
        {
            OnSwarmExit?.Invoke();
            swarm.StopGiveAway();
        }
    }
    
    #endregion
}
