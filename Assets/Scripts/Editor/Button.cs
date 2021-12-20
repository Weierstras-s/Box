using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour
{    
    public Button btn;
    public Text txt;
    public void Save() {
        Debug.Log("Hello");
    }

    // Start is called before the first frame update
    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            Debug.Log("Åö×²µ½UI");
        }
    }
}
