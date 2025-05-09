using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUIHandler : MonoBehaviour
{
    [Header("Car details")]
    public Image carImage;

    //Other components
    Animator animator = null;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupCar(CarData carData)
    {
        carImage.sprite = carData.CarUISprite;
    }

    public void StartCarEntranceAnimation(bool isAppearingOnRightSide)
    {
        if (isAppearingOnRightSide)
            animator.Play("Car UI Appear From Left");
        else animator.Play("Car UI Appear From Left");
    }

    public void StartCarExitAnimation(bool IsExitingOnRightSide)
    {
        if (IsExitingOnRightSide)
            animator.Play("Car UI Disappear To Right");
        else animator.Play("Car UI Disappear To Right");
    }

    //Events
    public void OnCarExitAnimationCompleted()
    {
        Destroy(gameObject);
    }
}
