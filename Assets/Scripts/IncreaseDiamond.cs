using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IncreaseDiamond : MonoBehaviour
{
    private TextMeshProUGUI _diamondText;
    public static  int count = 0;
    void Start()
    {
        _diamondText= GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(MoveCrystal.increaseValue)
        {
            _diamondText.text = IncreaseCount().ToString("0");
          
            MoveCrystal.increaseValue = false;
        }
    }

    int IncreaseCount()
    {
        count++;
         return count;
    }
}
