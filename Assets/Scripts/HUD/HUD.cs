using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : UIAnimator
{
    public static HUD instance;

    public bool HudActive => _menu.activeSelf;
    //public bool DeathScreenActive => _deathScreen.activeSelf;

    

    // Menu
    [SerializeField] private GameObject _menu;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    // Death screen
    [SerializeField] private GameObject _deathScreen;
    [SerializeField] private Button _restartAfterDeathButton;
    [SerializeField] private Button _returnAfterDeathButton;
    private bool _deathIsToggled = false;

    // Overlay message
    [SerializeField] private GameObject _message;
    private float _hideMessageAt;

    private bool _lastFrameInUpgradeIsland;

    // HUD elements
    [SerializeField] private GameObject _cross;
    private Text _collectedTrash;
    private Slider _healthSlider;

    [SerializeField] GameObject upgradeMenu;
    [SerializeField] BoatMovement boat;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
    }

    protected override void Start() 
    {
        base.Start();

        Transform panel = transform.Find("Canvas/Panel");
        _collectedTrash = panel.Find("TrashBar/TrashCount").GetComponent<Text>();
        _healthSlider = panel.Find("HealthBar/Slider").GetComponent<Slider>();

        StatsManager.instance.PropertyChanged += OnStatsChanged;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _quitButton.onClick.AddListener(() => ChangeScene("MainMenu"));
        _resumeButton.onClick.AddListener(ToggleMenu);

        _returnAfterDeathButton.onClick.AddListener(() => ChangeScene("MainMenu"));
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();

        if (_menu.activeSelf) return;

        if (boat.IsDead && !_deathIsToggled) ToggleDeathScreen();

        if (_deathScreen.activeSelf) return;


        if(boat.inUpgradeIsland)
        {
            if (!_lastFrameInUpgradeIsland) 
            {
                Debug.Log("Show message");
                ShowMessage("Press TAB for upgrades", 2);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _cross.SetActive(upgradeMenu.activeInHierarchy);
                upgradeMenu.SetActive(!upgradeMenu.activeInHierarchy);
                Cursor.visible = !Cursor.visible;
                Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
                boat.GetComponent<BoatEquipments>().canUseWeapons = !boat.GetComponent<BoatEquipments>().canUseWeapons;
                boat.GetComponentInChildren<RotateCamera>().inUpgradeMenu = !boat.GetComponentInChildren<RotateCamera>().inUpgradeMenu;
            }
            _lastFrameInUpgradeIsland = true;
        }
        else
        {
            _lastFrameInUpgradeIsland = false;
        }

        float messageShownElapsed = Time.realtimeSinceStartup - _hideMessageAt;

        if (Time.realtimeSinceStartup > _hideMessageAt)
        {
            _hideMessageAt = Mathf.Infinity;
            HideElement(_message);
        }
    }

    void OnStatsChanged(object sender, PropertyChangedEventArgs propertyChange) {
        string name = propertyChange.PropertyName;

        switch (name)
        {
            case "CollectedTrash":
                _collectedTrash.text = StatsManager.instance.CollectedTrash.ToString();
                break;

            case "PlayerHealth":
                _healthSlider.value = StatsManager.instance.PlayerHealth / StatsManager.instance.PlayerMaxHealth;
                break;
        }
    }

    public void ToggleMenu()
    {
        bool menuActive = _menu.activeSelf;

        if (menuActive) HideElement(_menu);
        else ShowElement(_menu);

        Cursor.visible = !menuActive;
        Cursor.lockState = !menuActive ? CursorLockMode.None : CursorLockMode.Locked;
        boat.GetComponent<BoatEquipments>().canUseWeapons = menuActive;
        boat.GetComponentInChildren<RotateCamera>().inUpgradeMenu = !menuActive;

        _cross.SetActive(menuActive);
    }

    public void ToggleDeathScreen()
    {
        _deathIsToggled = true;
        bool screenActive = _deathScreen.activeSelf;

        if (screenActive) HideElement(_deathScreen);
        else ShowElement(_deathScreen);

        Cursor.visible = !screenActive;
        Cursor.lockState = !screenActive ? CursorLockMode.None : CursorLockMode.Locked;
        boat.GetComponent<BoatEquipments>().canUseWeapons = screenActive;
        boat.GetComponentInChildren<RotateCamera>().inUpgradeMenu = !screenActive;
    }

    public void ShowMessage(string message, float time = 1.5f)
    {
        _message.GetComponentInChildren<TextMeshProUGUI>().text = message;
        _hideMessageAt = Time.realtimeSinceStartup + time;

        ShowElement(_message);
    }
}
