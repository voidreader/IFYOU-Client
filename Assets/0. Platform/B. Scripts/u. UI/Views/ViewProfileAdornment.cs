using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewProfileAdornment : CommonView
    {
        public static Action OnSelectCanel = null;

        [Header("프로필 사진")]
        public Transform profilePictureContent;
        public GameObject profilePicturePrefab;
        public GameObject pictureScroll;
        public Image pictureToggle;
        public TextMeshProUGUI pictureToggleText;

        [Header("프로필 테두리")]
        public Transform profileFrameContent;
        public GameObject profileFramePrefab;
        public GameObject frameScroll;
        public Image frameToggle;
        public TextMeshProUGUI frameToggleText;

        JsonData profileCurrencyList = null;
        List<ProfileBriefElement> portraitList = new List<ProfileBriefElement>();
        List<ProfileBriefElement> frameList = new List<ProfileBriefElement>();
        List<GameObject> createObject = new List<GameObject>();

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);

            profileCurrencyList = UserManager.main.userProfileCurrency;

            LoadProfilePictureData();
            LoadProfileFrameData();
        }

        public override void OnView()
        {
            base.OnView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_MULTIPLE_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_MULTIPLE_BUTTON_LABEL, "변경", string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "프로필 꾸미기", string.Empty);

            ViewCommonTop.OnClickButtonAction = OnClickSaveProfile;
            OnSelectCanel = SelectCancel;
        }


        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            foreach (GameObject g in createObject)
                Destroy(g);

            createObject.Clear();
            portraitList.Clear();
            frameList.Clear();
        }


        #region Toggle Action
        
        /// <summary>
        /// 프로필 사진 토글 활성화
        /// </summary>
        public void EnablePicture()
        {
            if (LobbyManager.main == null)
                return;

            pictureScroll.SetActive(true);
            pictureToggle.sprite = LobbyManager.main.spriteGenreOn;
            pictureToggleText.color = LobbyManager.main.colorGenreOn;
            frameScroll.SetActive(false);
            frameToggle.sprite = LobbyManager.main.spriteGenreOff;
            frameToggleText.color = LobbyManager.main.colorGenreOff;
        }

        /// <summary>
        /// 프로필 테두리 토글 활성화
        /// </summary>
        public void EnableFrame()
        {
            pictureScroll.SetActive(false);
            pictureToggle.sprite = LobbyManager.main.spriteGenreOff;
            pictureToggleText.color = LobbyManager.main.colorGenreOff;
            frameScroll.SetActive(true);
            frameToggle.sprite = LobbyManager.main.spriteGenreOn;
            frameToggleText.color = LobbyManager.main.colorGenreOn;
        }



        #endregion

        public void SelectCancel()
        {
            if(pictureScroll.activeSelf)
            {
                foreach (ProfileBriefElement be in portraitList)
                    be.SelectCancel();
            }
            else
            {
                foreach (ProfileBriefElement be in frameList)
                    be.SelectCancel();
            }
        }


        void OnClickSaveProfile()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = LobbyConst.FUNC_USER_PROFILE_SAVE;
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["kind"] = "profile";


            for (int i=0;i<portraitList.Count;i++)
            {
                if (portraitList[i].currentCount == 1)
                {
                    sending[LobbyConst.NODE_CURRENCY_LIST] = JsonMapper.ToObject("[]");
                    sending[LobbyConst.NODE_CURRENCY_LIST].Add(portraitList[i].SaveJsonData());
                    break;
                }
            }

            for (int i = 0; i < frameList.Count; i++)
            {
                if (frameList[i].currentCount == 1)
                {
                    if(!sending.ContainsKey(LobbyConst.NODE_CURRENCY_LIST))
                        sending[LobbyConst.NODE_CURRENCY_LIST] = JsonMapper.ToObject("[]");

                    sending[LobbyConst.NODE_CURRENCY_LIST].Add(frameList[i].SaveJsonData());
                    break;
                }
            }

            NetworkLoader.main.SendPost(CallbackSaveProfile, sending, true);
        }

        void CallbackSaveProfile(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackSaveProfile");
                return;
            }

            UserManager.main.userProfile = JsonMapper.ToObject(res.DataAsText);
            Debug.Log(JsonMapper.ToStringUnicode(UserManager.main.userProfile));
            ViewMain.OnProfileSetting?.Invoke();
            Signal.Send(LobbyConst.STREAM_IFYOU, LobbyConst.SIGNAL_SAVE_PROFILE_DECO);

        }

        void LoadProfilePictureData()
        {
            if (!profileCurrencyList.ContainsKey(LobbyConst.NODE_PORTRAIT) || profileCurrencyList[LobbyConst.NODE_PORTRAIT] == null)
            {
                return;
            }

            ProfileBriefElement portraitElement = null;

            for(int i=0;i<profileCurrencyList[LobbyConst.NODE_PORTRAIT].Count;i++)
            {
                portraitElement = Instantiate(profilePicturePrefab, profilePictureContent).GetComponent<ProfileBriefElement>();
                portraitElement.InitProfileBrief(profileCurrencyList[LobbyConst.NODE_PORTRAIT][i]);

                portraitList.Add(portraitElement);
                createObject.Add(portraitElement.gameObject);
            }
        }

        void LoadProfileFrameData()
        {
            if (!profileCurrencyList.ContainsKey(LobbyConst.NODE_FRAME) || profileCurrencyList[LobbyConst.NODE_FRAME] == null)
            {
                return;
            }

            ProfileBriefElement frameElement = null;

            for (int i = 0; i < profileCurrencyList[LobbyConst.NODE_FRAME].Count; i++)
            {
                frameElement = Instantiate(profileFramePrefab, profileFrameContent).GetComponent<ProfileBriefElement>();
                frameElement.InitProfileBrief(profileCurrencyList[LobbyConst.NODE_FRAME][i]);

                frameList.Add(frameElement);
                createObject.Add(frameElement.gameObject);
            }
        }
    }
}