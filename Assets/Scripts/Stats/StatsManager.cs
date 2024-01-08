using System.ComponentModel;
using UnityEngine;

public class StatsManager : MonoBehaviour, INotifyPropertyChanged
{
    public static StatsManager instance;

    public event PropertyChangedEventHandler PropertyChanged;

    private Damagable _playerBoat;

    void Awake() {
        instance = this;

        _playerBoat = FindObjectOfType<BoatMovement>();

        _playerBoat.HealthChanged += (health) =>
        {
            OnPropertyChanged(nameof(PlayerHealth));
        };
    }

    private int _collectedTrash = 0;

    public int CollectedTrash { 
        get { return _collectedTrash; }
        set {
            _collectedTrash = value;
            OnPropertyChanged(nameof(CollectedTrash));
        }
    }

    public float PlayerHealth => _playerBoat.Health;
    public float PlayerMaxHealth => _playerBoat.MaxHealth;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
