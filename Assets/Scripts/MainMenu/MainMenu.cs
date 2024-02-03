using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : UIAnimator
{
    public AudioSource backgroundAudioSource;
    public AudioClip backgroundClip;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    private GameObject _rightPane;
    private GameObject _loadingPane;
    private GameObject _settingsPane;
    private GameObject _storyControlsPane;

    private Slider _loadingSlider;

    private Transform _buttonsList;

    private Button _playButton;
    private Button _settingsButton;
    private Button _quitButton;
    private Button _settingsPaneDoneButton;
    private Button _gotItButton;


    protected override void Start() 
    {
        // UIAnimator
        base.Start();

        _rightPane = transform.Find("Center/RightPane").gameObject;
        _loadingPane = transform.Find("Center/LoadingPane").gameObject;
        _settingsPane = transform.Find("Center/SettingsPane").gameObject;
        _settingsPaneDoneButton = transform.Find("Center/SettingsPane/DoneButton").GetComponent<Button>();
        _storyControlsPane = transform.Find("Center/StoryControlsPane").gameObject;
        _gotItButton = transform.Find("Center/StoryControlsPane/GotItButton").GetComponent<Button>();

        _loadingSlider = _loadingPane.transform.Find("Slider").GetComponent<Slider>();

        _buttonsList = transform.Find("Center/RightPane/ButtonList");

        _playButton     = _buttonsList.Find("PlayButton").GetComponent<Button>();
        _settingsButton = _buttonsList.Find("SettingsButton").GetComponent<Button>();
        _quitButton     = _buttonsList.Find("QuitButton").GetComponent<Button>();

        _gotItButton.onClick.AddListener(LaunchGame);
        _playButton.onClick.AddListener(ToggleStoryControlsPane);
        _settingsButton.onClick.AddListener(ToggleSettingsPane);
        _settingsPaneDoneButton.onClick.AddListener(ToggleSettingsPane);
        _quitButton.onClick.AddListener(QuitGame);

        AddHoverEvents(_playButton);
        AddHoverEvents(_settingsButton);
        AddHoverEvents(_quitButton);
        AddHoverEvents(_settingsPaneDoneButton);
        AddHoverEvents(_gotItButton);

        backgroundAudioSource.clip = backgroundClip;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    private void LaunchGame() 
    {
        backgroundAudioSource.PlayOneShot(clickClip, 0.35f);
        StartCoroutine(LaunchGameSequence());
    }

    private IEnumerator LaunchGameSequence()
    {
        HideElement(_settingsPane);
        HideElement(_rightPane);
        ShowElement(_loadingPane);
        HideElement(_storyControlsPane);

        Invoke("StopAudio", 1f);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync("Main World Scene", LoadSceneMode.Single);

        while (!operation.isDone)
        {
            _loadingSlider.value = operation.progress;
            yield return null;
        }    
    }

    private void ToggleSettingsPane() 
    {
        backgroundAudioSource.PlayOneShot(clickClip, 0.35f);
        if (_storyControlsPane.activeSelf) HideElement(_storyControlsPane);        
        if (_settingsPane.activeSelf) HideElement(_settingsPane);
        else ShowElement(_settingsPane);
    }

    private void ToggleStoryControlsPane()
    {
        backgroundAudioSource.PlayOneShot(clickClip, 0.35f);
        if (_settingsPane.activeSelf) HideElement(_settingsPane);
        if (_storyControlsPane.activeSelf) HideElement(_storyControlsPane);
        ShowElement(_storyControlsPane);
    }

    private void QuitGame() 
    {
        backgroundAudioSource.PlayOneShot(clickClip, 0.35f);
        FadeOutWith(() => Application.Quit());
    }

    private void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { backgroundAudioSource.PlayOneShot(hoverClip, 0.15f); });
        trigger.triggers.Add(entryEnter);
    }

    private void StopAudio()
    {
        backgroundAudioSource.Stop();
    }
}
