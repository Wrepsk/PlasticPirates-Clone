using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIAnimator : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private float _shownTime;

    private float _hiddenTime;
    private string _changeToScene;
    private Action _hideWith;
    private bool _hidden;

    private List<KeyValuePair<float, CanvasGroup>> _showElements;
    private List<KeyValuePair<float, CanvasGroup>> _hideElements;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        
        _shownTime = Time.realtimeSinceStartup;
        _hiddenTime = -1f;

        _showElements = new List<KeyValuePair<float, CanvasGroup>>();
        _hideElements = new List<KeyValuePair<float, CanvasGroup>>();
    }

    protected virtual void Start() { }

    protected virtual void Update()
    {
        // Handle fade in animation
        
        float currentTime = Time.realtimeSinceStartup;

        if (currentTime - _shownTime < 0.5f) 
            _canvasGroup.alpha = 2 * (currentTime - _shownTime);
        else if (_hiddenTime == -1f)
            _canvasGroup.alpha = 1f;


        // Fade out animation

        if (_hiddenTime != -1f)
        {
            if (currentTime - _hiddenTime < 0.5f)
            {
                _canvasGroup.alpha = 1 - 2 * (currentTime - _hiddenTime);
            }
            else if (!_hidden)
            {
                _hidden = true;
                if (_hideWith != null)
                    _hideWith();
                else
                    SceneManager.LoadScene(_changeToScene, LoadSceneMode.Single);
            }
        }


        // Handle elements fade in animations

        List<int> _showRemove = new List<int>();

        for (int i = 0; i < _showElements.Count; i++)
        {
            var element = _showElements[i];

            float timeElapsed = currentTime - element.Key;

            if (timeElapsed < 0.5f) element.Value.alpha = timeElapsed * 2;
            else _showRemove.Add(i);
        }

        foreach (var remove in _showRemove) _showElements.RemoveAt(remove);


        // Handle elements fade out animations

        List<int> _hideRemove = new List<int>();

        for (int i = 0; i < _hideElements.Count; i++)
        {
            var element = _hideElements[i];

            float timeElapsed = currentTime - element.Key;

            if (timeElapsed < 0.5f) element.Value.alpha = 1 - timeElapsed * 2;
            else 
            {
                _hideRemove.Add(i);
                element.Value.gameObject.SetActive(false);
            }
        }

        foreach (var remove in _hideRemove) _hideElements.RemoveAt(remove);

        // Debug.Log("_showElements.Count " + _showElements.Count.ToString());
        // Debug.Log("_hideElements.Count " + _hideElements.Count.ToString());
    }

    protected void FadeOutWith(Action  action)
    {
        _hiddenTime = Time.realtimeSinceStartup;

        _hideWith = action;
    }

    protected void ChangeScene(string sceneName)
    {
        Debug.Log("ChangeScene: " + sceneName);

        _hiddenTime = Time.realtimeSinceStartup;
        _changeToScene = sceneName;
    }

    protected void ShowElement(GameObject element)
    {
        if (element.activeSelf) return;

        CanvasGroup elementCanvasGroup = element.GetComponent<CanvasGroup>();
        elementCanvasGroup.alpha = 0f;
        elementCanvasGroup.gameObject.SetActive(true);

        if (elementCanvasGroup == null)
        {
            Debug.Log("ShowElement: Element " + element.name + " does not have a Canvas Group. Skipping.");
            return;
        }

        _showElements.Add(new KeyValuePair<float, CanvasGroup>(Time.realtimeSinceStartup, elementCanvasGroup));
    }

    protected void HideElement(GameObject element)
    {
        if (!element.activeSelf) return;

        CanvasGroup elementCanvasGroup = element.GetComponent<CanvasGroup>();
        elementCanvasGroup.alpha = 1f;

        if (elementCanvasGroup == null)
        {
            Debug.Log("HideElement: Element " + element.name + " does not have a Canvas Group. Skipping.");
            return;
        }

        _hideElements.Add(new KeyValuePair<float, CanvasGroup>(Time.realtimeSinceStartup, elementCanvasGroup));
    }
}