using UnityEngine;
using System.Collections;

public class ScissorCtrl : MonoBehaviour {


    public GameObject m_objBackScissor;

    [SerializeField]
    bool _needChange = true;


    // Use this for initialization
    void Awake() {


        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        Debug.Log("★★★ ScissorCtrl currentAspectRatio :: " + currentAspectRatio);

        if (currentAspectRatio > 0.57f) 
            _needChange = true;
        else
            _needChange = false;

        UpdateResolution();
    }

    // Use this for initialization
    void Start() {
        
    }


    public void UpdateResolution() {

        if (!_needChange)
            return;

        Debug.Log("★★★ ScissorCtrl UpdateResolution");

        Camera[] objCameras = Camera.allCameras;

        //width 2, height 3
        float fResolutionX = Screen.width / 9.0f;
        float fResolutionY = Screen.height / 16.0f;

        if (fResolutionX > fResolutionY) {
            float fValue = (fResolutionX - fResolutionY) * 0.5f;
            fValue = fValue / fResolutionX;

            //fResolutionX fix, left & right Scissor (Viewport Re Setting)
            foreach (Camera obj in objCameras) {
                obj.rect = new Rect(Screen.width * fValue / Screen.width + obj.rect.x * (1.0f - 2.0f * fValue), obj.rect.y
                    , obj.rect.width * (1.0f - 2.0f * fValue), obj.rect.height);
            }

            GameObject objLeftScissor = (GameObject)Instantiate(m_objBackScissor);
            objLeftScissor.GetComponent<Camera>().rect = new Rect(0, 0, Screen.width * fValue / Screen.width, 1.0f);

            GameObject objRightScissor = (GameObject)Instantiate(m_objBackScissor);
            objRightScissor.GetComponent<Camera>().rect = new Rect((Screen.width - Screen.width * fValue) / Screen.width, 0
                , Screen.width * fValue / Screen.width, 1.0f);
        }
        else if (fResolutionX < fResolutionY) {

            float fValue = (fResolutionY - fResolutionX) * 0.5f;
            fValue = fValue / fResolutionY;

            //fResolutionY fix, Top & Bottom Scissor (Viewport Re Setting)
            foreach (Camera obj in objCameras) {
                obj.rect = new Rect(obj.rect.x, Screen.height * fValue / Screen.height + obj.rect.y * (1.0f - 2.0f * fValue)
                       , obj.rect.width, obj.rect.height * (1.0f - 2.0f * fValue));

                //obj.rect = new Rect( obj.rect.x , obj.rect.y + obj.rect.y * fValue, obj.rect.width, obj.rect.height - obj.rect.height * fValue );
            }

            GameObject objTopScissor = (GameObject)Instantiate(m_objBackScissor);
            objTopScissor.GetComponent<Camera>().rect = new Rect(0, 0, 1.0f, Screen.height * fValue / Screen.height);

            GameObject objBottomScissor = (GameObject)Instantiate(m_objBackScissor);
            objBottomScissor.GetComponent<Camera>().rect = new Rect(0, (Screen.height - Screen.height * fValue) / Screen.height
                , 1.0f, Screen.height * fValue / Screen.height);
        }
        else {
            // Do Not Setting Camera
        }
    }

}
