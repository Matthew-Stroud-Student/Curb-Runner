using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerHandler : MonoBehaviour
{
    //Components
    TopDownCarController topDownCarController;
    Text text;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        //Get the top down car controller
        topDownCarController = GetComponentInParent<Transform>().parent?.GetComponentInParent<TopDownCarController>();

        //Get the speedometer
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "MPH: " + (topDownCarController.GetMPH()).ToString();
    }
}
