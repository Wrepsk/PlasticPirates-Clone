using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : UIAnimator
{
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
    }

    private void LaunchGame() 
    {
        StartCoroutine(LaunchGameSequence());
    }

    private IEnumerator LaunchGameSequence()
    {

        HideElement(_settingsPane);
        HideElement(_rightPane);
        ShowElement(_loadingPane);
        HideElement(_storyControlsPane);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);

        while (!operation.isDone)
        {
            _loadingSlider.value = operation.progress;
            yield return null;
        }
    }

    private void ToggleSettingsPane() 
    {
        if (_settingsPane.activeSelf) HideElement(_settingsPane);
        else ShowElement(_settingsPane);
    }

    private void ToggleStoryControlsPane()
    {
        ShowElement(_storyControlsPane);
    }

    private void QuitGame() 
    {
        FadeOutWith(() => Application.Quit());
    }
}
