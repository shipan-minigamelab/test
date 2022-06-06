using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShadowText : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI mainTextMeshPro;
    public TextMeshProUGUI  shadowTextMesh;
    
    public  void SetText(string text)
    {
        mainTextMeshPro.SetText(text);
        shadowTextMesh.SetText(text);
    }

    public void SetColor(Color color)
    {
        mainTextMeshPro.color = color;
    }
}
