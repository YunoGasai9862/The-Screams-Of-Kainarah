using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class RunAsyncCoroutine : MonoBehaviour //attach it to the a GameObject
{
    private static RunAsyncCoroutine instance;

    private readonly Queue<Func<IAsyncEnumerator<WaitForSeconds>>> asyncEnumeratorCollection = new();
    private void Awake()
    {
        instance = this;
    }

    public static void RunTheAsyncCoroutine(Func<IAsyncEnumerator<WaitForSeconds>> asyncEnumerator)
    {
        instance.asyncEnumeratorCollection.Enqueue(asyncEnumerator); //adds it to the Queue
    }
    public static RunAsyncCoroutine RunAsyncCoroutineInstance { get => instance; set => instance = value; } //getter + setter
    public static void StartAsyncCoroutine(IAsyncEnumerator<WaitForSeconds> asyncCoroutine)
    {


    }

    public static void AttachToGameObject()
    {
        var coroutineRunner = new GameObject("AsyncCoroutineRunner").AddComponent<RunAsyncCoroutine>();
        //run the script!
    }




}