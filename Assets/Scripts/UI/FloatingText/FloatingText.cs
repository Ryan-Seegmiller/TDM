using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FloatingText
{
    /// <summary>
    /// A component that creates floating text above at a specifc location
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FloatingText : MonoBehaviour
    {
        #region Declerations

        #region Componenets
        /// <summary>text mesh pro refernce in the inspector</summary>
        public TextMeshProUGUI textToFloat;
        /// <summary>RectTransorm of the floating text</summary>
        private RectTransform rectTransform;
        /// <summary>Canvas group on the floating text</summary>
        private CanvasGroup cvGroup;
        #endregion

        #region Movement
        /// <summary>Time the object will be on screen</summary>
        private float durationOnScreen = 1;
        /// <summary>Speed at which the text will move</summary>
        private float movementSpeed;
        /// <summary>Time at which the text was activated</summary>
        private float startTime;
        /// <summary>Direction at which the text will move</summary>
        private Vector3 movementDirection = Vector3.zero;

        /// <summary>Wether or not the text has no movement</summary>
        private bool lockedInPlace = false;
        /// <summary>The transorm that the text will be following</summary>
        private Transform attachedTransform = null;
        /// <summary>The offset to the attached object</summary>
        private Vector3 attachedOffset = Vector3.zero;
        /// <summary>The position at which the text goes</summary>
        private Vector3 attachedPosition = Vector3.zero;

        #endregion

        #region Static Movement
        /// <summary>Wether or not the object has static movement</summary>
        private bool hasStaticMovement = false;
        /// <summary>RectTransform that the text will move to</summary>
        private RectTransform transformToMoveTo;
        /// <summary>Start position the text will start at</summary>
        private Vector3 startPosition;
        /// <summary>Time that the text will float before it will move</summary>
        private float momentaryPause;
        #endregion

        /// <summary>Action that will happen on removal</summary>
        Action<FloatingText> onRemoval;
        #endregion

        #region MonoBehaveiours
        private void Update()
        {
            AttachToWorldPostion();
            RemovalTimer();
            MoveToStaticLocation();
            MoveText();
           // CheckOffScreen();
        }
        #endregion

        #region  Internal methods
        /// <summary>
        /// Detects wether or not the text is off screen
        /// </summary>
        private void CheckOffScreen()
        {   //TODO: Fix
            bool isFullyVisible = rectTransform.IsFullyVisibleFrom(GameManager.instance.mainCamera);
            if (!isFullyVisible)
            {
                Remove();
            }
        }
        /// <summary>
        /// Sets up the component refernces
        /// </summary>
        public void Setup()
        {
            rectTransform = GetComponent<RectTransform>();
            cvGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Attaches the text to a world positon
        /// </summary>
        private void AttachToWorldPostion()
        {
            if (hasStaticMovement) { return; }
            if (lockedInPlace) 
            {
                rectTransform.position = GameManager.instance.mainCamera.WorldToScreenPoint(attachedTransform.position + attachedOffset);
            }
            else
            {
                rectTransform.position = GameManager.instance.mainCamera.WorldToScreenPoint(attachedPosition + attachedOffset);
            }
        }
        /// <summary>
        /// Moves the text to the specified attached position
        /// </summary>
        private void MoveToStaticLocation()
        {
            if (!hasStaticMovement) { return; }

            float timeRemaining = MathFunc.TimeLeft(durationOnScreen, startTime);
            if (MathFunc.Timeout(momentaryPause, startTime))
            {
                float timePercentage = timeRemaining / (durationOnScreen - momentaryPause);
                rectTransform.position = Vector3.Lerp(transformToMoveTo.position, startPosition, timePercentage);
            }

        }
        /// <summary>
        /// Timer for removal
        /// </summary>
        private void RemovalTimer()
        {
            if (durationOnScreen == -1) { return; }
            if (MathFunc.Timeout(durationOnScreen, startTime))
            {
                Remove();
            }
        }
        /// <summary>
        /// Removes the text
        /// </summary>
        private void Remove()
        {
            onRemoval?.Invoke(this);//Plays the on removal method after the alotted time
            durationOnScreen = 0;
            startTime = 0;
            hasStaticMovement = false;
        }

        /// <summary>
        /// moved the text in the direction in which was set 
        /// </summary>
        private void MoveText()
        {
            if (hasStaticMovement) { return; }
            if (lockedInPlace || movementDirection == Vector3.zero) { return; }
            float timeLeft = MathFunc.TimeLeft(durationOnScreen, startTime);
            rectTransform.position += movementDirection * movementSpeed;
            cvGroup.alpha = timeLeft / durationOnScreen;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the postion in a stationary manner
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="offset"></param>
        /// <param name="timeOnScreen"></param>
        /// <param name="movementSpeed"></param>
        /// <param name="moveDirection"></param>
        /// <param name="removal"></param>
        public void SetPosition(Vector3 worldPosition, Vector3 offset, float timeOnScreen, float movementSpeed, Vector3 moveDirection, Action<FloatingText> removal)
        {
            rectTransform.position = GameManager.instance.mainCamera.WorldToScreenPoint(worldPosition + offset);
            startTime = Time.realtimeSinceStartup;
            onRemoval = removal;
            this.durationOnScreen = timeOnScreen;
            this.movementSpeed = movementSpeed;
            this.movementDirection = moveDirection;
            attachedPosition = worldPosition;
            lockedInPlace = false;
        }

        /// <summary>
        ///Sets the text up to be attached to the transform with an offset 
        /// </summary>
        /// <param name="attachedTransform"></param>
        /// <param name="offset"></param>
        /// <param name="timeOnScreen"></param>
        /// <param name="removal"></param>
        public void SetPositionAttached(Transform attachedTransform, Vector3 offset, float timeOnScreen, Action<FloatingText> removal)
        {
            startTime = Time.realtimeSinceStartup;

            this.attachedTransform = attachedTransform;
            attachedOffset = offset;
            lockedInPlace = true;
            this.durationOnScreen = timeOnScreen;
            onRemoval = removal;
            AttachToWorldPostion();
        }

        public void SetPositionStaticMovement(RectTransform attachedTransform, float movementSpeed , float timeOnScreen,float momentaryPause ,Vector3 startPosition, Vector3 scale ,Action<FloatingText> removal)
        {
            startTime = Time.realtimeSinceStartup;

            rectTransform.position = startPosition;
            hasStaticMovement = true;
            this.transformToMoveTo = attachedTransform;
            this.startPosition = startPosition;
            this.durationOnScreen = timeOnScreen + momentaryPause;
            this.momentaryPause = momentaryPause;
            this.movementSpeed = movementSpeed;
            rectTransform.localScale = scale;
            onRemoval = removal;
        }


        /// <summary>
        /// Sets the texts color and its text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public void SetText(string text, Color color)
        {
            textToFloat.text = text;
            textToFloat.color = color;
            cvGroup.alpha = 1;//Makes it fully visible
        }

        #region Removal
        /// <summary>
        /// Calls the on removal delegate
        /// </summary>
        public void RemoveText()
        {
            onRemoval?.Invoke(this);
        }
        #endregion

        #endregion
    }
}
