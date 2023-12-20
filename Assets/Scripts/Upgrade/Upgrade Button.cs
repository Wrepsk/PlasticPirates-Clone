using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] Button nextUpgrade;
    [SerializeField] UpgradeMenu upgradeMenu;
    [SerializeField] GameObject checkmark;
    [SerializeField] Image lineToNextUpgrade;

    private Color oldLineColor;

    public bool isPurchased = false;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(UpdateUpgradeMenuVariable);
        oldLineColor = lineToNextUpgrade.color;
    }

    void UpdateUpgradeMenuVariable()
    {
        upgradeMenu.selectedUpgradeButton = this.GetComponent<Button>();
    }

    private void Update()
    {
        if(nextUpgrade != null)
        {
            if (this.GetComponent<Button>().interactable == false && isPurchased)
            {
                Color newColor = lineToNextUpgrade.color;
                newColor = new Color(202, 202, 202);
                newColor.a = 255;
                lineToNextUpgrade.color = newColor;
                checkmark.SetActive(true);
                nextUpgrade.interactable = true;
                isPurchased = false;
            }
            if(nextUpgrade.interactable == false && this.GetComponent<Button>().interactable == false)
            {
                lineToNextUpgrade.color = oldLineColor;
            }
        }
    }
}
