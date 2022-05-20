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
using Mirror.FizzySteam;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prototype1
{
    public class ZFNetworkManager : NetworkManager
    {

        #region Events
        public static event Action OnClientDisconnected;
        public static event Action OnClientConnected;
        public static event Action OnServerStart;

        #endregion



        ///<summary>Called once on the frame before Update</summary>
        public override void Start(){
            base.Start();
            
            Menu.OnChooseKcpTransport += HandleChoosingKcpTransport;
            Menu.OnChooseFizzySteamWorksTransport += HandleChoosingFizzySteamWorksTransport;

        }

        ///<summary>Executed when the object is destroyed</summary>
        public override void OnDestroy(){

            Menu.OnChooseKcpTransport -= HandleChoosingKcpTransport;
            Menu.OnChooseFizzySteamWorksTransport -= HandleChoosingFizzySteamWorksTransport;

        }
        
        ///<summary>Reinitialize the network manager after enabling Kcp Transport</summary>
        private void HandleChoosingKcpTransport(){
            Shutdown();
            transport = GetComponent<KcpTransport>();
        }

        ///<summary>Reinitialize the network manager after enabling FizzySteamworks Transport</summary>
        private void HandleChoosingFizzySteamWorksTransport(){
            Shutdown();
            transport = GetComponent<FizzySteamworks>();
        }

        
        public override void OnClientDisconnect(NetworkConnection conn){
            base.OnClientDisconnect(conn);

            OnClientDisconnected?.Invoke();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnClientConnected?.Invoke();
        }

        public override void OnStartServer()
        {            
            startPositions.Clear();

            OnServerStart?.Invoke();
            
        }



    }

}