//This is a test file

using UnityEngine;
using System.Collections;

public class VariablesAndStuff : MonoBehaviour
{
    public int counter;
    void Awake () {
        counter = 0;
    }
    

    void Start () {
        counter += 1;
    }
    void Update () {
        counter += 1;
        Debug.Log(counter);
    }

}