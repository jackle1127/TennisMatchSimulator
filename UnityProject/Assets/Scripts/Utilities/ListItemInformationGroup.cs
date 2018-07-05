using System;
using UnityEngine.Events;

[Serializable]
public class ListItemInformationGroup
{
    public string label;
    public UnityEvent onClick = new UnityEvent();
}