using FloatingText;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// A collection of 2 integers representing a minimum and maxiumum value.
/// </summary>
[System.Serializable]
public struct IntRange
{
    /// <summary>
    /// Minimum value.
    /// </summary>
    public int min;
    /// <summary>
    /// Maximum value.
    /// </summary>
    public int max;

    /// <summary>
    /// A collection of 2 integers representing a minimum and maximum value.
    /// </summary>
    /// <param name="newMin"></param>
    /// <param name="newMax"></param>
    public IntRange(int newMin, int newMax)
    {
        this.min = newMin;
        this.max = newMax;
    }
}


/// <summary>
/// Stock that can be sold in a store.
/// </summary>
[System.Serializable]
public struct SaleItem
{
    /// <summary>
    /// The name of the item being sold.
    /// </summary>
    public string itemName;
    /// <summary>
    /// The minimum and maximum cost of the item being sold.
    /// </summary>
    public IntRange costRange;
    /// <summary>
    /// The minimum and maximum quantity of the item being sold.
    /// </summary>
    public IntRange quantityRange;
    /// <summary>
    /// The actual cost of the item being sold.
    /// </summary>
    [HideInInspector] public int cost;
    /// <summary>
    /// The actual quantity of the item being sold.
    /// </summary>
    [HideInInspector] public int quantity; //How much of this item the shop keeper holds (-1 is infinity)

    /// <summary>
    /// Stock that can be sold in a store.
    /// </summary>
    /// <param name="newName"></param>
    /// <param name="newCostRange"></param>
    /// <param name="newQuantityRange"></param>
    public SaleItem(string newName, IntRange newCostRange, IntRange newQuantityRange)
    {
        this.itemName = newName;
        this.costRange = newCostRange;
        this.quantityRange = newQuantityRange;
        this.cost = 0;
        this.quantity = 0;
    }
}



/// <summary>
/// A shop entity that you can purchase items from.
/// </summary>
public class Shop : MonoBehaviour, IPauseable
{
    /// <summary> The item(s) that this shop is selling. </summary>
    public SaleItem[] inventory;
    /// <summary> The prefab for the shop's button UI. </summary>
    [SerializeField] GameObject buttonPrefab;

    //References
    /// <summary> The shop buttons that are currently being used by this shop. </summary>
    List<GameObject> buttons = new List<GameObject>();
    /// <summary> The parent object of all ShopUI buttons loaded into the game. </summary>
    RectTransform buttonContainer = null;
    /// <summary> The shop's X/close button. </summary>
    [HideInInspector] public GameObject xButton = null;
    /// <summary> The UI's title label. </summary>
    GameObject title = null;
    /// <summary>
    /// The current active event system.
    /// </summary>
    EventSystem eventSystem = null;

    //Positioning and spacing
    /// <summary> The width and height of each shop button. </summary>
    Vector2 buttonSize = new Vector2(504, 144); //The size of the buttons
    /// <summary> The size of the title's rect height. </summary>
    const float titleSize = 96; //The size of the title's rect height (Has nothing to do with the size of the text font)
    /// <summary> The spacing (in pixels) between buttons. </summary>
    const float buttonPadding = 8; //The spacing between buttons
    /// <summary> The size (in pixels) of the shop UI outline. </summary>
    const float outlinePadding = 8; //The extra background spacing along the edges of the shop UI

    /// <summary> The shop is open. </summary>
    [HideInInspector] public bool open = false;
    /// <summary> The number of children that the buttonContainer has on start. </summary>
    int childrenOnStart = 0;

    //Pause
    bool paused = false;
    public bool IsPaused { get => paused; set => paused = value; }

