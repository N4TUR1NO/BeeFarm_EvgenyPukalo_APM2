using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Swarm : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private float moveSpeed;
    
    [Tooltip("Множитель времени сбора цветка.\nУменьшается с апгрейдом")]
    [SerializeField] private float collectingTimeMultiplier;
    
    [Tooltip("Величина, на которую при апгрейде умножается текущая скорость сбора")]
    [Range(0.01f, 0.99f)]
    [SerializeField] private float speedUpgradeMultiplier;
    [SerializeField] private GameObject beePrefab;
    
    private static int score = 0;
    private Rigidbody _rb;
    private List<Bee> _bees = new List<Bee>();
    private Sequence _giveAwaySequence;
    
    public static Action<Vector3> OnSwarmMoved;
    public static Action<int> OnScoreChanged;
    
    #endregion
    
    #region Properties

    public float CollectingTimeMultiplier => collectingTimeMultiplier;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    #endregion

    #region OnEnable/OnDisable
    
    private void OnEnable()
    {
        InputController.OnDrag += Move;
        InputController.OnRelease += Stop;
    }

    private void OnDisable()
    {
        InputController.OnDrag -= Move;
        InputController.OnRelease -= Stop;
    }

    #endregion

    #region Methods
    
    private void Stop()
    {
        _rb.velocity = Vector3.zero;
    }
    
    private void Move(float xDirection, float zDirection)
    {
        OnSwarmMoved?.Invoke(new Vector3(xDirection, 0, zDirection));
        _rb.velocity = new Vector3(xDirection * moveSpeed, _rb.velocity.y, zDirection * moveSpeed);
    }

    public void CreateBee()
    {
        Vector3 offset = new Vector3(Random.Range(-2f, 2f), 2, Random.Range(-2f, 2f));
        Bee newBee = Instantiate(beePrefab,transform.position + offset, Quaternion.identity).GetComponent<Bee>();
        newBee.Swarm = this;
        newBee.Offset = offset;
        newBee.Activate();
        AddToList(newBee);
    }
    
    public void IncScore()
    {
        OnScoreChanged?.Invoke(++score);
    }
    
    public void DecScore()
    {
        OnScoreChanged?.Invoke(--score);
    }

    public void AddToList(Bee bee)
    {
        _bees.Add(bee);
    }

    public void StartGiveAway(Beehive beehive)
    {
        _giveAwaySequence = DOTween.Sequence();
        _giveAwaySequence
            .AppendInterval(beehive.CollectingTime)
            .SetLoops(-1)
            .OnStepComplete(() =>
            {
                GiveAway(beehive);
            });
    }

    private void GiveAway(Beehive beehive)
    {
        int emptyBees = 0;
        foreach (Bee bee in _bees)
        {
            if (bee.TryDecPollen())
            {
                beehive.IncPollenAmount();
            }
            else
            {
                emptyBees++;
            }
        }
        if (emptyBees == _bees.Count)
        {
            StopGiveAway();
        }
        else
        {
            SoundController.Instance.PlayClip(SfxType.BeehivePickUpSfx);
        }
    }
    
    public void StopGiveAway()
    {
        _giveAwaySequence.Kill();
    }

    public void UpgradeMultiplier()
    {
        collectingTimeMultiplier *= speedUpgradeMultiplier;
    }

    #endregion
}
