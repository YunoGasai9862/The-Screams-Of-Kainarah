using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangeAnimation : MonoBehaviour
{
    private Image _PanelImage;
    private Color _Color;
    [SerializeField] int ColorMin;
    [SerializeField] int ColorMax;
    void Start()
    {
        _PanelImage= GetComponent<Image>();
        _Color.a = 255;
        StartCoroutine(GenerateColor( _PanelImage));
    }

    // Update is called once per frame
    void Update()
    {



    }
    public int GenerateRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }

    IEnumerator GenerateColor( Image _PanelImage)
    {
         yield return new WaitForSecondsRealtime(3f);
        _Color.r = GenerateRandomNumber(ColorMin, ColorMax);
        _Color.g = GenerateRandomNumber(ColorMin, ColorMax);
        _Color.b = GenerateRandomNumber(ColorMin, ColorMax);
        _PanelImage.color = new Color(_Color.r, _Color.g, _Color.b, 255);
        Debug.Log(_Color);
        StartCoroutine(GenerateColor(_PanelImage));
    }
}
