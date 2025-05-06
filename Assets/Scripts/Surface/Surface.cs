using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    public enum SurfaceTypes { Road, Dirt, Gravel, Grass };

    [Header("Surface")]
    public SurfaceTypes surfaceType;

    // Start is called before the first frame update
    void Start()
    {

    }

}
