using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Text collectedTrash;

    void Start() {
        Transform panel = transform.Find("Canvas/Panel");
        collectedTrash = panel.Find("TrashBar/TrashCount").gameObject.GetComponent<Text>();

        StatsManager.instance.PropertyChanged += OnStatsChanged;
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
