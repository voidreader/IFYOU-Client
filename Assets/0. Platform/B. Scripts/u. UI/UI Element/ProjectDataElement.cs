using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ProjectDataElement : MonoBehaviour
    {
        StoryData storyData = null;
        public TextMeshProUGUI projectName;
        public TextMeshProUGUI projectDataSize;


        public void InitProjectData(StoryData __data, long dataSize)
        {
            storyData = __data;

            projectName.text = __data.title;
            projectDataSize.text = string.Format("{0:#,0} KB", dataSize / 1024);

            if (dataSize / (1024 * 1024) > 0)
                projectDataSize.text = string.Format("{0:#,0} MB", dataSize / (1024 * 1024));
        }

        public void OnClickDeleteProjectData()
        {
            SystemManager.ShowLobbyPopup(SystemManager.GetLocalizedText("6021"), DeleteProjectData, null);
        }

        void DeleteProjectData()
        {
            ES3.DeleteDirectory(Application.persistentDataPath + "/" + storyData.projectID);

            ViewDataManager.OnRequestCalcAllProejctDataSize?.Invoke();

            Destroy(gameObject);
        }
    }
}
