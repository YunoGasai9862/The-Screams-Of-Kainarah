using System.Collections;
using System.Threading;
using UnityEngine.Rendering.Universal;

public interface ICustomLightPreprocessing
{
    public IEnumerator ExecuteLightningLogic(LightPackage lightSource, ILightPreprocess customLightPreprocessingImplementation, CancellationToken cancellationToken);

}