using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text costText;
    [SerializeField] GameObject descArea;

    public Button selectedUpgradeButton;

    System.Reflection.FieldInfo stat;
    string statName;
    string description;
    int newValue;
    int cost;
    
    public void StatName(string _statName)
    {
        stat = PlayerStats.instance.GetType().GetField(_statName);
        statName = _statName;
    }
    public void NewValue(int _newValue)
    {
        newValue = _newValue;
    }
    public void Cost(int _cost)
    {
        cost = _cost;
        costText.text = "Cost: " + cost;
    }
    public void Description(string _description)
    {
        description = _description;
        descText.text = description;
        descArea.SetActive(true);
    }

    public void ChangeValueOfStat()
    {
        if(selectedUpgradeButton != null)
        {
            if(StatsManager.instance.CollectedTrash >= cost)
            {
                Debug.Log("Changing value");
                Debug.Log(statName);
                StatsManager.instance.CollectedTrash -= cost;
                stat.SetValue(PlayerStats.instance, newValue);
                selectedUpgradeButton.interactable = false;
                selectedUpgradeButton.GetComponent<UpgradeButton>().isPurchased = true;
                selectedUpgradeButton = null;
            }
        }
    }
}
