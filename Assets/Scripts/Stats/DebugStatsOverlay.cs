using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class DebugStatsOverlay : MonoBehaviour
{

    HUD hud;

    Text collectedTrash;

    Canvas canvas;

    void Start() {
        Transform panel = transform.Find("Canvas/Panel");
        collectedTrash = panel.Find("CollectedTrash/Value").gameObject.GetComponent<Text>();

        StatsManager.instance.PropertyChanged += OnStatsChanged;

        hud = FindObjectOfType<HUD>();
        canvas = transform.Find("Canvas").GetComponent<Canvas>();
    }

    void Update()
    {
        if (hud != null)
        {
            if (Input.GetKey(KeyCode.K))
            {
                canvas.enabled = true;
                hud.gameObject.SetActive(false);
            }
            else
            {
                canvas.enabled = false;
                hud.gameObject.SetActive(true);
            }
        }
    }

    void OnStatsChanged(object sender, PropertyChangedEventArgs propertyChange) {
        string name = propertyChange.PropertyName;

        switch (name)
        {
            case "CollectedTrash":
                collectedTrash.text = StatsManager.instance.CollectedTrash.ToString();
                break;
        }
    }
}
