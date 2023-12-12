using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameObject _settingsPane;
    private Button _settingsPaneDoneButton;

    private Transform _buttonsList;

    private Button _playButton;
    private Button _settingsButton;
    private Button _quitButton;


    private void Start() {
        _settingsPane = transform.Find("SettingsPane").gameObject;
        _settingsPaneDoneButton = transform.Find("SettingsPane/DoneButton").GetComponent<Button>();

        _buttonsList = transform.Find("RightPane/ButtonList");

        _playButton     = _buttonsList.Find("PlayButton").GetComponent<Button>();
        _settingsButton = _buttonsList.Find("SettingsButton").GetComponent<Button>();
        _quitButton     = _buttonsList.Find("QuitButton").GetComponent<Button>();

        _playButton.onClick.AddListener(LaunchGame);
        _settingsButton.onClick.AddListener(ToggleSettingsPane);
        _settingsPaneDoneButton.onClick.AddListener(ToggleSettingsPane);
        _quitButton.onClick.AddListener(QuitGame);
    }

    private void LaunchGame() {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    private void ToggleSettingsPane() {
        _settingsPane.SetActive(!_settingsPane.activeSelf);
    }

    private void QuitGame() {
        Application.Quit();
    }
}
