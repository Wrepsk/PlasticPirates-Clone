using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : MonoBehaviour
{
    public EquipmentInfo equipmentInfo;
    public GameObject equipmentGameObject;

    public AudioSource audioSource;
    public AudioClip audioClip;
    protected bool suppressLastAudio = true;
    protected bool disableAutoPlay = false;

    public float cooldown = 0f; // no cooldown
    private float _timeLastUsed = -Mathf.Infinity;

    public abstract void Use();

    public void BaseUse()
    {
        if (Time.realtimeSinceStartup - _timeLastUsed < cooldown) return;

        _timeLastUsed = Time.realtimeSinceStartup;

        if (audioSource != null && audioClip != null && !disableAutoPlay && !(!suppressLastAudio && audioSource.isPlaying))
        {
            audioSource.PlayOneShot(audioClip);
        }

        Use();
    }
}
