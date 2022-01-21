using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

namespace ThirdPerson
{

    public class Menu : MonoBehaviour
    {
        public GameObject menu;

        // Start is called before the first frame update
        public void CloseGame()
        {
            Application.Quit();
        }
        public void PlayHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        public void PlayClient()
        {
            NetworkManager.Singleton.StartClient();
        }
        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
        }
        public void HideMenu()
        {
            menu.SetActive(false);
        }
    }
}
