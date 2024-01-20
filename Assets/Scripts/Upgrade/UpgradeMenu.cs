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

    public Damagable playerBoat;

    public Button selectedUpgradeButton;

    System.Reflection.PropertyInfo stat;
    string statName;
    string description;
    int newValue;
    int cost;
    
    public void StatName(string _statName)
    {
        stat = playerBoat.GetType().GetProperty(_statName);
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
                stat.SetValue(playerBoat, newValue);

                if (statName == "MaxHealth") 
                {	
                    playerBoat.Health = playerBoat.MaxHealth;
                }

                playerBoat.DealDamage(0);
                selectedUpgradeButton.interactable = false;
                selectedUpgradeButton.GetComponent<UpgradeButton>().isPurchased = true;
                selectedUpgradeButton = null;
                descArea.SetActive(false);

            }
        }
    }
}
