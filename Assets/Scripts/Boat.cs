using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WaterSystem.Physics;

public class Boat : MonoBehaviour
{
    public float defSpeed = 5.0f; // 5 m/s
    private float speed;
    public float boostMultipliar = 4f;
    public float rotationSpeed = 90.0f; // 90 deg/s

    [SerializeField] Transform equipmentHorizontalMover;
    [SerializeField] Transform equipmentVerticalMover;
    float rotationY;
    float rotationZ;

    [SerializeField] Equipment[] equipments;
    int equipmentIndex;
    int previousEquipmentIndex = -1;

    Transform rotatorTransform;

    void Start()
    {
        rotatorTransform = transform.GetChild(0);
        speed = defSpeed;
        EquipEquipment(0);
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        RotateEquipmentHolder();
        SwitchEquipment();

        if(equipments[equipmentIndex] != null)
        {
            if (equipments[equipmentIndex].equipmentInfo.isAutomatic && Input.GetMouseButton(0))
                equipments[equipmentIndex].Use();
            else if(Input.GetMouseButtonDown(0))
                    equipments[equipmentIndex].Use();
        }

    }

    void Move()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.position += rotatorTransform.rotation * new Vector3(verticalInput * speed * Time.deltaTime, 0, 0);

        rotatorTransform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = defSpeed * boostMultipliar;
        }
        else
            speed = defSpeed;
    }

    void RotateEquipmentHolder()
    {
        rotationY += 350f * Input.GetAxis("Mouse X") * Time.deltaTime;
        rotationZ -= 350f * Input.GetAxis("Mouse Y") * Time.deltaTime;

        rotationZ = Mathf.Clamp(rotationZ, -45f, 45f);

        equipmentHorizontalMover.transform.localEulerAngles = new Vector3(0, rotationY, 0);
        equipmentVerticalMover.transform.localEulerAngles = new Vector3(0, 0, rotationZ);

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
