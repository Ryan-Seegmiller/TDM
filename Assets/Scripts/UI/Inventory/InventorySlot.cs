using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inventory Slot componenet
/// </summary>
public class InventorySlot : MonoBehaviour
{
    #region Decleartions
    /// <summary>Image to be slotted</summary>
    public Image slottedImage;
    /// <summary>Border selected</summary>
    public Image selectedBorderImage;
    /// <summary>Count text that displays</summary>
    public TextMeshProUGUI countText;
    #endregion

    #region Unity Behaveiours
    private void Start()
    {
        Deselect();
        SetCount(0);
    }
    #endregion

    #region Image
    /// <summary>
    /// Sets the image uisng the sprite and tint
    /// </summary>
    /// <param name="spriteToSetImageTo"></param>
    /// <param name="tint"></param>
    public void SetImage(Sprite spriteToSetImageTo, Color tint)
    {
        slottedImage.sprite = spriteToSetImageTo;
        slottedImage.color = tint;
    }
    /// <summary>
    /// Sets the image using an image
    /// </summary>
    /// <param name="imageToSetTo"></param>
    public void SetImage(Image imageToSetTo)
    {
        slottedImage.sprite = imageToSetTo.sprite;
        slottedImage.color = imageToSetTo.color;
    }
    /// <summary>
    /// Returns the image
    /// </summary>
    /// <returns></returns>
    public Image GetImage()
    {
        return slottedImage;
    }
    /// <summary>
    /// Remove the image 
    /// </summary>
    public void RemoveImage()
    {
        slottedImage.sprite = null;
        slottedImage.color = Color.clear;
    }
    /// <summary>
    /// Returns if there is in fact a image in the slot for the image
    /// </summary>
    /// <returns></returns>
    public bool HasImage()
    {
        return slottedImage.sprite != null;
    }
    #endregion

    #region Select
    /// <summary>
    /// Turns on the slected border
    /// </summary>
    public void Select()
    {
        selectedBorderImage.color = new Color(selectedBorderImage.color.r, selectedBorderImage.color.g, selectedBorderImage.color.b, 1);
    }
    /// <summary>
    /// Turns off the selected border
    /// </summary>
    public void Deselect()
    {
        selectedBorderImage.color = new Color(selectedBorderImage.color.r, selectedBorderImage.color.g, selectedBorderImage.color.b, 0);
    }
    #endregion

    #region Count
    /// <summary>
    /// Sets the count text to the amount listed
    /// </summary>
    /// <param name="amount"></param>
    public void SetCount(int amount)
    {
        if(amount == 0)
        {
            countText.text = "";
            return;
        }
        countText.text = "x" + amount;
    }
    #endregion

}
