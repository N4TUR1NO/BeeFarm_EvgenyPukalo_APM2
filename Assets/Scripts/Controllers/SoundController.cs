using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxType
{
    CollectSfx,
    DeathSfx,
    BeehivePickUpSfx
}

[Serializable] public class SfxInfo
{
    public SfxType type;
    public AudioClip clip;
}

public class SoundController : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private List<SfxInfo> list;

    public static SoundController Instance;
    private Transform _point;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _point = Camera.main.transform;
    }

    #endregion

    #region Methods

    public void PlayClip(SfxType type)
    {
        SfxInfo selected = GetVfxByType(type);
        AudioSource.PlayClipAtPoint(selected.clip, _point.position);
    }

    private SfxInfo GetVfxByType(SfxType type)
    {
        foreach (var sfxInfo in list)
        {
            if (sfxInfo.type == type)
                return sfxInfo;
        }

        return null;
    }
    
    #endregion
}
