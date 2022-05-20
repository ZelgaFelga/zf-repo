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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype1;
using Steamworks;
using Mirror;
using UnityEngine.SceneManagement;

namespace Prototype3
{
    public class Menu3 : Menu
    {
        
        [SerializeField]
        private GameObject GameBackground;

        [SerializeField]
        private GameObject zfLogo;

        [SerializeField] 
        private GameObject copyright;

        [SerializeField]
        private GameObject gpl;


        [SerializeField]
        private Canvas RoundTimerUI;

        [SerializeField]
        private Canvas PlantTimerUI;

        [SerializeField]
        private Canvas UIplayer;

        [SerializeField]
        private GameObject WorldCamera;


        private CSteamID lobbySteamID;

        private CSteamID lobbyOwnerSteamID;

        #region Steam lobby

        protected override void OnLobbyCreated(LobbyCreated_t callback){
            
            if(NetworkServer.active) return;

            base.OnLobbyCreated(callback);
        }

        protected override void OnLobbyEntered(LobbyEnter_t callback){

            base.OnLobbyEntered(callback);

            lobbySteamID = new CSteamID(callback.m_ulSteamIDLobby);

            lobbyOwnerSteamID = SteamMatchmaking.GetLobbyOwner(lobbySteamID);

            Debug.Log($"Lobby : {lobbySteamID}, Owner : {lobbyOwnerSteamID}");

            Debug.Log("My local net id is " + NetworkConnection.LocalConnectionId);

            EnableGameBackgroundImage(false);
            
        }

        protected void DisposeLobbyCallbacks(){
                
            lobbyEntered.Dispose();
            gameLobbyJoinRequested.Dispose();
            lobbyCreated.Dispose();

        }


        #endregion

        #region Changing Panel
        
        public override void goToNewMatchPanel(){

            base.goToNewMatchPanel();
        }

        public override void goToConfigPanel()
        {
            base.goToConfigPanel();

        }
        #endregion

        #region Gérer la déconnection du joueur client / Hôte


        public override void HandleClientDisconnected()
        {

            Debug.Log("Player disconnected !");
            WorldCamera.SetActive(true);
            #region original code commented

            // if(NetworkClient.isConnected && NetworkServer.active){
            //     //host

            //     if(UseSteam){

            //         SteamMatchmaking.SetLobbyJoinable(lobbySteamID,false);
            //         SteamMatchmaking.DeleteLobbyData(lobbySteamID,HostAddressKey);

            //         lobbyCreated.Dispose();
            //         lobbyEntered.Dispose();
            //     }

            // }
            // else
            // {
            //     UIplayer.enabled = false;
            // }


            // //Host or client
            // if(UseSteam){
            //     SteamMatchmaking.LeaveLobby(lobbySteamID);
            //     SteamMatchmaking.DeleteLobbyData(lobbySteamID,HostAddressKey);
            //     gameLobbyJoinRequested.Dispose();
            // }
           

            // base.HandleClientDisconnected();


            
            // if(UseSteam)
            //     CreateLobbyCallBacks();

            #endregion

            #region test code

            if(UseSteam){

                SteamMatchmaking.DeleteLobbyData(lobbySteamID,HostAddressKey);
                SteamMatchmaking.LeaveLobby(lobbySteamID);
                DisposeLobbyCallbacks();
                networkManager.networkAddress = "localhost";

            }
            
            base.HandleClientDisconnected();

            #endregion

            //original code uncommented

            UIplayer.enabled = false;

            if(!NetworkClient.isConnected || !NetworkServer.active){

                RoundTimerUI.enabled = false;
                PlantTimerUI.enabled = false;
            }

            EnableGameBackgroundImage(true);
            
        }

        #endregion

        #region  Gérer la connection du client

        ///<summary>Handles main menu panel's display when a client connects</summary>
        ///<remarks>
        ///Deactivates main menu panel, 
        ///Enables RoundTimer,
        ///Enables PlantTimer,
        ///Enables PlayerUI
        ///</remarks>
        public override void HandleClientConnected()
        {
            Debug.Log("Player connected !");

            WorldCamera.SetActive(false);

            base.HandleClientConnected();


            EnableGameBackgroundImage(false);

            RoundTimerUI.enabled = true;
            PlantTimerUI.enabled = true;
            UIplayer.enabled = true;
        }

        #endregion

        #region GameBackgroundImage
        private void EnableGameBackgroundImage(bool value){
            GameBackground.SetActive(value);
            zfLogo.SetActive(value);
            copyright.SetActive(value);
            gpl.SetActive(value);
        }
        #endregion
    }

    
}
