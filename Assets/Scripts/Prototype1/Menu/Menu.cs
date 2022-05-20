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


using kcp2k;
using Mirror;
using Mirror.Discovery;
using Mirror.FizzySteam;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype1
{

    public class Menu : MonoBehaviour
    {

        #region Events

        public static event Action OnChooseKcpTransport;
        public static event Action OnChooseFizzySteamWorksTransport;

        #endregion

        #region Menus Gameobjects
        [SerializeField]
        private GameObject mainMenuPanel;

        [SerializeField]
        private GameObject newMatchPanel;

        [SerializeField]
        private GameObject configPanel;

        [SerializeField]
        private GameObject pausePanel;
       

        #endregion

        [SerializeField]
        private NetworkDiscovery networkDiscovery;

        readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

#if UNITY_EDITOR
        void OnValidate()
        {
            if(networkDiscovery == null)
                UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
        }
#endif
        #region steamLobby

        public bool UseSteam { get; set; }

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        
        protected const string HostAddressKey = "HostAddress";

        public ZFNetworkManager networkManager;


        ///<summary>Called once on the frame before Update</summary>
        void Start()
        {

            UseSteam = true;
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            

        }
        
        ///<summary>Executed when the object is enabled</summary>
        private void OnEnable()
        {
            ZFNetworkManager.OnClientDisconnected += HandleClientDisconnected;
            ZFNetworkManager.OnClientConnected += HandleClientConnected;
            
        }

        ///<summary>Executed each frame</summary>
        void Update()
        {

            if (networkManager == null)
                networkManager = FindObjectOfType<ZFNetworkManager>();
        }

        ///<summary>Executed when the object is destroyed</summary>
        void OnDestroy()
        {
            ZFNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
            ZFNetworkManager.OnClientConnected -= HandleClientConnected;
        }

        ///<summary>Creates the lobby callbacks</summary>
        public void CreateLobbyCallBacks()
        {

            if (UseSteam)
            {

                lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
                gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
                lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

            }

        }

        ///<summary>Host a local or an online game</summary>
        public void HostLobby()
        {
            newMatchPanel.SetActive(false);

            if (UseSteam)
            {//Online game


                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly,
                 networkManager.maxConnections);

                return;

            }else{


                //Local game


                EnableKcpTransport();

                networkDiscovery.transport = networkDiscovery.GetComponent<KcpTransport>();

                //host local game
                networkManager.StartHost();

                networkDiscovery.AdvertiseServer();


            }


        }

        ///<summary>Starts the host if the lobby was successfully created</summary>
        ///<param name="callback">The callback returned from the creation of the lobby</param>
        protected virtual void OnLobbyCreated(LobbyCreated_t callback)
        {
            
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                newMatchPanel.SetActive(true);
                return;
            }

            //Host an online game
            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey,
                SteamUser.GetSteamID().ToString());

            Debug.Log($"Lobby Id {callback.m_ulSteamIDLobby}");

        }

        ///<summary>When a new client requests to join the game from Steam</summary>
        ///<param name="callback">The callback returned when a client requests to join a game from steam</param>
        protected void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        ///<summary>When a new client enters the created lobby</summary>
        ///<param name="callback">The callback returned when a client enters the lobby</param>
        protected virtual void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active) { return; }
            
            string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey);

            //Join an online game
            networkManager.networkAddress = hostAddress;

            Debug.Log("Host address " + hostAddress);
            
            networkManager.StartClient();

            UseSteam = true;

            mainMenuPanel.SetActive(false);

        }
        #endregion


        #region Changing Panel
        ///<summary>Display the new match panel</summary>
        public virtual void goToNewMatchPanel()
        {
            mainMenuPanel.SetActive(false);
            newMatchPanel.SetActive(true);

        }

        ///<summary>Display the configuration panel</summary>
        public virtual void goToConfigPanel()
        {
            mainMenuPanel.SetActive(false);
            configPanel.SetActive(true);
        }
        #endregion

        //joindre une partie locale
        #region Join a local game
        ///<summary>Enables KcpTransport then starts the client</summary>
        public void JoinLocalMatch()
        {
            //Enabling kcp transport

            EnableKcpTransport();
            
            networkDiscovery.StartDiscovery();
            // networkManager.StartClient();


        }
        #endregion

        #region Gérer la déconnection du joueur client / Hôte


        // Handling client disconnection
        ///<summary>Handles main menu panel's display when a client disconnects</summary>

        public virtual void HandleClientDisconnected()
        {
            //-------------------------------
            // if the client is the host
            // if (NetworkClient.isConnected && NetworkServer.active)
            // if(NetworkClient.isHostClient)
            // {
            //     networkManager.StopHost();//We stop the server / local client
            // }
            // else
            // {//else
              
            //     networkManager.StopClient();//We stop le client
            // }
            // ------------------------------------------------------------------
            
            //Go back to Main menu
            pausePanel.SetActive(false);

            configPanel.SetActive(false);

            mainMenuPanel.SetActive(true);

            //reset cursor
            Cursor.lockState = CursorLockMode.None;

            // Switching back to Fizzy Transport

            networkDiscovery.StopDiscovery();

            EnableFizzySteamWorksTransport();

        }

        ///<summary>Enables FizzySteamWorks Transport</summary>
        private void EnableFizzySteamWorksTransport()
        {

            UseSteam = true;

            networkManager.GetComponent<KcpTransport>().enabled = false;

            networkManager.GetComponent<SteamManager>().enabled = true;
            networkManager.GetComponent<FizzySteamworks>().enabled = true;


            OnChooseFizzySteamWorksTransport?.Invoke();


            CreateLobbyCallBacks();//added

        }

        ///<summary>Enables Kcp Transport</summary>
        private void EnableKcpTransport()
        {

            UseSteam = false;

            networkManager.GetComponent<KcpTransport>().enabled = true;
            OnChooseKcpTransport?.Invoke();


            networkManager.GetComponent<FizzySteamworks>().enabled = false;

            networkManager.GetComponent<SteamManager>().enabled = false;


        }

        ///<summary>Quit the game</summary>
        public void QuitGame()
        {
            Application.Quit();

        }
        #endregion

        #region Gérer la connection du client

        ///<summary>Handles main menu panel's display when a client connects</summary>
        public virtual void HandleClientConnected()
        {

            mainMenuPanel.SetActive(false);

            newMatchPanel.SetActive(false);

            configPanel.SetActive(false);
           
            PauseMenu.IsGameInPause = false;
        }

        #endregion

        public void OnDiscoveredServer(ServerResponse info){
            discoveredServers[info.serverId] = info;

            Debug.Log("there are" + discoveredServers.Count);
            
            Connect(info);
            Debug.Log($"Connected to {info.EndPoint.Address.ToString()}");
        }

        void Connect(ServerResponse info)
        {
            networkDiscovery.StopDiscovery();
            networkManager.StartClient(info.uri);
        }
    }

}