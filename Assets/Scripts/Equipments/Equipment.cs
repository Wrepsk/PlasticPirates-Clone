using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : MonoBehaviour
{
    public EquipmentInfo equipmentInfo;
    public GameObject equipmentGameObject;

    public abstract void Use();
}
