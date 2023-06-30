using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillWares : MonoBehaviour
{
    [Header("Fill Wares with Items")]
    [SerializeField] List<GameObject> wareObjects;
    private List<GameObject> freeslots;
    private int wareCounter = 0;
    [SerializeField] GenerateBoxes _boxes;
    private float scaleSize = .9f;

    [Header("Position of the Grid")]

    [SerializeField] int StartX;
    [SerializeField] int StartY;
    [SerializeField] int IncrementX;
    [SerializeField] int IncrementY;

    void Start()
    {
        _boxes.GenerateInventory(3, StartX, StartY, IncrementX, IncrementY, gameObject, "ClickFeedbackOnItem");
        freeslots = new List<GameObject>();
        StartCoroutine(FillSlots(gameObject));
        StartCoroutine(FillUpWares());
    }

    IEnumerator FillUpWares()
    {
        yield return new WaitForSeconds(.7f);
        for (int i = 0; i < freeslots.Count; i++)
        {
            if (Possible(i))
            {
                Sprite _sprite = wareObjects[wareCounter].GetComponent<Image>().sprite;
                AssignTagsandSprite(freeslots[i], scaleSize, _sprite, wareObjects[wareCounter].tag);
                wareCounter++;
            }
        }

    }

    IEnumerator FillSlots(GameObject panel)
    {

        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            freeslots.Add(panel.transform.GetChild(i).gameObject);
        }

    }

    public bool Possible(int index)
    {
        return wareObjects.Count != wareCounter && freeslots[index].transform.childCount == 0;
    }

    public GameObject AssignTagsandSprite(GameObject _freeslot, float scaleSize, Sprite sprite, string name)
    {
        GameObject temp = new GameObject(name);

        temp.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        temp.transform.tag = name;
        temp.AddComponent<Image>();
        temp.GetComponent<Image>().sprite = sprite;
        temp.transform.SetParent(_freeslot.transform, false);
        return temp;
    }
}
