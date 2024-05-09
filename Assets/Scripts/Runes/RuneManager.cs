using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RuneManager : MonoBehaviour, IPauseable
{
    // Singleton instance of the RuneManager
    public static RuneManager instance { get; private set; }

    // Property from IPauseable interface
    public bool IsPaused { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    // Highlight color for runes
    public Color RUNEHIGHLIGHT = new Color(0.25f, 0, 0, 0.8f);
    public Color SIGILICECOLOR = new Color(0, 0, 1, 0.8f);
    public Color SIGILFIRECOLOR = new Color(1, 0, 0, 0.8f);
    public Color SIGILEARTHCOLOR = new Color(0, 1, 0, 0.8f);
    // Constants for rune counts
    [HideInInspector] public const int ELEMENTALRUNECOUNT = 3;
    private const int MODIFIERRUNECOUNT = 5;
    // Reference to the rune canvas
    [SerializeField] private Canvas runeCanvas;
    // Arrays for rune sprites
    private Sprite[] modifierRuneSprites = new Sprite[MODIFIERRUNECOUNT];
    private Sprite[] elementalRuneSprites = new Sprite[ELEMENTALRUNECOUNT];

    // Index of selected element rune and modifier runes
    [HideInInspector] public int selectedElementRune
    {
        get { return _selectedElementRune; }
        set { _selectedElementRune = value; UpdateSigilColor(); }
    }
    private int _selectedElementRune = 0;
    [HideInInspector] public int[] selectedModRunes = new int[3] { 0, 0, 0 };

    // Visual representations of selected runes
    public Image[] selectedModRuneVisual = new Image[3];
    public Image selectedElementRuneVisual;

    // Flag for enabling/disabling runes
    [HideInInspector] public bool runesEnabled = true;

    // Visual representation of the sigil
    public Image sigilVisual;
    // Flag for checking if the modifier rune menu is open
    [HideInInspector] public bool modRuneMenuOpen = false;

    // Index of selected modifier rune grouping
    private int selectedModRuneMenuGrouping
    {
        get { return _selectedModRuneMenuGrouping; }
        set { _selectedModRuneMenuGrouping = value; UpdateStatsText(); }
    }
    private int _selectedModRuneMenuGrouping = 0;
    // Animator component
    private Animator anim;

    // Flag for pausing state
    private bool paused = false;

    // Directional input for selecting modifier runes
    private Vector2 directionalInput = Vector2.zero;
    public TMP_Text statsText;
    private void Start()
    {
        // Initialize modRuneMenuOpen state
        anim.SetBool("MenuOpen", false);
        modRuneMenuOpen = false;
        statsText.text = "";
        statsText.enabled = false;
        selectedElementRune = 0;
    }

    private void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        // Load rune sprites
        elementalRuneSprites = Resources.LoadAll<Sprite>("Runes/ElementalRuneTiles");
        modifierRuneSprites = Resources.LoadAll<Sprite>("Runes/ModifierRuneTiles");

        // Get Animator component
        anim = GetComponent<Animator>();

        // Get rune canvas
        runeCanvas = transform.GetChild(0).GetComponent<Canvas>();

        // Set initial rune sprites
        selectedElementRuneVisual.sprite = elementalRuneSprites[selectedElementRune];
        for (int i = 0; i < 3; i++)
        {
            selectedModRuneVisual[i].sprite = modifierRuneSprites[selectedModRunes[i]];
        }
    }

    private void FixedUpdate()
    {
        // Rotate sigil if it exists and not paused
        if (sigilVisual != null && !paused)
        {
            if (modRuneMenuOpen)
            {
                // Spin sigil while menu open
                sigilVisual.rectTransform.localEulerAngles = new Vector3(0, 0, sigilVisual.rectTransform.localEulerAngles.z + 1);
            }
            else
            {
                sigilVisual.rectTransform.localEulerAngles = Vector3.zero;
            }
        }
    }

    public void OnOpenRuneMenu()
    {
        if (paused || (GameManager.instance.player.characterType != CharacterType.mage)) {
            return;
        }

        // Toggle modifier rune menu
        modRuneMenuOpen = !modRuneMenuOpen;
        if (!ControllerManager.instance.CONTROLLERENABLED)
        {
            CameraController.instance.LockMouse(!modRuneMenuOpen);
        }
        anim.SetBool("MenuOpen", modRuneMenuOpen);

        if (modRuneMenuOpen) { HighlightModRune(); }
        else { ClearHighlight(); }
        UpdateStatsText();
    }

    public void OnRight()
    {
        if (paused) {
            return;
        }
        if (modRuneMenuOpen)
        {
            // Cycle modifier runes if menu open
            selectedModRunes[selectedModRuneMenuGrouping]++;
            if (selectedModRunes[selectedModRuneMenuGrouping] > MODIFIERRUNECOUNT-1)
            {
                selectedModRunes[selectedModRuneMenuGrouping] = 0;
            }
            selectedModRuneVisual[selectedModRuneMenuGrouping].sprite = modifierRuneSprites[selectedModRunes[selectedModRuneMenuGrouping]];
            UpdateStatsText();
        }
        else
        {
            // Cycle elemental runes if menu not open
            selectedElementRune++;
            if (selectedElementRune >= ELEMENTALRUNECOUNT)
            {
                selectedElementRune = 0;
            }
            selectedElementRuneVisual.sprite = elementalRuneSprites[selectedElementRune];
        }
    }

    public void OnLeft()
    {
        if (paused) {
            return;
        }
        if (modRuneMenuOpen)
        {
            selectedModRunes[selectedModRuneMenuGrouping]--;
            if (selectedModRunes[selectedModRuneMenuGrouping] < 0)
            {
                selectedModRunes[selectedModRuneMenuGrouping] = MODIFIERRUNECOUNT - 1;
            }
            selectedModRuneVisual[selectedModRuneMenuGrouping].sprite = modifierRuneSprites[selectedModRunes[selectedModRuneMenuGrouping]];
            UpdateStatsText();
        }
        else
        {
            selectedElementRune--;
            if (selectedElementRune < 0)
            {
                selectedElementRune = ELEMENTALRUNECOUNT - 1;
            }
            selectedElementRuneVisual.sprite = elementalRuneSprites[selectedElementRune];
        }
    }

    public void ToggleRuneUI(bool state)
    {
        // Enable/disable rune UI
        runesEnabled = state;
        runeCanvas.enabled = state;
    }

    public int GetManaCost()
    {
        // Calculate total mana cost of all modifier runes
        int t = 0;
        for (int i = 0; i < selectedModRunes.Length; i++)
        {
            t += RuneDataSheet.runeStats[selectedModRunes[i]].mana;
        }
        return t;
    }

    public int GetElement()
    {
        return selectedElementRune;
    }

    public void CalculateAngleForModRuneMenu()
    {
        if (ControllerManager.instance.CONTROLLERENABLED || Application.isMobilePlatform)
        {
            // Get angle based on right stick for controller
            float angle = Vector2.SignedAngle(new Vector2(0, 1), directionalInput);
            if (angle > -60 && angle < 60)
            {
                selectedModRuneMenuGrouping = 0;
            }
            else if (angle < 60)
            {
                selectedModRuneMenuGrouping = 1;
            }
            else if (angle > 60)
            {
                selectedModRuneMenuGrouping = 2;
            }
        }
        else
        {
            // Get angle based on screen sectors for mouse
            selectedModRuneMenuGrouping = (directionalInput.y >= Screen.height / 2) ? 0 : (directionalInput.x > Screen.width / 2) ? 1 : 2;
        }

        HighlightModRune();
    }

    public void OnRStick(InputValue inputValue)
    {
        if (modRuneMenuOpen && ControllerManager.instance.CONTROLLERENABLED)
        {
            directionalInput = inputValue.Get<Vector2>();
            CalculateAngleForModRuneMenu();
        }
    }

    public void OnMobileStick(Vector2 inputVec)
    {
        if (modRuneMenuOpen && Application.isMobilePlatform)
        {
            directionalInput = inputVec;
            CalculateAngleForModRuneMenu();
        }
    }

    public void OnMouse(InputValue inputValue)
    {
        if (modRuneMenuOpen && !ControllerManager.instance.CONTROLLERENABLED && !Application.isMobilePlatform)
        {
            directionalInput = inputValue.Get<Vector2>();
            CalculateAngleForModRuneMenu();
        }
    }
    void UpdateSigilColor()
    {
        Color newColor = Color.white;
        switch (selectedElementRune)
        {
            case 0:
                newColor = SIGILICECOLOR;
                break;
            case 1:
                newColor = SIGILFIRECOLOR;
                break;
            case 2:
                newColor = SIGILEARTHCOLOR;
                break;
            default:
                newColor = Color.white;
                break;
        }
        sigilVisual.color = newColor;
    }
    void UpdateStatsText()
    {
        if (modRuneMenuOpen)
        {
            statsText.enabled = true;
            RuneData data = RuneDataSheet.runeStats[selectedModRunes[selectedModRuneMenuGrouping]];
            statsText.text = $"DMG : {data.damage}\nRNG : {data.range}\nSPD : {data.speed}\nMANA : {data.mana}";
        }
        else
        {
            statsText.enabled = false;
        }
    }
    void HighlightModRune()
    {
        // Highlight selected rune
        for (int i = 0; i < 3; i++)
        {
            selectedModRuneVisual[i].color = (i == selectedModRuneMenuGrouping) ? RUNEHIGHLIGHT : Color.white;
        }
    }
    void ClearHighlight()
    {
        for (int i = 0; i < 3; i++)
        {
            selectedModRuneVisual[i].color = Color.white;
        }
    }
    public void Pause()
    {
        paused = true;
    }
    public void Play()
    {
        paused = false;
    }
    public void SubscribeToGameManager()
    {
        throw new System.NotImplementedException();
    }
}
