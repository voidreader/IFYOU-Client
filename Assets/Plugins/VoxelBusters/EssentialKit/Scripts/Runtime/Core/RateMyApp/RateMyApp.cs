using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using VoxelBusters.CoreLibrary;


namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// The RateMyApp class provides an unique way to prompt user to review the app. 
    /// </summary>
    /// <description>
    /// By default, prompt system makes use of configuration available in RateMyApp section of NativePluginsSettings. 
    /// These values can be adjusted according to developer preference.
    /// </description>
    public class RateMyApp : SingletonBehaviour<RateMyApp>
    {
        #region Fields

        private     IRateMyAppController        m_controller            = null;

        private     bool                        m_isShowingPrompt       = false;

        Action<int> callback = null;
        
        #endregion

        #region Static properties

        internal static RateMyAppSettings Settings
        {
            get
            {
                return EssentialKitSettings.Instance.ApplicationSettings.RateMyAppSettings;
            }
        }

        #endregion

        #region Static methods

        public static void AskForReviewNow(string __title, string __contents, string __okLabel, string __cancelLabel, Action<int> __callback)
        {
            // check whether feature is available
            if (!IsSingletonActive)
            {
                DebugLogger.LogError("Feature is not active.");
                return;
            }

            Instance.callback = __callback;

            Instance.ShowPromptWindow(__title, __contents, __okLabel, __cancelLabel);
        }



        /// <summary>
        /// Immediately prompts user to review. This method ignores IRateMyAppValidator conditions to be satisfied.
        /// </summary>
        public static void AskForReviewNow()
        {
            // check whether feature is available
            if (!IsSingletonActive)
            {
                DebugLogger.LogError("Feature is not active.");
                return;
            }

            Instance.ShowPromptWindow();
        }

        #endregion

        #region Unity methods

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();

            // configure component
            m_controller = GetComponent<IRateMyAppController>();

            if(m_controller == null)
                m_controller = gameObject.AddComponent<RateMyAppDefaultController>();
        }

        private void Update()
        {
            if (m_isShowingPrompt || m_controller == null)
            {
                return;
            }
            if (m_controller.CanShowRateMyApp())
            {
                ShowPromptWindow();
            }
        }

        #endregion

        #region Private methods

        void ShowPromptWindow(string __title, string __description, string __okLabel, string __cancelLabel)
        {
            // mark that we are showing window
            m_isShowingPrompt = true;

            // create prompt
            var dialogSettings = Settings.ConfirmationDialogSettings;
            var localisationServiceProvider = ExternalServiceProvider.LocalisationServiceProvider;
            var dialogBuilder = new AlertDialogBuilder()
                .SetTitle(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kTitle, __title))
                .SetMessage(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kDescription, __description))
                .AddButton(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kOkButton, __okLabel), () => OnPromptButtonPressed(PromptButtonType.Ok))
                .AddCancelButton(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kCancelButton, __cancelLabel), () => OnPromptButtonPressed(PromptButtonType.Cancel));
            if (dialogSettings.CanShowRemindMeLaterButton)
            {
                dialogBuilder.AddButton(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kRemindLaterButton, defaultValue: dialogSettings.RemindLaterButtonLabel), () => OnPromptButtonPressed(PromptButtonType.RemindLater));
            }
            var newAlertDialog = dialogBuilder.Build();
            newAlertDialog.Show();
        }

        private void ShowPromptWindow()
        {
            // mark that we are showing window
            m_isShowingPrompt = true;

            // create prompt
            var     dialogSettings  = Settings.ConfirmationDialogSettings;
            if (dialogSettings.CanShow)
            {
                var     localisationServiceProvider = ExternalServiceProvider.LocalisationServiceProvider;
                var     dialogBuilder               = new AlertDialogBuilder()
                    .SetTitle(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kTitle, defaultValue: dialogSettings.PromptTitle))
                    .SetMessage(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kDescription, defaultValue: dialogSettings.PromptDescription))
                    .AddButton(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kOkButton, defaultValue: dialogSettings.OkButtonLabel), () => OnPromptButtonPressed(PromptButtonType.Ok))
                    .AddCancelButton(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kCancelButton, defaultValue: dialogSettings.CancelButtonLabel), () => OnPromptButtonPressed(PromptButtonType.Cancel));
                if (dialogSettings.CanShowRemindMeLaterButton)
                {
                    dialogBuilder.AddButton(localisationServiceProvider.GetLocalisedString(key: RateMyAppLocalisationKey.kRemindLaterButton, defaultValue: dialogSettings.RemindLaterButtonLabel), () => OnPromptButtonPressed(PromptButtonType.RemindLater));
                }
                var newAlertDialog  = dialogBuilder.Build();
                newAlertDialog.Show();
            }
            else
            {
                OnPromptButtonPressed(PromptButtonType.Ok);
            }
        }

        private void OnPromptButtonPressed(PromptButtonType selectedButtonType)
        {
            // reset flag
            m_isShowingPrompt = false;
            switch (selectedButtonType)
            {
                case PromptButtonType.RemindLater:
                    m_controller.DidClickOnRemindLaterButton();
                    break;

                case PromptButtonType.Cancel:
                    m_controller.DidClickOnCancelButton();

                    Instance.callback?.Invoke(0);
                    break;

                case PromptButtonType.Ok:
                    m_controller.DidClickOnOkButton();
                    ShowReviewWindow();

                    Instance.callback?.Invoke(1);
                    break;
            }
        }

        private void ShowReviewWindow()
        {
            Utilities.RequestStoreReview();
        }

        #endregion

        #region Nested types

        private enum PromptButtonType
        {
            RemindLater,

            Cancel,

            Ok,
        }

        #endregion
    }
}