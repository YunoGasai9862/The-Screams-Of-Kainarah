using System.Collections;
using System.Threading;
using UnityEngine.Rendering.Universal;

public interface ICustomLightPreprocessing
{
    public IEnumerator ExecuteLightningLogic(Light2D lightSource, ILightPreprocess customLightPreprocessingImplementation, LightEntity lightEntity, CancellationToken cancellationToken);

}