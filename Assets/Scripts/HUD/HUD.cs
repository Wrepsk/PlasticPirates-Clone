using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Text collectedTrash;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] BoatMovement boat;

    void Start() {
        Transform panel = transform.Find("Canvas/Panel");
        collectedTrash = panel.Find("TrashBar/TrashCount").gameObject.GetComponent<Text>();

        StatsManager.instance.PropertyChanged += OnStatsChanged;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if(boat.inUpgradeIsland)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                upgradeMenu.SetActive(!upgradeMenu.activeInHierarchy);
                Cursor.visible = !Cursor.visible;
                Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
                boat.GetComponent<BoatEquipments>().canUseWeapons = !boat.GetComponent<BoatEquipments>().canUseWeapons;
                boat.GetComponentInChildren<RotateCamera>().inUpgradeMenu = !boat.GetComponentInChildren<RotateCamera>().inUpgradeMenu;
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
