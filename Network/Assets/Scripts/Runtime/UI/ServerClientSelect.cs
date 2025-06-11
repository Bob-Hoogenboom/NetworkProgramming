using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class ServerClientSelect : MonoBehaviour
    {
        [SerializeField]
        private GameObject serverPrefab;
        [SerializeField]
        private GameObject clientPrefab;

        public void OnServerClientClick()
        {
            OnServerClick();
            OnClientClick();
            SceneManager.LoadScene("Lobby");
        }

        public void OnServerClick()
        {
            DontDestroyOnLoad(Instantiate(serverPrefab).gameObject);
            SceneManager.LoadScene("Lobby");
        }

        public void OnClientClick()
        {
            DontDestroyOnLoad(Instantiate(clientPrefab).gameObject);
            SceneManager.LoadScene("Lobby");
        }
    }
}