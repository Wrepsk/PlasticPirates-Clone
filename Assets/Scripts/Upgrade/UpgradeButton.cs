using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] Button nextUpgrade;
    [SerializeField] UpgradeMenu upgradeMenu;
    [SerializeField] GameObject checkmark;
    [SerializeField] Image lineToNextUpgrade;

    private Color oldLineColor;

    public bool isPurchased = false;

    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(UpdateUpgradeMenuVariable);
        AddHoverEvents(this.GetComponent<Button>());
        oldLineColor = lineToNextUpgrade.color;
    }

    void UpdateUpgradeMenuVariable()
    {
        audioSource.PlayOneShot(clickClip, 0.35f);
        upgradeMenu.selectedUpgradeButton = this.GetComponent<Button>();
    }

    private void Update()
    {
        if (this.GetComponent<Button>().interactable == false && isPurchased)
        {
            Color newColor = lineToNextUpgrade.color;
            newColor = new Color(202, 202, 202);
            newColor.a = 255;
            lineToNextUpgrade.color = newColor;
            checkmark.SetActive(true);
            if(nextUpgrade != null)
                nextUpgrade.interactable = true;
            isPurchased = false;
        }
        if (nextUpgrade != null)
        {
            if(nextUpgrade.interactable == false && this.GetComponent<Button>().interactable == false)
            {
                lineToNextUpgrade.color = oldLineColor;
            }
        }
    }

    private void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { if (this.GetComponent<Button>().interactable == true) { audioSource.PlayOneShot(hoverClip, 0.15f); } });
        trigger.triggers.Add(entryEnter);
    }
}
