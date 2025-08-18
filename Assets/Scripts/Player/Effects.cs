using UnityEngine;

public class Effects: MonoBehaviour
{
    private MaterialFader MaterialFader { get; set; } = new MaterialFader();

    [SerializeField]
    private Renderer materialRenderer;

    private void Awake()
    {
        MaterialFader.FadeFloat(new MaterialPropertyUpdate<float>()
        {
            Material = GetComponent<Renderer>().sharedMaterial,
            PropertyName = "_FadeIn",
            Value = 1.0f
        }, 0.1f, 1);
    }
}