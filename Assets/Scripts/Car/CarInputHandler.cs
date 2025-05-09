using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    public int playerNumber = 1;

    //Components
    TopDownCarController topDownCarController;

    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();    
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        switch (playerNumber)
        {
            case 1:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P1");
                inputVector.y = Input.GetAxis("Vertical_P1");
                break;

            case 2:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P2");
                inputVector.y = Input.GetAxis("Vertical_P2");
                break;

            case 3:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P3");
                inputVector.y = Input.GetAxis("Vertical_P3");
                break;

            case 4:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P4");
                inputVector.y = Input.GetAxis("Vertical_P4");
                break;

            case 5:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P5");
                inputVector.y = Input.GetAxis("Vertical_P5");
                break;

            case 6:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P6");
                inputVector.y = Input.GetAxis("Vertical_P6");
                break;

            case 7:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P7");
                inputVector.y = Input.GetAxis("Vertical_P7");
                break;

            case 8:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P8");
                inputVector.y = Input.GetAxis("Vertical_P8");
                break;

            case 9:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P9");
                inputVector.y = Input.GetAxis("Vertical_P9");
                break;

            case 10:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P10");
                inputVector.y = Input.GetAxis("Vertical_P10");
                break;

            case 11:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P11");
                inputVector.y = Input.GetAxis("Vertical_P11");
                break;

            case 12:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P12");
                inputVector.y = Input.GetAxis("Vertical_P12");
                break;

            case 13:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P13");
                inputVector.y = Input.GetAxis("Vertical_P13");
                break;

            case 14:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P14");
                inputVector.y = Input.GetAxis("Vertical_P14");
                break;

            case 15:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P15");
                inputVector.y = Input.GetAxis("Vertical_P15");
                break;

            case 16:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P16");
                inputVector.y = Input.GetAxis("Vertical_P16");
                break;

            case 17:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P17");
                inputVector.y = Input.GetAxis("Vertical_P17");
                break;

            case 18:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P18");
                inputVector.y = Input.GetAxis("Vertical_P18");
                break;

            case 19:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P19");
                inputVector.y = Input.GetAxis("Vertical_P19");
                break;

            case 20:
                //Get Input from Unity's Input system.
                inputVector.x = Input.GetAxis("Horizontal_P20");
                inputVector.y = Input.GetAxis("Vertical_P20");
                break;
        }

        //Send the input to the car controller
        topDownCarController.SetInputVector(inputVector);
    }
}
