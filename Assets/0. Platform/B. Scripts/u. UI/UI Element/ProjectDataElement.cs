using UnityEngine;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ProjectDataElement : MonoBehaviour
    {
        public TextMeshProUGUI projectName;
        public TextMeshProUGUI projectDataSize;

        string projectId = string.Empty;

        public void InitProjectData(JsonData __j, long dataSize)
        {
            projectId = SystemManager.GetJsonNodeString(__j, LobbyConst.STORY_ID);

            projectName.text = SystemManager.GetJsonNodeString(__j, LobbyConst.STORY_TITLE);
            projectDataSize.text = string.Format("{0:#,0} KB", dataSize / 1024);

            if (dataSize / (1024 * 1024) > 0)
                projectDataSize.text = string.Format("{0:#,0} MB", dataSize / (1024 * 1024));
        }

        public void OnClickDeleteProjectData()
        {

        }

        void DeleteProjectData()
        {
            ES3.DeleteDirectory(Application.persistentDataPath + "/" + projectId);

            ViewDataManager.OnRequestCalcAllProejctDataSize?.Invoke();
            Destroy(gameObject);
        }
    }
}
