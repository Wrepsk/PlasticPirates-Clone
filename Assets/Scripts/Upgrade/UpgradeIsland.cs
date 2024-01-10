using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeIsland : MonoBehaviour
{
    [SerializeField] GameObject PromptParent, UpgradeMenuParent;

    bool listening;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PromptParent.SetActive(true);
            listening = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PromptParent.SetActive(false);
            listening = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && listening)
        {
            UpgradeMenuParent.SetActive(true);
            PromptParent.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && UpgradeMenuParent.activeInHierarchy)
        {
            UpgradeMenuParent.SetActive(false);
            PromptParent.SetActive(listening);
        }
    }
}
