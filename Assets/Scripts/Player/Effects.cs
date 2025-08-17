using UnityEngine;

public class Effects: MonoBehaviour
{
    private MaterialFader MaterialFader { get; set; } = new MaterialFader();

    [SerializeField]
    private Material material;

    private void Awake()
    {
        MaterialFader.FadeFloat(new MaterialPropertyUpdate<float>()
        {
            Material = material,
            PropertyName = "_FadeIn",
            Value = 1.0f
        }, 0.1f, 1);
    }
}