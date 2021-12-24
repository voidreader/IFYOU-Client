using System;
using UnityEngine;

using LitJson;
using BestHTTP;

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

            // 생성과 동시에 로드하도록 변경
            MountSound();
        }

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

