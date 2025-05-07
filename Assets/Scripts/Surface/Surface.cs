using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    public enum SurfaceTypes { Road, Grass, Gravel, Dirt };

    [Header("Surface")]
    public SurfaceTypes surfaceType;

    // Start is called before the first frame update
    void Start()
    {

    }

}
