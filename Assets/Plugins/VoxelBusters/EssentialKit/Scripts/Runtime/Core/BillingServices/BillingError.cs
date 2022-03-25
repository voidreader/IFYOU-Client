using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// Billing services error domain.
    /// </summary>
    public static class BillingErrorDomain
    {
        public  const   string  kName        = "BillingServices";
    }

    /// <summary>
    /// Constants indicating the possible error that might occur when using billing services.
    /// </summary>
    public static class BillingErrorCode
    {
        /// <summary> Error code indicating that an unknown or unexpected error occurred. </summary>
        public  const   int     kUnknown                = 0;

        /// <summary> Error code indicating that product is not found. </summary>
        public  const   int     kStoreNotInitialized    = 1;

        /// <summary> Error code indicating that store is not available. </summary>
        public  const   int     kStoreNotAvailable      = 2;

        /// <summary> Error code indicating that product is not found. </summary>
        public  const   int     kProductNotFound        = 3;
    }

    public static class BillingError
    {
        public  static   readonly   Error   kUnknown                = new Error(domain: BillingErrorDomain.kName, code: BillingErrorCode.kUnknown, description: "Unknown error.");

        public  static   readonly   Error   kStoreNotInitialized    = new Error(domain: BillingErrorDomain.kName, code: BillingErrorCode.kStoreNotInitialized, description: "Store not initialized.");

        public  static   readonly   Error   kStoreNotAvailable      = new Error(domain: BillingErrorDomain.kName, code: BillingErrorCode.kStoreNotAvailable, description: "Store not available.");

        public  static   readonly   Error   kProductNotFound        = new Error(domain: BillingErrorDomain.kName, code: BillingErrorCode.kProductNotFound, description: "Product not found.");
    }
}