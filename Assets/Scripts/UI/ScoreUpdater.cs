using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdater : MonoBehaviour
{
    private Text _scoreText;

    #region LifeCycle
    
    private void Awake()
    {
        _scoreText = GetComponent<Text>();
    }

    #endregion
    
    #region OnEnable/OnDisable

    private void OnEnable()
    {
        Swarm.OnScoreChanged += UpdateText;
    }

    private void OnDisable()
    {
        Swarm.OnScoreChanged -= UpdateText;
    }

    #endregion

    #region Methods
    
    private void UpdateText(int score)
    {
        _scoreText.text = score.ToString();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        transform
            .DOScale(Vector3.one * 1.5f, 0.5f)
            .OnComplete(() => transform.localScale = Vector3.one);
    }
    
    #endregion
}
