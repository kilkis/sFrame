using UnityEngine;
using System.Collections;
namespace sFrame
{
    public class sULoading : MonoBehaviour
    {

        public static sULoading instance;
        public GameObject loading;
        public GameObject maincamera;

        public GameObject playerCC;

        void Awake()
        {
            instance = this;
        }
        // Use this for initialization
        void Start()
        {

        }

        public void hideLoading()
        {
            loading.SetActive(false);
        }

        public void showLoading()
        {
            loading.SetActive(true);
        }

        public void enableCamera()
        {
            maincamera.SetActive(true);
        }

        public void disableCamera()
        {
            maincamera.SetActive(false);
        }
    }
}