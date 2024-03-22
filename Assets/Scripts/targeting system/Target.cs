using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public MainBody targetShip;
    public new RectTransform transform;
    public RectTransform arrow;

    public RectTransform topAnchor, bottomAnchor, distanceDisplayTransform;

    public TMP_Text distanceDisplay;

    [SerializeField]
    public Image _renderer;

    [SerializeField]
    public Sprite _square;
    
    [SerializeField]
    public Sprite _arrow;
    
}
