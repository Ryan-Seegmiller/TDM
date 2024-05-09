using TMPro;
using UnityEngine;

/// <summary>
/// Progression values display
/// </summary>
public class ProgressionValues : MonoBehaviour
{
    #region Declerations
    /// <summary>Score text reference</summary>
    public TextMeshProUGUI scoreText;
    /// <summary>Level text reference</summary>
    public TextMeshProUGUI levelText;
    /// <summary>Mulitiplier text reference</summary>
    public TextMeshProUGUI multiplierText;
    /// <summary>Gold text reference</summary>
    public TextMeshProUGUI goldText;
    /// <summary>Score digit count</summary>
    public int scoreDigitCount = 4;
    #endregion

    #region MonoBeavhiours
    private void Start()
    {
       SetGoldText(0);
       SetLevelText(1);
       SetMultplierText(GameManager.instance.scoreMultiplier);
       SetScoreText(0);
    }
    #endregion

    #region Set Text
    /// <summary> Sets the score text</summary>
    /// <param name="score"></param>
    public void SetScoreText(int score)
    {
        string scoreString = score.ToString();
        scoreString = scoreString.PadLeft(scoreDigitCount, '0');
        
        scoreText.text = scoreString;
    }
    /// <summary>
    /// Set level text
    /// </summary>
    /// <param name="level"></param>
    public void SetLevelText(int level)
    {
        levelText.text = level.ToString();
    }
    /// <summary>
    /// Set Multiplier text
    /// </summary>
    /// <param name="multiplier"></param>
    public void SetMultplierText(float multiplier)
    {
        multiplierText.text = $"X {multiplier}";
    }
    /// <summary>
    /// Set Gold Text
    /// </summary>
    /// <param name="gold"></param>
    public void SetGoldText(int gold)
    {
        goldText.text = gold.ToString();
    }
    #endregion
}
