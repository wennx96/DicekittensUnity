using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    private GameObject Cardboard;
    private GameObject Base;
    private GameObject PositionIndicator;

    private bool _lifted = false;
    public bool Lifted
    {
        get
        {
            return _lifted;
        }
        set
        {
            if (_lifted == value) return;
            _lifted = value;
            if (_lifted)
            {
                Base.transform.position = new Vector3(Base.transform.position.x, Base.transform.position.y + 1, Base.transform.position.z);
                Cardboard.transform.position = new Vector3(Cardboard.transform.position.x, Cardboard.transform.position.y + 1, Cardboard.transform.position.z);
                PositionIndicator.SetActive(true);
            }
            else
            {
                Base.transform.position = new Vector3(Base.transform.position.x, Base.transform.position.y - 1, Base.transform.position.z);
                Cardboard.transform.position = new Vector3(Cardboard.transform.position.x, Cardboard.transform.position.y - 1, Cardboard.transform.position.z);
                PositionIndicator.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cardboard = transform.Find("Cardboard").gameObject;
        Base = transform.Find("Base").gameObject;
        PositionIndicator = transform.Find("PositionIndicator").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
