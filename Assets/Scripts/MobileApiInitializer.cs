using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class MobileApiInitializer : MonoBehaviour
{
    // Checks if EM has been initialized and initialize it if not.
    // This must be done once before other EM APIs can be used.
    void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
