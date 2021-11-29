using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ViewDataManager : CommonView
    {
        public static Action OnRequestCalcAllProejctDataSize = null;

        public TextMeshProUGUI totalDataSize;
        DirectoryInfo dirInfo = null;

        void CalcAllProjectDataSize()
        {
            JsonData projectData = StoryManager.main.totalStoryListJson;
            long totalDataSize = 0;

            for(int i=0;i<projectData.Count;i++)
            {
                // 프로젝트 폴더 경로
                string path = Application.persistentDataPath + "/" + SystemManager.GetJsonNodeString(projectData[i], LobbyConst.STORY_ID);
                dirInfo = new DirectoryInfo(path);

                if(dirInfo.Exists)
                {
                    long size = 0;

                    foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
                        size += fi.Length;

                    totalDataSize += size;
                }
            }
        }
    }
}
