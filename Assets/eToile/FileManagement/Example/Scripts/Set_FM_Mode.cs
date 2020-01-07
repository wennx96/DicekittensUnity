using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Set the FileManagement string conversion mode.
 */

public class Set_FM_Mode : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        // This mode allows a very wide char collection:
        FileManagement.stringConversion = FM_StringMode.UTF8;
	}
}
