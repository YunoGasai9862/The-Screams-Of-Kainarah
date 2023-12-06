using TMPro;
using UnityEngine;
public class IncreaseDiamond : MonoBehaviour
{
    private TextMeshProUGUI _diamondText;
    public static int count = 0;
    void Start()
    {
        _diamondText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveCrystal.IncreaseValue)
        {
            _diamondText.text = IncreaseCount().ToString("0");

            MoveCrystal.IncreaseValue = false;
        }
    }

    int IncreaseCount()
    {
        return ++count;
    }
}
