using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FillWares : MonoBehaviour
{
    [Header("Fill Wares with Items")]
    [SerializeField] List<GameObject> wareObjects;
    private List<GameObject> freeslots;
    private int wareCounter = 0;
    [SerializeField] GameObject panel;
    void Start()
    {
        freeslots=new List<GameObject>();
        StartCoroutine(FillSlots(panel));
        StartCoroutine(FillUpWares());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FillUpWares()
    {
        yield return new WaitForSeconds(1);
        for (int i=0; i<freeslots.Count; i++)
        {
            if (Possible(i))
            {
                GameObject _temp = new GameObject(wareObjects[wareCounter].tag);
                Sprite _sprite = wareObjects[wareCounter].GetComponent<Image>().sprite;
                _temp.AddComponent<Image>();
                _temp.GetComponent<Image>().sprite = _sprite;
                wareCounter++;
                _temp.transform.SetParent(freeslots[i].transform, false);
            }
        }

    }

    IEnumerator  FillSlots(GameObject panel)
    {
        yield return new WaitForSeconds(.5f);
        for(int i=0; i<panel.transform.childCount; i++)
        {
            freeslots.Add(panel.transform.GetChild(i).gameObject);
        }
       
    }

    public bool Possible(int index)
    {
        return wareObjects.Count!= wareCounter && freeslots[index].transform.childCount==0;
    }
}
