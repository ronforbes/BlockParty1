using System.Collections;
using UnityEngine;

public class Ad : MonoBehaviour
{
    ADBannerView banner;

    void Start()
    {
        banner = new ADBannerView(ADBannerView.Type.Banner, ADBannerView.Layout.Bottom);
        ADBannerView.onBannerWasLoaded += OnBannerLoaded;
        ADBannerView.onBannerWasClicked += OnBannerClicked;
    }

    void OnBannerLoaded()
    {
        banner.visible = true;
    }

    void OnBannerClicked()
    {

    }
}
