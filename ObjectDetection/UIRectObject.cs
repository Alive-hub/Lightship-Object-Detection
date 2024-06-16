using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class UIRectObject : MonoBehaviour
{
    private RectTransform _rectangleRectTransform;
    private Image _rectangleImage;
    private TMP_Text _text;

    public void Awake()
    {
        _rectangleRectTransform = GetComponent<RectTransform>();
        _rectangleImage = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void SetRectTransform(Rect rect)
    {
        _rectangleRectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
        _rectangleRectTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }

    public void SetColor(Color color)
    {
        _rectangleImage.color = color;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public RectTransform getRectTransform(){
        return _rectangleRectTransform;
    }
}