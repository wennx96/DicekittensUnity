using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : Asset
{

    Texture2D Thrumbnail;
    Vector2 GridSize;
    Vector2 GridOffset;
    bool ShowGrid;
    bool ForceEditing;

    public Map(string path, string name) : base(path, name)
    {

    }

}
