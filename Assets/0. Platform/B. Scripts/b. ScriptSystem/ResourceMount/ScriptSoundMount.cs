using System;
using UnityEngine;

using LitJson;
using BestHTTP;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


namespace PIERStory
{
    [Serializable]
    public class ScriptSoundMount
    {
        Action OnMountCompleted = delegate { };

        static readonly string SOUND_ID = "sound_id";
        static readonly string GAME_VOLUME = "game_volume";
        static readonly string SOUND_TYPE = "sound_type";

        public AudioClip audioClip = null;

        public string sound_id = string.Empty;

        public string template = string.Empty;
        public string sound_name = string.Empty;

        public string speaker = string.Empty;

        public string sound_url = string.Empty;
        public string sound_key = string.Empty;

        JsonData resourceData = null;

        public bool isMounted = false;

        public float volume = 1f;
        public string type = "bgm";
        
        
        public bool isAddressable = false; // 어드레서블 에셋인지 아닌지. (2022.02.11)
        public string addressableKey = string.Empty; // 어드레서블 키 
        
        public AsyncOperationHandle<AudioClip> mountedAddressable;
        

        public ScriptSoundMount(string __type, JsonData __j, Action __cb)
        {
            OnMountCompleted = __cb;
            resourceData = __j;

            template = __type;

            sound_id = SystemManager.GetJsonNodeString(resourceData, SOUND_ID);
            sound_name =  SystemManager.GetJsonNodeString(resourceData, CommonConst.SOUND_NAME);
            sound_url = SystemManager.GetJsonNodeString(resourceData, CommonConst.SOUND_URL);
            sound_key = SystemManager.GetJsonNodeString(resourceData, CommonConst.SOUND_KEY);

            volume = float.Parse(SystemManager.GetJsonNodeString(resourceData, GAME_VOLUME));
            type = SystemManager.GetJsonNodeString(resourceData, SOUND_TYPE);


            // * 생성과 동시에 로드
            // * 보이스는 에셋번들 거쳐서 처리             
            if(template == GameConst.COL_VOICE)
                MountVoice();
            else 
                MountSound();

        }
        
        /// <summary>
        /// 어드레서블 키 
        /// </summary>
        /// <param name="__templage"></param>
        /// <returns></returns>
        string GetAddressableKey(string __templage) {
            
            string middleKey = string.Empty;
            string key = string.Empty;
            
            switch(__templage) {
                case GameConst.COL_VOICE:
                middleKey = "/voice/";
                break;
            }
            
            // 중간 키 없으면 엠티 리턴 
            if(string.IsNullOrEmpty(middleKey)) {
                return string.Empty;
            }
            
            key = StoryManager.main.CurrentProjectID + middleKey + sound_name;
            
            if(sound_key.Contains(".mp3")) {
                key += ".mp3";
            }
            else if(sound_key.Contains(".wav")) {
                key += ".wav";
            }
            else {
                return string.Empty;
            }
            
            return key;
            
            
        }
        
        
        /// <summary>
        /// 보이스 - 어드레서블 마운트
        /// </summary>
        void MountVoice() {
            addressableKey = GetAddressableKey(template);
            
            Addressables.LoadResourceLocationsAsync(addressableKey).Completed += (op) => {
                
                // 에셋번들 있음 
               if(op.Status == AsyncOperationStatus.Succeeded && op.Result.Count > 0)  {
                   Addressables.LoadAssetAsync<AudioClip>(addressableKey).Completed += (handle) => {
                       if(handle.Status == AsyncOperationStatus.Succeeded) { // * 성공!
                            
                            isAddressable = true; // 어드레서블을 사용합니다. 
                            mountedAddressable = handle; // 메모리 해제를 위한 변수.
                            
                            audioClip = handle.Result; // 오디오클립 설정 
                            
                            SendSuccessMessage(); // 성공처리 
                       }
                       else {
                           Debug.Log(">> Failed LoadAssetAsync " + sound_name);
                           MountSound();
                       }
                   }; // end of LoadAssetAsync
               }
               else {
                   // 없음
                   MountSound();
               }
            }; // ? end of LoadResourceLocationsAsync            
            
        }
        

        /// <summary>
        /// 오디오 클립 불러오기 
        /// </summary>
        void MountSound()
        {
            try
            {
                
                
                if (ES3.FileExists(sound_key))
                {
                    if (sound_key.Contains("mp3") || sound_key.Contains("MP3"))
                        audioClip = ES3.LoadAudio(sound_key, AudioType.MPEG);
                    else if (sound_key.Contains("wav"))
                        audioClip = ES3.LoadAudio(sound_key, AudioType.WAV);

                    SendSuccessMessage();
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                SendFailMessage();
            }

            var req = new HTTPRequest(new Uri(sound_url), OnSoundDownloaded);
            req.Send();
        }

        void OnSoundDownloaded(HTTPRequest req, HTTPResponse res)
        {
            if (req.State != HTTPRequestStates.Finished)
            {
                Debug.LogError("Download failed : " + sound_url);
                SendFailMessage();
                return;
            }

            ES3.SaveRaw(res.Data, sound_key, SystemManager.noEncryptionSetting);

            if (sound_key.Contains("mp3"))
                audioClip = ES3.LoadAudio(sound_key, AudioType.MPEG);
            else if (sound_key.Contains("wav"))
                audioClip = ES3.LoadAudio(sound_key, AudioType.WAV);

            SendSuccessMessage();
        }

        void SendFailMessage()
        {
            isMounted = false;
            OnMountCompleted();
        }

        void SendSuccessMessage()
        {
            isMounted = true;
            OnMountCompleted();
        }
    }

}

