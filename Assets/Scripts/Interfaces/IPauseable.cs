/// <summary>
/// An interface that links that object to the pausables
/// </summary>
public interface IPauseable
{
    /// <summary>
    /// A way to determine wether or not the object has been paused
    /// </summary>
    bool IsPaused { get; set; }

    /// <summary>
    /// Behaveiour that trigger on pause
    /// </summary>
    void Pause();
    /// <summary>
    /// Behaviour that triggers on play
    /// </summary>
    void Play();

    /// <summary>
    /// Subscribe to the ipauseables in the game manager
    /// </summary>
    public void SubscribeToGameManager();
    ///<example>
    ///\nif (!GameManager.GameManager.instance.pausables.Contains(this))
    ///{\n
    ///\n    GameManager.GameManager.instance.pausables.Add(this);
    ///}\n
    /// </example>
   

}
