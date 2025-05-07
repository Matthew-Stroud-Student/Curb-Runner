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
    ParticleSystem.MainModule particleSystemMainModule;

    void Awake()
    {
        //Get the top down car controller
        topDownCarController = GetComponentInParent<TopDownCarController>();

        //Get the particle system
        particleSystemSmoke = GetComponent<ParticleSystem>();

        //Get the emission component
        particleSystemEmissionModule = particleSystemSmoke.emission;

        //Get the main module
        particleSystemMainModule = particleSystemSmoke.main;

        //Set it to zero emission
        particleSystemEmissionModule.rateOverTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Reduce the particles over time.
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        //Check what surface we are driving and apple different settings
        switch (topDownCarController.GetSurface())
        {
            case Surface.SurfaceTypes.Road:
                particleSystemMainModule.startColor = new Color(0.83f, 0.83f, 0.83f);
                break;

            case Surface.SurfaceTypes.Dirt:
                particleEmissionRate = topDownCarController.GetVelocityMagnitude() * 5;
                particleSystemMainModule.startColor = new Color(0.64f, 0.42f, 0.24f);
                break;

            case Surface.SurfaceTypes.Grass:
                particleEmissionRate = topDownCarController.GetVelocityMagnitude() * 5;
                particleSystemMainModule.startColor = new Color(0.15f, 0.4f, 0.13f);
                break;

            case Surface.SurfaceTypes.Gravel:
                particleEmissionRate = topDownCarController.GetVelocityMagnitude() * 5;
                particleSystemMainModule.startColor = new Color(0.64f, 0.42f, 0.24f);
                break;
        }

        if (topDownCarController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
        {
            //If the car tires are screeching then we'll emitt smoke. If the player is braking then emitt a lot of smoke.
            if (isBraking)
                particleEmissionRate = 30;
            //If the player is drifting we'll emitt smoke based on how much the player is drifting.
            else particleEmissionRate = Mathf.Abs(lateralVelocity) * 10;
        }

    }
}
