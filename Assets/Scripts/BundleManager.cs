using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A wrapper to read the download progress specifically for bundles
/// </summary>
public class BundleProgress
{
    public float bundleProgress = 1;
    public AssetBundle bundle;
    public string error = null;

    public void Unload(bool unloadAll)
    {
        bundle.Unload(unloadAll);
    }
}

/// <summary>
/// A short and simple class to download and read bundles
/// </summary>
public class BundleManager: MonoBehaviour {

    public string domainPath = "http://visionashutosh.com/bundles/";   //Meanwhile I have hosted the asset bundles in this domain
    private static BundleManager instance;

    private IEnumerator bundleCoroutine;        

    private void Start()
    {
        this.SetUPSingleton();
    }

    public void LoadAssetBundle(string assetBundleName , Action<BundleProgress> callback)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        domainPath += "/android/";          //Incase of android update the path by /android
#elif UNITY_STANDALONE || UNITY_EDITOR
        domainPath += "/windows/";          //Incase of android update the path by /windows
#endif
        this.bundleCoroutine = this.GetAssetBundle(assetBundleName, callback);
        StartCoroutine(this.bundleCoroutine);
    }
    
    //Setting up the Singleton
    private void SetUPSingleton()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// A coroutine to download the asset bundle asynchronously
    /// </summary>
    /// <param name="bundleName">Name of the bundle to be donwloaded</param>
    /// <param name="callback">A Lambda function to passon the progress, error and bundle</param>
    /// <returns></returns>
    IEnumerator GetAssetBundle(string bundleName , Action<BundleProgress> callback)
    {
        WWW www = WWW.LoadFromCacheOrDownload(this.domainPath+bundleName , 2);
        BundleProgress bProgess = new BundleProgress();

        while (!www.isDone)
        {
            bProgess.bundleProgress = www.progress * 100;
            callback(bProgess);
            Debug.Log(string.Format("Progress - {0}%. from {1}", www.progress * 100, www.url));
            yield return new WaitForSeconds(0.1f);
        }

        yield return www;
        if (www.error != null)
        {
            Debug.Log(www.error);
            bProgess.error = www.error;
            callback(bProgess);
        }
        else
        {   
            bProgess.bundle = www.assetBundle;
            callback(bProgess);
        }
    }

    public static BundleManager Instance
    {
        get { return BundleManager.instance; }
    }
}
