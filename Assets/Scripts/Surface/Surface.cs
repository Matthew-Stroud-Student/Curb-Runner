using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
    public enum SurfaceTypes { Road, Dirt, Grass, Gravel };

    [Header("Surface")]
    public SurfaceTypes surfaceType;

    // Start is called before the first frame update
    void Start()
    {

    }

}
