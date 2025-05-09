using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCarUIHandler : MonoBehaviour
{
    [Header("Car prefab")]
    public GameObject carPrefab;

    [Header("Spawn on")]
    public Transform spawnOnTransform;

    bool isChangingCar = false;

    CarData[] carDatas;

    int selectedCarIndex = 0;

    //Other components
    CarUIHandler carUIHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        //Load the car daya
        carDatas = Resources.LoadAll<CarData>("CarData/");

        StartCoroutine(SpawnCarCO(true));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            OnPreviousCar();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            OnNextCar();
        }
    }

    public void OnPreviousCar()
    {
        if (isChangingCar)
            return;

        selectedCarIndex--;

        if (selectedCarIndex < 0)
            selectedCarIndex = carDatas.Length - 1;

        StartCoroutine(SpawnCarCO(true));
    }

    public void OnNextCar()
    {
        if (isChangingCar)
            return;

        selectedCarIndex++;

        if (selectedCarIndex > carDatas.Length - 1)
            selectedCarIndex = 0;

        StartCoroutine(SpawnCarCO(false));
    }

    IEnumerator SpawnCarCO(bool isCarAppearingOnRightSide)
    {
        isChangingCar = true;

        if (carUIHandler != null)
            carUIHandler.StartCarExitAnimation(!isCarAppearingOnRightSide);

        GameObject instantiatedCar = Instantiate(carPrefab, spawnOnTransform);

        carUIHandler = instantiatedCar.GetComponent<CarUIHandler>();
        carUIHandler.SetupCar(carDatas[selectedCarIndex]);
        carUIHandler.StartCarEntranceAnimation(isCarAppearingOnRightSide);

        yield return new WaitForSeconds(0.2f);

        isChangingCar = false;
    }
}
