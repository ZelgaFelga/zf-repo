#region GPL Licence
/**
*Copyright 2022 GHEBACHE Cherrad (email : dzkernel@gmail.com) , MEGRI Lyes (email : megri.lyes@gmail.com)
*
*This file is part of Zelga Felga.
*
*Zelga Felga is free software; you can redistribute it and/or modify
*it under the terms of the GNU General Public License as published by
*the Free Software Foundation; either version 3 of the License, or
*(at your option) any later version.
*
*Zelga Felga is distributed in the hope that it will be useful,
*but WITHOUT ANY WARRANTY; without even the implied warranty of
*MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*GNU General Public License for more details.

*You should have received a copy of the GNU General Public License
*along with this program; if not, see 
*<https://www.gnu.org/licenses/gpl-3.0.txt>
*
*/
#endregion

using Mirror;
using UnityEngine;


namespace Prototype1
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool IsGameInPause = false;

        [SerializeField]
        private GameObject mainMenuPanel;

        [SerializeField]
        private GameObject pauseMenuPanel;

        [SerializeField]
        private GameObject optionsMenuPanel;

        [SerializeField]
        private GameObject newMatchMenuPanel;

        [SerializeField]
        private ZFNetworkManager networkManager;


        ///<summary>Executed once per frame</summary>
        public virtual void Update()
        {
            if (networkManager == null)
                networkManager = FindObjectOfType<ZFNetworkManager>();

          

            if (Input.GetKeyDown(KeyCode.Escape))
            {


                if (IsGameInPause)
                {//Jeu en pause

                    //Menu option actif
                    if (optionsMenuPanel.activeSelf)
                    {

                        optionsMenuPanel.SetActive(false);
                        Pause();

                        //Menu pause actif
                    }
                    else if (pauseMenuPanel.activeSelf)
                    {

                        Resume();

                    }
                }
                else
                {// Jeu pas en pause

                    //Aucun menu est actif
                    if (NoMenuIsActive)
                        Pause();
                }

            }
        }

        ///<summary>Pause the game</summary>
        public virtual void Pause()
        {
            pauseMenuPanel.SetActive(true);
           
            IsGameInPause = true;
            Cursor.lockState = CursorLockMode.None;

        }

        ///<summary>Resume the game</summary>
        public virtual void Resume()
        {
            pauseMenuPanel.SetActive(false);
           
            IsGameInPause = false;
            Cursor.lockState = CursorLockMode.Locked;

        }


        ///<summary>Go to configuration menu</summary>

        public virtual void GoToOptionsMenu()
        {
            IsGameInPause = true;
            
            pauseMenuPanel.SetActive(false);
            optionsMenuPanel.SetActive(true);
        }

        ///<summary>Return to main menu</summary>
        public virtual void GoBackToMainMenu()
        {
            IsGameInPause = false;
            
            if (NetworkClient.isConnected && NetworkServer.active){
                Debug.Log("Host disconnected !");
                networkManager.StopHost();
            }
            else if(NetworkClient.isConnected && !NetworkServer.active)
            {
                networkManager.StopClient();
                Debug.Log("Client disconnected !");
            }

            pauseMenuPanel.SetActive(false);
                   
        }

        bool NoMenuIsActive
        {

            get { return !mainMenuPanel.activeSelf && !optionsMenuPanel.activeSelf && !pauseMenuPanel.activeSelf && !newMatchMenuPanel.activeSelf; }
        }
    }
}
