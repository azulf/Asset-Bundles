using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Downloader : MonoBehaviour
{
    [SerializeField] private string _sceneReference;
    private AsyncOperationHandle<SceneInstance> _loadHandle;

    private IEnumerator Start()
    {
		string remoteCatalogUrl = "https://azulf.github.io/Asset-Bundles/mewarnai/StandaloneWindows64/catalog_1.0.json";
		var handle = Addressables.LoadContentCatalogAsync(remoteCatalogUrl);
		yield return handle;

		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			Debug.Log("✅ Remote catalog loaded");
			yield return CheckAndLoadScene(_sceneReference);
		}
		else
		{
			Debug.LogError("❌ Gagal load remote catalog");
		}
		cekLogKey();
    }
	

	private void cekLogKey()
	{
		foreach (var locator in Addressables.ResourceLocators)
        {
            Debug.Log($"[Locator] {locator.LocatorId}");

            // Ambil semua key dari locator ini
            foreach (var key in locator.Keys)
            {
                Debug.Log($" - Key: {key}");
            }
        }
	}
	

    private IEnumerator CheckAndLoadScene(string sceneReference)
    {
        // Cek apakah ada yang perlu didownload
        var sizeHandle = Addressables.GetDownloadSizeAsync(sceneReference);
        yield return sizeHandle;

        if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
        {
            if (sizeHandle.Result > 0)
            {
                Debug.Log("Scene belum ada di cache. Downloading...");
                var downloadHandle = Addressables.DownloadDependenciesAsync(sceneReference);
                yield return downloadHandle;

                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Download selesai. Loading scene...");
                    yield return LoadScene(sceneReference);
                }
                else
                {
                    Debug.LogError("Gagal download scene!");
                }
            }
            else
            {
                Debug.Log("Scene sudah ada di cache. Loading langsung...");
				cekLogKey();
                yield return LoadScene(sceneReference);
            }
        }
        else
        {
            Debug.LogError("Gagal cek download size!");
        }
    }

    private IEnumerator LoadScene(string sceneReference)
    {
        _loadHandle = Addressables.LoadSceneAsync(sceneReference, LoadSceneMode.Single);
        yield return _loadHandle;
		
    }
}
