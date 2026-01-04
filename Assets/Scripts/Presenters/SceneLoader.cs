using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrashSucker.Presenters
{
    public class SceneLoader: MonoBehaviour
    {
        public static void LoadSceneByIndex(int index)
        {
            SceneManager.LoadScene(index);
        }

        public static void LoadSceneByName(string name)
        {
            SceneManager.LoadScene(name);
        }

        public static void Quit() => Application.Quit();

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
