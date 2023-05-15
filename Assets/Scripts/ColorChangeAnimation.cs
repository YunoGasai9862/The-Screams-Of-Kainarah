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
        _Color.a = 100/255.0f;
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
            if (CheckCondition(_Color.r, .1f))
            {
                Initialize(0f, 0f, 0f);
                ChangeBoolValues(false, true, false);



            }

            _Color.r+= 0.001f;

        }

        if (g)
        {
            if (CheckCondition(_Color.g, .1f))
            {
                Initialize(0f, 0f,0f);
                ChangeBoolValues(false, false, true);

            }

            _Color.g += 0.001f;
        }
        if (b)
        {
            if (CheckCondition(_Color.b, .1f))
            {
                Initialize(0,0,0);
                ChangeBoolValues(true, false, false);

            }

            _Color.b += 0.001f;
        }
        _PanelImage.color = new Color(_Color.r, _Color.g, _Color.b, _Color.a);

    }

    public void ChangeBoolValues(bool r,  bool g,  bool b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    public void Initialize(float r, float g, float b)
    {
        this._Color.r = r;
        this._Color.g = g;
        this._Color.b = b;
    }

    public bool CheckCondition(float value, float checkAgainst)
    {
        return value >= checkAgainst;
    }
}
