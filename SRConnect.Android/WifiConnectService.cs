﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using SRConnect.Droid;
using SRConnect.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(WifiConnectService))]
namespace SRConnect.Droid
{

    public class WifiConnectService : IWifiConnect
    {
        public async void ConnectToWifi(string ssid, string password)
        {
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
            {
                var formattedSsid = $"\"{ssid}\"";
                var formattedPassword = $"\"{password}\"";

                var wifiConfig = new WifiConfiguration
                {
                    Ssid = formattedSsid,
                    PreSharedKey = formattedPassword
                };

                var wifiManager = (WifiManager)Android.App.Application.Context
                        .GetSystemService(Context.WifiService);

                var addNetwork = wifiManager.AddNetwork(wifiConfig);
                var network = new WifiConfiguration();

                IList<Android.Net.Wifi.WifiConfiguration> networks = wifiManager.ConfiguredNetworks.ToList<WifiConfiguration>();
                foreach (var n in networks)
                {
                    Console.WriteLine($"Config Networks: {n.Bssid}");
                    if (n.Ssid == formattedSsid)
                    {
                        network = n;
                    }
                }

                if (network == null)
                {
                    Console.WriteLine($"Cannot connect to network: {ssid}");
                    return;
                }

                wifiManager.Disconnect();
                var enableNetwork = wifiManager.EnableNetwork(network.NetworkId, true);
                wifiManager.Reconnect();

                await Application.Current.MainPage.DisplayAlert("Failed to connect", ssid, "OK");
            }
            else
            {
                WifiNetworkSpecifier.Builder builder = new WifiNetworkSpecifier.Builder();
                WifiNetworkSpecifier networkSpecifier = builder
                                                            .SetSsid(ssid)
                                                            .SetWpa2Passphrase(password)
                                                            .Build();

                NetworkRequest.Builder networkRequest = new NetworkRequest.Builder();
                networkRequest
                        .SetNetworkSpecifier(networkSpecifier)
                        .Build();

                ConnectivityManager connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);

                NetworkCallback networkCallback = new NetworkCallback(connectivityManager);
            }
        }



        private class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            private ConnectivityManager connectivityManager;
            public Action<Network> NetworkAvailable { get; set; }
            public Action NetworkUnavailable { get; set; }

            public NetworkCallback(ConnectivityManager _connectivityManager)
            {
                connectivityManager = _connectivityManager;
            }

            public override void OnAvailable(Network network)
            {
                base.OnAvailable(network);
                connectivityManager.BindProcessToNetwork(network);
                NetworkAvailable?.Invoke(network);
            }

            public override void OnUnavailable()
            {
                base.OnUnavailable();

                NetworkUnavailable?.Invoke();
            }
        }
    }
}

