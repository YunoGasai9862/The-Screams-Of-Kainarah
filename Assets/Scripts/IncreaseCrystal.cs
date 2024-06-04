using TMPro;
using UnityEngine;
public class IncreaseCrystal : MonoBehaviour
{
    private TextMeshProUGUI m_diamondText;

    [SerializeField] public CrystalUIIncrementEvent crystalUIIncrementEvent;
    public static int DiamondCount { get; set; } = 0;

    void Start()
    {
        m_diamondText = GetComponent<TextMeshProUGUI>();
        crystalUIIncrementEvent.AddListener(IncrementCrystalCount);
    }
    void IncrementCrystalCount(int increment)
    {
        Debug.Log("INCREMENTING---");
        DiamondCount = int.Parse(m_diamondText.text) + increment;
        Debug.Log(DiamondCount);
        m_diamondText.text = DiamondCount.ToString("0");
    }

}
