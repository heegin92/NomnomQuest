using Ironcow.Synapse.Data;
using System;
using UnityEngine;

[System.Serializable]
public partial class MapData : BaseDataSO
{
    public string target;
    public string displayName;
    public int count;
    public bool isRandom;
    public Vector3 pos;
}