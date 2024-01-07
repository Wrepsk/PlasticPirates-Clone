using System.ComponentModel;
using UnityEngine;

public class StatsManager : MonoBehaviour, INotifyPropertyChanged
{
    public static StatsManager instance;

    public event PropertyChangedEventHandler PropertyChanged;

    void Awake() {
        instance = this;
    }

    private int _collectedTrash = 1000;

    public int CollectedTrash { 
        get { return _collectedTrash; }
        set {
            _collectedTrash = value;
            OnPropertyChanged(nameof(CollectedTrash));
        }
    }

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
