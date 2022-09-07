using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit.CloudServicesCore;

namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// This class contains the information related to <see cref="CloudServices.OnSavedDataChange"/> event.
    /// </summary>
    public class CloudServicesSavedDataChangeResult
    {
        #region Properties

        /// <summary>
        /// The reason causing local data change.
        /// </summary>
        public CloudSavedDataChangeReasonCode ChangeReason { get; private set; }

        /// <summary>
        /// An array of changed keys.
        /// </summary>
        public string[] ChangedKeys { get; private set; }

        #endregion

        #region Constructors

        public CloudServicesSavedDataChangeResult(CloudSavedDataChangeReasonCode changeReason, string[] changedKeys)
        {
            // set properties
            ChangeReason    = changeReason;
            ChangedKeys     = changedKeys;
        }

        #endregion
    }
}