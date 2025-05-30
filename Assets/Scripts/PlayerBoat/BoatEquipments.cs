using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using WaterSystem.Physics;

public class BoatEquipments : MonoBehaviour
{

    [SerializeField] Transform equipmentHorizontalMover;
    [SerializeField] Transform equipmentVerticalMover;
    [SerializeField] GameObject cam;
    [SerializeField] BoatTilter tilter;
    public float verticalOffset;
    float rotationY;
    float rotationZ;

    public bool canUseWeapons = true;

    [SerializeField] Equipment[] equipments;
    int equipmentIndex;
    int previousEquipmentIndex = -1;

    Transform rotatorTransform;

    void Start()
    {
        EquipEquipment(0);
    }

    void Update()
    {
        if (!canUseWeapons) return;

        RotateEquipmentHolder();
        SwitchEquipment();

        if(equipments[equipmentIndex] != null)
        {
            if(canUseWeapons)
            {
                if (equipments[equipmentIndex].equipmentInfo.isAutomatic && Input.GetMouseButton(0))
                    equipments[equipmentIndex].BaseUse();
                else if (Input.GetMouseButtonDown(0))
                    equipments[equipmentIndex].BaseUse();
                else if (equipments[equipmentIndex].equipmentInfo.isAutomatic && Input.GetMouseButtonUp(0))
                    equipments[equipmentIndex].BaseStopUse();
            }
        }
    }

    void RotateEquipmentHolder()
    {
        rotationY += 350f * Input.GetAxis("Mouse X") * Time.deltaTime;
        rotationZ -= 350f * Input.GetAxis("Mouse Y") * Time.deltaTime;

        rotationZ = Mathf.Clamp(rotationZ, -15f, 30f);

        equipmentHorizontalMover.transform.localEulerAngles = new Vector3(0, rotationY, 0);
        //equipmentVerticalMover.transform.localEulerAngles = new Vector3(0, 0, rotationZ);
        Vector3 verticalRot = equipmentVerticalMover.transform.localEulerAngles;
        verticalRot.z = cam.transform.localEulerAngles.x + verticalOffset - tilter.targetTilt;
        equipmentVerticalMover.transform.localEulerAngles = verticalRot;
    }

        void EquipEquipment(int index)
    {
        if (index == previousEquipmentIndex)
            return;

        equipmentIndex = index;
        equipments[equipmentIndex].equipmentGameObject.SetActive(true);

        if(previousEquipmentIndex != -1)
        {
            equipments[previousEquipmentIndex].equipmentGameObject.SetActive(false);
        }

        previousEquipmentIndex = equipmentIndex;
    }

    void SwitchEquipment()
    {
        for (int i = 0; i < equipments.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString()))
            {
                EquipEquipment(i); 
                break;
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (equipmentIndex >= equipments.Length - 1)
                EquipEquipment(0);
            else
                EquipEquipment(equipmentIndex + 1);
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (equipmentIndex <= 0)
                EquipEquipment(equipments.Length - 1);
            else
                EquipEquipment(equipmentIndex - 1);
        }
    }
}
