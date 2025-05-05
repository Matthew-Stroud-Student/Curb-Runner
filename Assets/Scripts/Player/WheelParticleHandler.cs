using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelParticleHandler : MonoBehaviour
{
    //Local varialbes
    float particleEmissionRate = 0;

    //Components
    TopDownCarController topDownCarController;

    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmissionModule;

    void Awake()
    {
        //Get the top down car controller
        topDownCarController = GetComponentInParent<TopDownCarController>();

        //Get the particle system
        particleSystemSmoke = GetComponent<ParticleSystem>();

        //Get the emission component
        particleSystemEmissionModule = particleSystemSmoke.emission;

        //Set it to zero emission
        particleSystemEmissionModule.rateOverTime = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Redice the particles over time.
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if (topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            //If the car tires are screeching then we'll emitt smoke. If the player is braking then emitt a lot of smoke.
            if (isBraking)
                particleEmissionRate = 50;
            //If the player is drifting we'll emitt smoke based on how much the player is drifting.
            else particleEmissionRate = Mathf.Abs(lateralVelocity) * 30;
        }

    }
}
