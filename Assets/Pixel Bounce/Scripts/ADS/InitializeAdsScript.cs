using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAdsScript : MonoBehaviour
{
    public static InitializeAdsScript instance;
    [SerializeField]
    string gameId = "3338152";
    [SerializeField]
    string placementId = "video";

    public bool testMode = true;

    void Awake()
    {
        if (instance)
            Destroy(this);
        else
            instance = this;
    }
    // Initialize the Ads service:
    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
    }

    public void ShowAds()
    {
        if(Advertisement.IsReady(placementId))
            Advertisement.Show(placementId);
    }

}