using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Pawn : MonoBehaviour
{
    private GameObject Cardboard;
    private GameObject Base;
    private GameObject PositionIndicator;

    public string Label = "";
    public bool ProjectLabel = true;
    private GameObject LabelContainer;
    private GameObject LabelAnchor;
    private TextMeshProUGUI LabelMesh;

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
            Base.transform.position += _lifted ? Vector3.up : -Vector3.up;
            Cardboard.transform.position += _lifted ? Vector3.up : -Vector3.up;
            PositionIndicator.SetActive(_lifted);
        }
    }

    public float _scale = 1;
    public float Scale
    {
        get
        {
            return _scale;
        }
        set
        {
            if (_scale == value) return;
            _scale = value;
            transform.localScale = Vector3.one * _scale;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cardboard = transform.Find("Cardboard").gameObject;
        Base = transform.Find("Base").gameObject;
        PositionIndicator = transform.Find("PositionIndicator").gameObject;

        LabelContainer = new GameObject("PawnLabel");
        LabelContainer.transform.SetParent(GameObject.Find("PawnLabelCanvas").transform);
        LabelAnchor = Cardboard.transform.Find("LabelAnchor").gameObject;
        LabelMesh = LabelContainer.AddComponent<TextMeshProUGUI>();

        LabelMesh.alignment = TextAlignmentOptions.Center;
        LabelMesh.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        LabelMesh.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Drop Shadow");
        LabelMesh.fontStyle = FontStyles.Bold;
        LabelMesh.fontSize = 30;
        LabelMesh.SetText(Label);
    }

    // Update is called once per frame
    void Update()
    {
        Cardboard.transform.LookAt(Camera.main.transform);
        Cardboard.transform.rotation = Quaternion.Euler(0, Cardboard.transform.rotation.eulerAngles.y, Cardboard.transform.rotation.eulerAngles.z);
    }

    void OnGUI()
    {
        if (ProjectLabel && Label != "")
        {

            LabelMesh.transform.position = Camera.main.WorldToScreenPoint(LabelAnchor.transform.position);
        }
    }


}
