using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

using TMPro;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewDataManager : CommonView
    {
        public static Action OnRequestCalcAllProejctDataSize = null;

        public TextMeshProUGUI totalDataSizeText;
        public UIButton deleteAllButton;
        public GameObject projectDataSizePrefab;
        public Transform elementParent;             // 프리팹이 생성될 위치
        
        long totalDataSize = 0;

        ProjectDataElement projectElement = null;
        List<GameObject> projectPrefabs = new List<GameObject>();

        DirectoryInfo dirInfo = null;

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);

            OnRequestCalcAllProejctDataSize = CalcAllProjectDataSize;
            OnRequestCalcAllProejctDataSize?.Invoke();

            for (int i = 0; i < StoryManager.main.listTotalStory.Count; i++)
            {
                string path = Application.persistentDataPath + "/" + StoryManager.main.listTotalStory[i].projectID;
                dirInfo = new DirectoryInfo(path);

                if (dirInfo.Exists)
                {
                    long size = 0;

                    foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                        size += fi.Length;

                    projectElement = Instantiate(projectDataSizePrefab, elementParent).GetComponent<ProjectDataElement>();
                    projectElement.InitProjectData(StoryManager.main.listTotalStory[i], size);
                    projectPrefabs.Add(projectElement.gameObject);
                }
            }
        }

        public override void OnView()
        {
            base.OnView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5046"), string.Empty);
        }

        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);

            foreach (GameObject g in projectPrefabs)
                Destroy(g);

            projectPrefabs.Clear();
        }

        void CalcAllProjectDataSize()
        {
            for(int i=0;i<StoryManager.main.listTotalStory.Count;i++)
            {
                // 프로젝트 폴더 경로
                string path = Application.persistentDataPath + "/" + StoryManager.main.listTotalStory[i].projectID;
                dirInfo = new DirectoryInfo(path);

                if(dirInfo.Exists)
                {
                    long size = 0;

                    foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                        size += fi.Length;

                    totalDataSize += size;
                }

                // 1 KB = 1024Byte, 1MB = 1024KB, 1GB = 1024MB
                long totalDataLength = totalDataSize / (1024 * 1024);
                totalDataSizeText.text = string.Format("{0:#,0} MB", totalDataLength);

                // 1기가가 넘으면 한번더 나눠줘서 GB단위로 표시해준다
                if (totalDataLength > 1024)
                    totalDataSizeText.text = string.Format("{0:0.00} GB", (float)(totalDataLength / 1024));
            }
        }

        public void OnClickDeleteAllData()
        {
            // 전체 데이터가 0이하면 아무것도 실행하지 않는다
            if (totalDataSize < 1)
                return;

            SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("6021"), DeleteAllProjectData, null);
        }

        void DeleteAllProjectData()
        {
            for (int i = 0; i < StoryManager.main.listTotalStory.Count; i++)
            {
                // 프로젝트 폴더 경로
                string path = Application.persistentDataPath + "/" + StoryManager.main.listTotalStory[i].projectID;
                dirInfo = new DirectoryInfo(path);

                if (dirInfo.Exists)
                    ES3.DeleteDirectory(path);
            }
        }
    }
}
 