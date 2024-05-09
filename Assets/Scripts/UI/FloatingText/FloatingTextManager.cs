using System;
using System.Collections.Generic;
using UnityEngine;

namespace FloatingText
{
    /// <summary>
    /// A component that stores floating text
    /// </summary>
    public class FloatingTextManager : MonoBehaviour
    {
        #region Declerations
        /// <summary>Singleton refernce</summary>
        public static FloatingTextManager instance;
        /// <summary>the rect transform of the floating text manager</summary>
        private RectTransform rectTR;

        /// <summary>Max amount of floating text that can be in the scene</summary>
        public int maxFloatingText = 20;
        /// <summary>list of disabled text in the pool</summary>
        private List<FloatingText> disabledText = new List<FloatingText>();
        /// <summary>list of active text in the pool</summary>
        private List<FloatingText> activeText = new List<FloatingText>();

        /// <summary>Base prefab for the floating text</summary>
        public FloatingText baseTextPrefab;

        /// <summary>Text values for damage</summary>
        public FloatingTextValues damagedText;
        /// <summary>Text values for Door Text</summary>
        public FloatingTextValues doorText;
        /// <summary>Text values for Shop Text</summary>
        public FloatingTextValues shopText;
        /// <summary>Text values for the score</summary>
        public FloatingTextValues scoreText;
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            #region Singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion

            rectTR = GetComponent<RectTransform>();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the text active and stationary
        /// </summary>
        /// <param name="data"></param>
        /// <param name="position"></param>
        public FloatingText SetStationaryFloatingText(FloatingTextValues data, Vector3 position)
        {
            FloatingText floatingText = GrabFloatingText();
            floatingText.SetPosition(position, data.offset, data.duration, data.speed, data.direction, RemoveFloatingText);
            floatingText.SetText(data.text, data.textColor);
            return floatingText;
        }
        /// <summary>
        /// Sets the text to attach to a transform
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tr"></param>
        public void SetAttachedFLoatingText(FloatingTextValues data, Transform tr)
        {
            FloatingText floatingText = GrabFloatingText();
            floatingText.SetPositionAttached(tr, data.offset, data.duration, RemoveFloatingText);
            floatingText.SetText(data.text, data.textColor);

        }
        /// <summary>
        /// Sets the text to move tworads a location on the canvas and calls a method when it has reached its destination
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tr"></param>
        /// <param name="startPotion"></param>
        /// <param name="onRemoval"></param>
        public void SetStaticMovementFloatingText(FloatingTextValues data, RectTransform tr, Vector3 startPotion, Action onRemoval)
        {
            if (!data.hasStaticMovement) { Debug.LogWarning($"Text {data.text} was not set to hasStaticMovement"); return; }
            FloatingText floatingText = GrabFloatingText();
            floatingText.SetPositionStaticMovement(tr,data.speed ,data.duration, data.momentaryHoverFloat ,startPotion, data.size, (floatingText) => { RemoveFloatingText(floatingText); onRemoval.Invoke(); });
            floatingText.SetText(data.text, data.textColor);
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Gets a floating text from the active floating text if there is one
        /// </summary>
        /// <returns></returns>
        private FloatingText GrabFloatingText()
        {
            FloatingText textToSet = null;
            if (baseTextPrefab == null)
            {
                Debug.LogWarning("No Base text prefab set in the inspector");
                return textToSet;
            }

            if (disabledText.Count == 0)
            {
                textToSet = Instantiate(baseTextPrefab, rectTR);
                textToSet.Setup();
            }
            else
            {
                textToSet = disabledText[0];
                disabledText.Remove(textToSet);
            }
            activeText.Add(textToSet);
            textToSet.gameObject.SetActive(true);
            return textToSet;
        }
        /// <summary>
        /// Adds the floating text back to the list
        /// </summary>
        /// <param name="floatingText"></param>
        public void RemoveFloatingText(FloatingText floatingText)
        {
            if(activeText.Count <= maxFloatingText) 
            { 
                disabledText.Add(floatingText);
                activeText.Remove(floatingText);

                floatingText.gameObject.SetActive(false);
            }
            else
            {
                Destroy(floatingText.gameObject);
            }
        }
        #endregion
    }

    #region FLoating Text Values
    [Serializable]
    public struct FloatingTextValues
    {
        [Header("Text Values")]
        /// <summary>
        /// The displayed text
        /// </summary>
        public string text;
        /// <summary>
        /// Color of the text
        /// </summary>
        public Color textColor;
        /// <summary>
        /// Size of the text
        /// </summary>
        public Vector3 size;

        [Header("Timer")]
        /// <summary>
        /// Duration of the text on screen. Leave -1 for no time limit
        /// </summary>
        [Tooltip("For no time limit set -1")] public float duration;

        [Header("Movement values")]
        /// <summary>
        /// Speed at which the text will float
        /// </summary>
        public float speed;
        /// <summary>
        /// Offset at which the text will be placed initially
        /// </summary>
        public Vector3 offset;
        /// <summary>
        /// The direction at which the text will move
        /// </summary>
        [Tooltip("For no movement set all values to 0")] public Vector3 direction;

        [Header("Static movement")]
        ///<summary>
        /// Detmerines wther or not it will move to a particular location
        ///</summary>
        public bool hasStaticMovement;
        /// <summary>
        /// Time that it will stay in that spot
        /// </summary>
        public float momentaryHoverFloat;

    }
    #endregion

}
