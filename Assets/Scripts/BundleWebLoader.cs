using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BundleWebLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _logs;

    public void GetObjectBundle(string objectName, UnityAction<GameObject> callback) => StartCoroutine(LoadOrDownloadObjects(objectName, callback));

    IEnumerator LoadOrDownloadObjects(string objectName, UnityAction<GameObject> callback)
    {
        _logs.text = "Retrieving assets from: " + ModalController.customAssetLink;
        Debug.Log("Retrieving assets from: " + ModalController.customAssetLink);

        while (!Caching.ready)
            yield return null;

        using WWW www = WWW.LoadFromCacheOrDownload(ModalController.customAssetLink, 1);
        yield return www;

        if (www.error != null)
        {
            _logs.text = "Fetch Failure: " + www.error;
            Debug.Log("Fetch Failure: " + www.error);
        }

        AssetBundle bundle = www.assetBundle;
        GameObject furnitureObj = bundle.LoadAsset<GameObject>(objectName);

        bundle.Unload(false);
        callback(furnitureObj);
    }
}