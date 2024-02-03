using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyUpgradeHoverSound : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip hoverClip;

    // Start is called before the first frame update
    void Start()
    {
        AddHoverEvents(this.GetComponent<Button>());
    }

    private void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { audioSource.PlayOneShot(hoverClip, 0.15f); });
        trigger.triggers.Add(entryEnter);
    }
}
