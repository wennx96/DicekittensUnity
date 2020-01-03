using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CombatMap : MonoBehaviour
{
    private GameObject GridPlane;

    private Vector2 GridSize;

    // Start is called before the first frame update
    void Start()
    {
        GridPlane = transform.Find("GridPlane").gameObject;
        GridPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2(8, 10);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadMap()
    {

    }
}
