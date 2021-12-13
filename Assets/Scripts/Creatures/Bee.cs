using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Bee : MonoBehaviour
{
    #region Fields

    [Range(5f, 20f)] [SerializeField] private float moveSpeed;
    [SerializeField] private int capacity;
    [SerializeField] private float rotationTime;

    private int _count;
    private bool _isActive = false;
    private bool _isCollecting = false;
    private Slider _healthBar;
    public bool isSwarmReached = false;
    private float _stoppingDistance;
    private Vector3 _offset;
    private Vector3 _startScale;
    private Collider _col;
    private Rigidbody _rb;

    #endregion

    #region Properties

    public Swarm Swarm { get; set; }

    public bool IsActive => _isActive;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();
        _col = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _offset = Vector3.up * 2;
        _startScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (!_isActive)
            return;

        _rb.velocity = Vector3.zero;
        _stoppingDistance = _col.bounds.extents.z;
         if (Vector3.Distance(transform.position, Swarm.transform.position + _offset) < _stoppingDistance)
         {
             isSwarmReached = true;
         }
         if (!isSwarmReached)
         {
             transform.position = Vector3.MoveTowards(transform.position, 
                                                  Swarm.transform.position + _offset, 
                                                moveSpeed * 0.02f);
         }
         else
         {
             _rb.isKinematic = true;
         }
    }

    #endregion

    #region OnEnable/OnDisable

    private void OnEnable()
    {
        Swarm.OnSwarmMoved += MoveToSwarm;
    }

    private void OnDisable()
    {
        Swarm.OnSwarmMoved -= MoveToSwarm;
    }

    #endregion

    #region Methods

    private void MoveToSwarm(Vector3 moveDirection)
    {
        if (!_isActive) return;

        isSwarmReached = false;
        _rb.isKinematic = false;
        
        if (moveDirection != Vector3.zero)
            transform.DORotateQuaternion(Quaternion.LookRotation(moveDirection), rotationTime);
    }

    public void Activate()
    {
        _isActive = true;
    }

    public bool IncAmount()
    {
        if (_count == capacity)
        {
            StopCollecting();
            return false;
        }

        _count++;
        UpdateHealthBarValue();
        Swarm.IncScore();
        return true;
    }

    public bool CanCollect()
    {
        if (_count == capacity || _isCollecting)
            return false;

        _isCollecting = true;
        return true;
    }

    public void StopCollecting()
    {
        _isCollecting = false;
        isSwarmReached = false;
        DOTween.Kill(this);
        transform.DOScale(_startScale, 0.1f);
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.white;
        transform.DOLookAt(Swarm.transform.position + _offset, rotationTime);
    }

    public bool TryDecPollen()
    {
        if (_count == 0)
            return false;

        _count--;
        Swarm.DecScore();
        UpdateHealthBarValue();
        return true;
    }

    private void UpdateHealthBarValue()
    {
        _healthBar.value = ((float) _count / capacity);
    }

    public void CollectingAnimation(Transform flower)
    {
        transform.DOMove(flower.transform.position + _offset, 0.5f);
        transform.DOScale(_startScale * 0.7f, 0.5f);
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
    }

    public void SetKinematic(bool isKinematic)
    {
        _rb.isKinematic = isKinematic;
    }
    
    #endregion

    #region Events

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Swarm>(out var swarm))
        {
            if (_isActive) return;

            Swarm = swarm;
            Swarm.AddToList(this);
            Activate();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isSwarmReached)
            if (other.gameObject.TryGetComponent<Bee>(out var bee))
            {
                if (bee.isSwarmReached)
                    isSwarmReached = true;
            }
    }

    #endregion
}