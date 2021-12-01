using UnityEngine;

namespace PIERStory
{
    public class HeightHitBox :MonoBehaviour
    {
        public int tallGrade = 1;

        private void OnTriggerEnter(Collider other)
        {
            GameModelCtrl touchModel = null;
            Transform p = null;

            p = other.transform.parent;

            while (p != null)
            {
                touchModel = p.GetComponent<GameModelCtrl>();

                if (touchModel != null)
                {
                    touchModel.SetTall(tallGrade);
                    return;
                }

                p = p.parent;
            }
        }
    }
}
