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
    private bool r, g, b;
    void Start()
    {
        r = true;
        g = false;
        b = false;
        _PanelImage = GetComponent<Image>();
        _Color.a = 255/255.0f;
        //StartCoroutine(GenerateColor( _PanelImage));
    }

    // Update is called once per frame
    void Update()
    {

        ColorIncrement(ref r, ref g, ref b);

    }

    public void ColorIncrement(ref bool r,ref bool g,ref bool b)
    {
        if(r)
        {
            if (_Color.r >= 1.0f)
            {
                Initialize();

                r = false;
                g = true;
                b = false;
              

            }

            _Color.r+= 0.001f;
            _Color.g += 0.0006f;
            _Color.b += 0.0009f;

        }

        if (g)
        {
            if (_Color.g >= 1.0f)
            {
                Initialize();
                r = false;
                g = false;
                b = true;

            }

            _Color.g += 0.001f;
        }
        if (b)
        {
            if (_Color.b >= 1.0f)
            {
                Initialize();
                r = true;
                b = false;
                g = false;

            }

            _Color.b += 0.001f;
        }
        _PanelImage.color = new Color(_Color.r, _Color.g, _Color.b, _Color.a);

    }

    public void Initialize()
    {
        _Color.r = 0.0f;
        _Color.g = 0.0f;
        _Color.b = 0.0f;
    }

}
