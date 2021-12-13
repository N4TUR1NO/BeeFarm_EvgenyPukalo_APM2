using System;
using DG.Tweening;
using UnityEngine;

public class Flower : MonoBehaviour
{
    #region Fields

    [SerializeField] private int health;
    
    [Header("Collecting")] 
    [SerializeField] private float collectionTime;
    [SerializeField] private float shakeDuration;
    [SerializeField] [Range(0.01f, 1f)] private float shakeStrength;

    [Header("Disappearance")] 
    [SerializeField] private float delay;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;
    
    private bool _isCollecting = false;
    private Bee _currentBee = null;
    private Sequence _collectingSeq;
    
    #endregion

    #region Methods

    private bool TryCollect(Bee bee)
    {
        if (_isCollecting)
            return false;

        StartCollecting(bee);
        return true;
    }

    private void StartCollecting(Bee bee)
    {
        _isCollecting = true;
        _currentBee = bee;
        _collectingSeq = DOTween.Sequence();
        _collectingSeq.AppendInterval(collectionTime * bee.Swarm.CollectingTimeMultiplier).OnStepComplete(() =>
        {
            if (_currentBee.IncAmount())
            {
                SoundController.Instance.PlayClip(SfxType.CollectSfx);
                DecHealth();
                CollectingVFX();
                transform.DOShakeScale(shakeDuration, shakeStrength);
            }
            else
            {
                StopCollecting(_currentBee);
            }
        }).SetLoops(-1).Pause();
        _collectingSeq.Restart();
    }

    private void StopCollecting(Bee bee)
    {
        if (bee != _currentBee)
            return;

        _currentBee = null;
        _collectingSeq.Pause();
        _isCollecting = false;
    }

    private void DecHealth()
    {
        if (--health == 0)
        {
            Die();
        }
    }

    private void CollectingVFX()
    {
        GameObject vfx = PoolController.Instance.GetPoolObject(PoolType.PollenVFX);
        vfx.transform.position = transform.position + new Vector3(0, 3, 0);
        vfx.SetActive(true);
        
        Sequence waitSec = DOTween.Sequence();
        waitSec.AppendInterval(1f).OnComplete(() => PoolController.Instance.ReturnObjectToPool(vfx, PoolType.PollenVFX));
    }

    private void DeathVFX()
    {
        GameObject vfx = PoolController.Instance.GetPoolObject(PoolType.DeathVFX);
        vfx.transform.position = transform.position;
        vfx.SetActive(true);
        
        Sequence waitSec = DOTween.Sequence();
        waitSec.AppendInterval(2f).OnComplete(() => PoolController.Instance.ReturnObjectToPool(vfx, PoolType.DeathVFX));
    }

    private Tween DeathAnimation()
    {
        return transform.DOScale(Vector3.zero, duration)
            .SetDelay(delay)
            .SetEase(curve);
    }
    
    private void Die()
    {
        _collectingSeq.Kill();
        transform.DOKill();
        _currentBee.StopCollecting();

        SoundController.Instance.PlayClip(SfxType.DeathSfx);
        DeathVFX();
        DeathAnimation().OnComplete(() => Destroy(gameObject));
    }

    #endregion

    #region Events

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Bee>(out var bee))
        {
            if (!bee.CanCollect() || !bee.IsActive)
                return;

            if (TryCollect(bee))
            {
                bee.MoveToFlower(transform);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Bee>(out var bee))
        {
            if (!bee.CanCollect() || !bee.IsActive)
                return;

            if (TryCollect(bee))
            {
                bee.MoveToFlower(transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Bee>(out var bee))
        {
            StopCollecting(bee);
            bee.StopCollecting();
        }
    }

    #endregion
}
