using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("Name " + gameObject.name);
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable() " + gameObject.name);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
