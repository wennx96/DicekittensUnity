using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public bool LiftingPawnWhenDrag = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != this && Instance != null) Destroy(this);
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