    private void Start()
    {
        SubscribeToGameManager();
        //Set reference to the button container and its needed children
        buttonContainer = HUDManager.instance.transform.Find("ShopButtons").GetComponent<RectTransform>();
        xButton = buttonContainer.transform.Find("XButton").gameObject;
        title = buttonContainer.transform.Find("Title").gameObject;
        //Get event system
        eventSystem = EventSystem.current;
        //Count button container children
        childrenOnStart = buttonContainer.transform.childCount;
        //Set cost and quantity of each sale item
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i].cost = Random.Range((int)inventory[i].costRange.min, (int)inventory[i].costRange.max);
            inventory[i].quantity = Random.Range((int)inventory[i].quantityRange.min, (int)inventory[i].quantityRange.max);
        }
    }


    /// <summary> Opens this entity's shop. </summary>
    public void OpenShop()
    {
        buttons.Clear();

        StartCoroutine(AssignNavigation());

        //Set self reference in x button
        xButton.GetComponent<ShopXUI>().shop = this;

        //Unlock mouse
        CameraController.instance.LockMouse(false);

        //Enable title
        title.SetActive(true);
        xButton.SetActive(true);
        //Set background size
        buttonContainer.sizeDelta = new Vector2(buttonSize.x + (outlinePadding * 2), ((buttonSize.y + buttonPadding) * inventory.Length) - buttonPadding + (outlinePadding * 2) + titleSize);
        //Set the text position
        title.transform.localPosition = new Vector2(0, (buttonContainer.sizeDelta.y / 2) - (titleSize / 2));
        xButton.transform.localPosition = new Vector2((buttonContainer.sizeDelta.x / 2), (buttonContainer.sizeDelta.y / 2));

        //Enable buttons
        for (int i = 0; i < inventory.Length; i++)
        {
            ShopUI newButton = null;
            //Create a new button
            if (buttonContainer.childCount - 1 < i + childrenOnStart)
            {
                newButton = Instantiate(buttonPrefab, buttonContainer).GetComponent<ShopUI>();
                buttons.Add(newButton.gameObject);
            }
            //Use existing button
            else
            {
                buttonContainer.GetChild(i + childrenOnStart).gameObject.SetActive(true);
                newButton = buttonContainer.GetChild(i + childrenOnStart).GetComponent<ShopUI>();
                buttons.Add(newButton.gameObject);
            }



            //Set button position
            newButton.transform.position = new Vector3(0, -(buttonSize.y * i), 0);
            //Set button text
            newButton?.UpdateButton(this, i);

            newButton.transform.localPosition = new Vector3(0, ((buttonContainer.sizeDelta.y / 2) - (buttonSize.y / 2))   - ((buttonSize.y + buttonPadding) * i) - outlinePadding - titleSize);
        }
    }

    /// <summary> Closes this entity's shop. </summary>
    public void CloseShop()
    {
        open = false;
        MobileControlManager.instance.shopButton?.Toggle();
        //Lock mouse
        CameraController.instance.LockMouse(true);

        if (xButton == null)
        {
            //Set reference to the button container and its needed children
            buttonContainer = HUDManager.instance.transform.Find("ShopButtons").GetComponent<RectTransform>();
            xButton = buttonContainer.transform.Find("XButton").gameObject;
        }

        xButton.GetComponent<ShopXUI>().shop = null;

        //Disable all buttons
        for (int i = 0; i < buttonContainer.childCount; i++)
        {
            buttonContainer.GetChild(i).gameObject.SetActive(false);
        }
        //Hide backgrond
        buttonContainer.sizeDelta = Vector2.zero;

        //Unpause the player
        Character player = GameManager.instance.player;
        if (player != null)
        {
            Movement playerMovement = player.GetComponent<Movement>();
            CharacterInput playerInput = player.GetComponent<CharacterInput>();
            if (!paused)
            {
                playerMovement?.Play();
                playerInput?.Play();
            }
        }
    }

    /// <summary> Assigns the navigation for the shop's buttons. </summary>
    public IEnumerator AssignNavigation()
    {
        yield return new WaitForEndOfFrame();
        open = true;

        //Assign button navigations
        if (eventSystem)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                GameObject newButton = buttonContainer.GetChild(i + childrenOnStart).gameObject;
                Button button = newButton.GetComponent<Button>();
                Navigation navigation = button.navigation;
                if (i == 0)
                {
                    Button closeButton = xButton.GetComponent<Button>();
                    Navigation closeButtonNav = closeButton.navigation;
                    //Set button as selected
                    eventSystem.SetSelectedGameObject(newButton);
                    //Set button's UP navigation
                    navigation.selectOnUp = closeButton;
                    //Set the close button's DOWN navigation
                    closeButtonNav.selectOnDown = button;
                    closeButton.navigation = closeButtonNav;
                }
                else
                {
                    Button prevButton = buttonContainer.GetChild(i + childrenOnStart - 1).GetComponent<Button>();
                    Navigation prevNavigation = prevButton.navigation;
                    //Set button's UP navigation
                    navigation.selectOnUp = prevButton;
                    //Set the previouis buttons DOWN navigation
                    prevNavigation.selectOnDown = button;
                    prevButton.navigation = prevNavigation;
                }
                button.navigation = navigation;
            }
            //Disable buttons that have 0 quantity
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].quantity == 0)
                {
                    buttonContainer.GetChild(i + childrenOnStart).GetComponent<ShopUI>().Disable();
                }
            }
        }
    }



    public void Pause()
    {
        paused = true;
        if (open)
        {
            CloseShop();
        }
    }

    public void Play()
    {
        paused = false;
        if (open)
        {
            CloseShop();
        }
    }

    public void SubscribeToGameManager()
    {
        if (!GameManager.instance.pausables.Contains(this))
        { GameManager.instance.pausables.Add(this); }
    }
}