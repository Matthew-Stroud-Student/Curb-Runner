using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerHandler : MonoBehaviour
{
    //Components
    TopDownCarController topDownCarController;
    public Text speedometerText;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        //Get the top down car controller
        topDownCarController = GetComponent<TopDownCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        speedometerText.text = "MPH: " + (topDownCarController.GetMPH()).ToString();
    }
}
