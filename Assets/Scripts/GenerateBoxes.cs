using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class GenerateBoxes : MonoBehaviour
{
    // Start is called before the first frame update
    private int _count = 0;
    [SerializeField] GameObject InventoryBox;



    public void GenerateInventory(int _Size, int _startX, int _startY, int _increment, int _decrement, ref Queue<GameObject> inventoryList, ref GameObject PanelObject, string ScriptTobeAddedForItems, string slotTag)
    {
        int increment = _startX;
        int decrement = _startY;

        for (int i = 0; i < _Size; i++)
        {
            for (int j = 0; j < _Size; j++)
            {
                Vector3 IncrementalSize = new(increment, decrement);
                GameObject _temp = Instantiate(InventoryBox, IncrementalSize, Quaternion.identity);
                if (ScriptTobeAddedForItems != "")
                {
                    try
                    {
                        var scriptInstance = _temp.AddComponent(Type.GetType(ScriptTobeAddedForItems)) as ISerializableFeildsHelper; //adds the script (cast it to ISerializableFieldsHelper)

                        if (scriptInstance == null)
                        {
                            throw new ExceptionList.NullException("The above instance lacks the correct implementation of the interface");
                        }

                        scriptInstance.FieldName = slotTag; //adds the field tag

                    }
                    catch (ExceptionList.NullException ex)
                    {
                        Debug.LogError(ex.ExceptionMessage);
                    }

                }
                _temp.name = (_count).ToString("0");
                _temp.tag = slotTag;
                inventoryList.Enqueue(_temp);
                _temp.transform.SetParent(PanelObject.transform, false);
                increment += _increment;
                _count++;

            }
            decrement -= _decrement;
            increment = _startX;
        }

        _count = 0;

    }

    public void GenerateInventory(int _Size, int _startX, int _startY, int _increment, int _decrement, GameObject PanelObject, string ScriptTobeAddedForItems)
    {
        int increment = _startX;
        int decrement = _startY;

        for (int i = 0; i < _Size; i++)
        {
            for (int j = 0; j < _Size; j++)
            {
                Vector3 IncrementalSize = new(increment, decrement);
                GameObject _temp = Instantiate(InventoryBox, IncrementalSize, Quaternion.identity);
                if (ScriptTobeAddedForItems != "")
                    _temp.AddComponent(Type.GetType(ScriptTobeAddedForItems)); //adds the script
                _temp.name = (_count).ToString("0");

                _temp.transform.SetParent(PanelObject.transform, false);
                increment += _increment;
                _count++;

            }
            decrement -= _decrement;
            increment = _startX;
        }

        _count = 0;



    }

}
