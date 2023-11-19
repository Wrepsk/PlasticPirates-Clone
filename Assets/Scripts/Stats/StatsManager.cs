using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    public int CollectedTrash { get; set; } = 0;

    void Awake() {
        instance = this;
    }
}
