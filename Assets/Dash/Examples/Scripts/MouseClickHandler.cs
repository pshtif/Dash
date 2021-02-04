
using System.Collections.Generic;
using Dash;
using DG.Tweening;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class MouseClickHandler : MonoBehaviour
{
    public Vector2 width => new Vector2(100,100);
    
    void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        buttons.ForEach(b => b.onClick.AddListener(() => DashCore.Instance.SendEvent("MouseClick", NodeFlowDataFactory.Create(b.transform))));
    }
}