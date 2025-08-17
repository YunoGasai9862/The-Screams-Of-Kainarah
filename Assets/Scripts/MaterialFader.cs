using System.Collections;
using UnityEngine;

public class MaterialFader
{
    public IEnumerator FadeFloat(MaterialPropertyUpdate<float> materialPropertyUpdate, float increment, float waitForInSeconds)
    {
        Material material = materialPropertyUpdate.Material;

        float currentPropertyValue = material.GetFloat(materialPropertyUpdate.PropertyName);

        while (currentPropertyValue < materialPropertyUpdate.Value)
        {
            currentPropertyValue += increment;

            material.SetFloat(materialPropertyUpdate.PropertyName, currentPropertyValue);

            yield return new WaitForSeconds(waitForInSeconds);
        }
    }

    public void MaterialFloatPropertyReset(Material material, string propertyName, float resetTo)
    {
        material.SetFloat(propertyName, resetTo);
    }
}
