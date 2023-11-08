
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickering : MonoBehaviour, IObserverAsync<LightEntity>
{
    private Light2D m_light;

    [Header("Light Intensity Swing Values")]
    public float maxIntensity;
    public float minIntensity;

    [Header("Add the Subject which willl be responsible for notifying")]
    public bool anySubjectThatIsNotifyingTheLight;
    [HideInInspector]
    public LightObserverPattern _subject;

    private LightEntity m_lightEntity;
    private SemaphoreSlim m_Semaphore;

    private void Awake()
    {
        m_light = GetComponent<Light2D>();
        
        if(!RunAsyncCoroutineWaitForSeconds.GetIsAttached)  //if its false
            RunAsyncCoroutineWaitForSeconds.AttachToGameObject(); //make sure this happens first
    }
    private void OnEnable()
    {
        if(anySubjectThatIsNotifyingTheLight)
            _subject.AddObserver(this);
    }

    private void OnDisable()
    {
        if(anySubjectThatIsNotifyingTheLight)
            _subject.RemoveObserver(this);
    }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public virtual async Task OnNotify(LightEntity Data, CancellationToken _cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        m_lightEntity = Data;

        if (m_lightEntity != null)
        {

            if (m_lightEntity.LightName == transform.parent.name && m_lightEntity.canFlicker)
            {
                m_Semaphore= new SemaphoreSlim(0);
    
                RunAsyncCoroutineWaitForSeconds.RunTheAsyncCoroutine(LightFlickerHelper.lightFlicker(m_light, minIntensity, maxIntensity, m_Semaphore, Data.innerRadiusMin, Data.innerRadiusMax, Data.outerRadiusMin, Data.outerRadiusMax) , _cancellationToken); //Async runner

                await m_Semaphore.WaitAsync(); //similar to using a bool variable, initializing it with 0. The thread becomes lock, and released in the helper class function
                //if the value becomes 0, everything else is put on hold, hence initializing it with 0

                //successfully was able to do it! (Async convesion)

                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }
 
            }

            if (m_lightEntity.LightName == transform.parent.name && !m_lightEntity.canFlicker)
            {
                StopAllCoroutines(); //the fix!
            }
        }

    }
}
