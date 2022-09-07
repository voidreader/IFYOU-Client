using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.EssentialKit
{
    internal static class ImplementationBlueprint
    {
        #region Constants

        private     const   string      kMainAssembly                   = "VoxelBusters.EssentialKit";
        
        private     const   string      kIOSAssembly                    = "VoxelBusters.EssentialKit.iOSModule";

        private     const   string      kAndroidAssembly                = "VoxelBusters.EssentialKit.AndroidModule";

        private     const   string      kSimulatorAssembly              = "VoxelBusters.EssentialKit.SimulatorModule";

        private     const   string      kRootNamespace                  = "VoxelBusters.EssentialKit";

        private     const   string      kAddressBookNamespace           = kRootNamespace + ".AddressBookCore";

        private     const   string      kBillingServicesNamespace       = kRootNamespace + ".BillingServicesCore";

        private     const   string      kCloudServicesNamespace         = kRootNamespace + ".CloudServicesCore";

        private     const   string      kGameServicesNamespace          = kRootNamespace + ".GameServicesCore";

        private     const   string      kMediaServicesNamespace         = kRootNamespace + ".MediaServicesCore";

        private     const   string      kNativeUINamespace              = kRootNamespace + ".NativeUICore";

        private     const   string      kNetworkServicesNamespace       = kRootNamespace + ".NetworkServicesCore";

        private     const   string      kNotificationServicesNamespace  = kRootNamespace + ".NotificationServicesCore";

        private     const   string      kSharingServicesNamespace       = kRootNamespace + ".SharingServicesCore";

        private     const   string      kWebViewNamespace               = kRootNamespace + ".WebViewCore";

        private     const   string      kExtrasNamespace                = kRootNamespace + ".ExtrasCore";

        private     const   string      kDeepLinkServicesNamespace      = kRootNamespace + ".DeepLinkServicesCore";

        #endregion

        #region Static properties

        public static NativeFeatureRuntimeConfiguration AddressBook 
        { 
            get { return GetAddressBookConfig(); } 
        }

        public static NativeFeatureRuntimeConfiguration BillingServices 
        { 
            get { return GetBillingServicesConfig(); } 
        }

        public static NativeFeatureRuntimeConfiguration CloudServices 
        { 
            get { return GetCloudServicesConfig(); } 
        }
        
        public static NativeFeatureRuntimeConfiguration GameServices
        {
            get { return GetGameServicesConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration MediaServices
        {
            get { return GetMediaServicesConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration NativeUI
        {
            get { return GetNativeUIConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration NetworkServices
        {
            get { return GetNetworkServicesConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration NotificationServices
        {
            get { return GetNotificationServicesConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration SharingServices
        {
            get { return GetSharingServicesConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration WebView
        {
            get { return GetWebViewConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration Extras
        {
            get { return GetExtrasConfig(); }
        }
        
        public static NativeFeatureRuntimeConfiguration DeepLinkServices
        {
            get { return GetDeepLinkServicesConfig(); }
        }
        
        #endregion

        #region Constructors

        private static NativeFeatureRuntimeConfiguration GetAddressBookConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kAddressBookNamespace + ".iOS", nativeInterfaceType: "AddressBookInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kAddressBookNamespace + ".iOS", nativeInterfaceType: "AddressBookInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kAddressBookNamespace + ".Android", nativeInterfaceType: "AddressBookInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kAddressBookNamespace + ".Simulator", nativeInterfaceType: "AddressBookInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kAddressBookNamespace, nativeInterfaceType: "NullAddressBookInterface"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetBillingServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kBillingServicesNamespace + ".iOS", nativeInterfaceType: "BillingServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kBillingServicesNamespace + ".iOS", nativeInterfaceType: "BillingServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kBillingServicesNamespace + ".Android", nativeInterfaceType: "BillingServicesInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kBillingServicesNamespace + ".Simulator", nativeInterfaceType: "BillingServicesInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kBillingServicesNamespace, nativeInterfaceType: "NullBillingServicesInterface"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetCloudServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kCloudServicesNamespace + ".iOS", nativeInterfaceType: "CloudServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kCloudServicesNamespace + ".iOS", nativeInterfaceType: "CloudServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kCloudServicesNamespace + ".Android", nativeInterfaceType: "CloudServicesInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kCloudServicesNamespace + ".Simulator", nativeInterfaceType: "CloudServicesInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kCloudServicesNamespace, nativeInterfaceType: "NullCloudServicesInterface"));
        }

        private static NativeFeatureRuntimeConfiguration GetGameServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kGameServicesNamespace + ".iOS", nativeInterfaceType: "GameCenterInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kGameServicesNamespace + ".iOS", nativeInterfaceType: "GameCenterInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kGameServicesNamespace + ".Android", nativeInterfaceType: "GameServicesInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kGameServicesNamespace + ".Simulator", nativeInterfaceType: "GameServicesInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kGameServicesNamespace, nativeInterfaceType: "NullGameServicesInterface"));
        }

        private static NativeFeatureRuntimeConfiguration GetMediaServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kMediaServicesNamespace + ".iOS", nativeInterfaceType: "MediaServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kMediaServicesNamespace + ".iOS", nativeInterfaceType: "MediaServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kMediaServicesNamespace + ".Android", nativeInterfaceType: "MediaServicesInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kMediaServicesNamespace + ".Simulator", nativeInterfaceType: "MediaServicesInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kMediaServicesNamespace, nativeInterfaceType: "NullMediaServicesInterface"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetNativeUIConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kNativeUINamespace + ".iOS", nativeInterfaceType: "NativeUIInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kNativeUINamespace + ".iOS", nativeInterfaceType: "NativeUIInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly,ns: kNativeUINamespace + ".Android", nativeInterfaceType: "UIInterface"),
                },
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kNativeUINamespace, nativeInterfaceType: "UnityUIInterface"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetNetworkServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kNetworkServicesNamespace + ".iOS", nativeInterfaceType: "NetworkServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kNetworkServicesNamespace + ".iOS", nativeInterfaceType: "NetworkServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kNetworkServicesNamespace + ".Android", nativeInterfaceType: "NetworkServicesInterface"),
                },
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kNetworkServicesNamespace, nativeInterfaceType: "UnityNetworkServicesInterface"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetNotificationServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kNotificationServicesNamespace + ".iOS", nativeInterfaceType: "NotificationCenterInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kNotificationServicesNamespace + ".iOS", nativeInterfaceType: "NotificationCenterInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kNotificationServicesNamespace + ".Android", nativeInterfaceType: "NotificationCenterInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kNotificationServicesNamespace + ".Simulator", nativeInterfaceType: "NotificationCenterInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kNotificationServicesNamespace, nativeInterfaceType: "NullNotificationCenterInterface"));
        }

        private static NativeFeatureRuntimeConfiguration GetSharingServicesConfig()
        {    
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kSharingServicesNamespace + ".iOS", nativeInterfaceType: "NativeSharingInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kSharingServicesNamespace + ".iOS", nativeInterfaceType: "NativeSharingInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kSharingServicesNamespace + ".Android", nativeInterfaceType: "SharingServicesInterface"),
                },
                simulatorPackage: new NativeFeatureRuntimePackage(assembly: kSimulatorAssembly, ns: kSharingServicesNamespace + ".Simulator", nativeInterfaceType: "NativeSharingInterface"),
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kSharingServicesNamespace, nativeInterfaceType: "NullSharingInterface"));
        }

        private static NativeFeatureRuntimeConfiguration GetWebViewConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kWebViewNamespace + ".iOS", nativeInterfaceType: "NativeWebView"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kWebViewNamespace + ".iOS", nativeInterfaceType: "NativeWebView"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kWebViewNamespace + ".Android", nativeInterfaceType: "WebView"),
                },
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kWebViewNamespace, nativeInterfaceType: "NullNativeWebView"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetExtrasConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kExtrasNamespace + ".iOS", nativeInterfaceType: "NativeUtilityInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kExtrasNamespace + ".iOS", nativeInterfaceType: "NativeUtilityInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kExtrasNamespace + ".Android", nativeInterfaceType: "UtilityInterface"),
                },
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kExtrasNamespace, nativeInterfaceType: "NullNativeUtilityInterface"));
        }
            
        private static NativeFeatureRuntimeConfiguration GetDeepLinkServicesConfig()
        {
            return new NativeFeatureRuntimeConfiguration(
                packages: new NativeFeatureRuntimePackage[]
                {
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.IPhonePlayer, assembly: kIOSAssembly, ns: kDeepLinkServicesNamespace + ".iOS", nativeInterfaceType: "DeepLinkServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.tvOS, assembly: kIOSAssembly, ns: kDeepLinkServicesNamespace + ".iOS", nativeInterfaceType: "DeepLinkServicesInterface"),
                    new NativeFeatureRuntimePackage(platform: RuntimePlatform.Android, assembly: kAndroidAssembly, ns: kDeepLinkServicesNamespace + ".Android", nativeInterfaceType: "DeepLinkServicesInterface"),
                },
                fallbackPackage: new NativeFeatureRuntimePackage(assembly: kMainAssembly, ns: kDeepLinkServicesNamespace, nativeInterfaceType: "NullDeepLinkServicesInterface"));
        }
            
        #endregion

        #region Public static methods

        public static NativeFeatureRuntimeConfiguration GetRuntimeConfiguration(string featureName)
        {
            switch (featureName)
            {
                case NativeFeatureType.kAddressBook:
                    return AddressBook;

                case NativeFeatureType.kBillingServices:
                    return BillingServices;

                case NativeFeatureType.kCloudServices:
                    return CloudServices;

                case NativeFeatureType.kExtras:
                    return Extras;

                case NativeFeatureType.kGameServices:
                    return GameServices;

                case NativeFeatureType.kMediaServices:
                    return MediaServices;

                case NativeFeatureType.kNativeUI:
                    return NativeUI;

                case NativeFeatureType.kNetworkServices:
                    return NetworkServices;

                case NativeFeatureType.kNotificationServices:
                    return NotificationServices;

                case NativeFeatureType.KSharingServices:
                    return SharingServices;

                case NativeFeatureType.kWebView:
                    return WebView;

                case NativeFeatureType.kDeepLinkServices:
                    return DeepLinkServices;

                default:
                    return null;
            }
        }

        #endregion
    }
}