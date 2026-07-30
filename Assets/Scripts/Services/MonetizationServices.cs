using System;
using UnityEngine;
namespace PhysicsMaster.Services {
    public interface IAdsService { bool IsRewardedReady { get; } void ShowRewarded(string placement, Action reward); void ShowInterstitial(string placement); }
    public interface IIapService { void Buy(string productId); void Restore(); }
    public sealed class DevelopmentMonetization : IAdsService, IIapService {
        public bool IsRewardedReady => true;
        public void ShowRewarded(string placement, Action reward) { Debug.Log("Development rewarded: " + placement); reward?.Invoke(); }
        public void ShowInterstitial(string placement) { Debug.Log("Development interstitial, menu-only: " + placement); }
        public void Buy(string productId) { Debug.Log("Configure Google Play Billing product: " + productId); }
        public void Restore() { Debug.Log("Restore purchases requested"); }
    }
}
