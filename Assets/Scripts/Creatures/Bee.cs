using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Bee : MonoBehaviour
{
    #region Fields
    
    [Range(5f, 10f)][SerializeField] private float moveSpeed;
    [SerializeField] private int capacity;
    [SerializeField] private float rotationTime;
    
    private int _count;
    private bool _isActive = false;
    private bool _isCollecting = false;
    private Slider _healthBar;

    #endregion
    
    #region Properties
    
    public Swarm Swarm { get; set; }

    public Vector3 Offset { get; set; }
    
    public bool IsActive => _isActive;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();
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

        transform.DOKill();
        
        if (moveDirection != Vector3.zero)
            transform.DORotateQuaternion(Quaternion.LookRotation(moveDirection), rotationTime);
        
        transform.DOMove(Swarm.transform.position + Offset, 
            Vector3.Distance(Swarm.transform.position + Offset, transform.position) / moveSpeed)
            .SetEase(Ease.Linear);
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
        transform.DOKill();
        if (Swarm)
        {
            transform.DOMove(Swarm.transform.position + Offset, 
                Vector3.Distance(Swarm.transform.position + Offset, transform.position) / moveSpeed)
                .SetEase(Ease.Linear);
            transform.DOLookAt(Swarm.transform.position + Offset, rotationTime);
        }
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
        _healthBar.value = ((float)_count / capacity);
    }

    public void MoveToFlower(Transform flower)
    {
        transform.DOKill();
        transform.DOMove(flower.transform.position + new Vector3(0, 2, 0), 1f);
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
            Offset = transform.position - swarm.transform.position;
            Activate();
        }
    }
    
    #endregion
}
