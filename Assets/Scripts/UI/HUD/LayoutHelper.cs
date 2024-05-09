using UnityEngine;

/// <summary>
/// Adds the ability to toggle on and off children
/// </summary>
public class LayoutHelper : MonoBehaviour
{
    #region Declerations
    /// <summary>Stores an array of children</summary>
    private Transform[] children;
    /// <summary>Is active?</summary>
    private bool active = true;
    #endregion

    #region MonoBehaveiours
    private void Start()
    {
        children = GetComponentsInChildren<Transform>();
     
    }
    #endregion

    #region Toggles
    /// <summary>
    /// Toggle the inventorty
    /// </summary>
    public void Toggle()
    {
        active = !active;
        if(children == null)
        {
            children = GetComponentsInChildren<Transform>();
        }
        foreach (Transform t in children)
        {
            if(t.gameObject == gameObject) { continue; }
            t.gameObject.SetActive(active);
        }
    }
    #endregion
}
