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
using Prototype2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prototype3
{
    public class ZFNetworkManager3 : ZFNetworkManager2
    {

        #region Events
        
        public static event Action OnMatchStarted;

        #endregion
       
        #region Attributes

            #region Match atttibutes

                #region Players Attributes
        public List<Player> Fellags = new List<Player>();

        public List<Player> DeadFellags {
            get
            {
                return Fellags.FindAll(x => x.IsDead);
            }
        }
        public List<Player> AliveFellags {
            get{
                return Fellags.FindAll(x => !x.IsDead);
            }
        }
        
        public List<Player> Zellags = new List<Player>();

        public List<Player> DeadZellags
        {
            get
            {
                return Zellags.FindAll(x => x.IsDead);
            }
        }
               
        public List<Player> AliveZellags {
            get{
                return Zellags.FindAll(x => !x.IsDead);
            }
        }
              
        
        //
        public GameObject[] spawnLocationsZ;
        public GameObject[] spawnLocationsF;
        
        [SerializeField]
        public int nextFellagSpawnPointIndex = 0;

        [SerializeField]

        public int nextZellagSpawnPointIndex = 0;

        Transform startPos;

                #endregion
               
                #region Tree attributes
        public GameObject TreePrefab
        {
            get
            {
                GameObject tp = null;

                foreach (var spawnPrefab in spawnPrefabs)
                {
                    if (spawnPrefab.GetComponent<Tree>())
                    {
                        tp = spawnPrefab;
                        break;
                    }
                }

                return tp;
            }
        }


        #endregion

        #endregion

        #endregion

 


     
        #region Server


        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
                conn.Disconnect();
        }
        
 
        public override void OnServerDisconnect(NetworkConnection conn)
        {            
            Debug.Log("OnServer Disconnect");

            NetworkIdentity disconnectedPlayerId = conn.identity;

            playersId.Remove(disconnectedPlayerId);

            Player disconnectedPlayer = disconnectedPlayerId.GetComponent<Player>();

            base.OnServerDisconnect(conn);

            switch (disconnectedPlayer.Team_)
            {
                case Team.Fellag:
                    Fellags.Remove(disconnectedPlayer);
                    disconnectedPlayer.RpcUpdateClientsNetManagerFellagsListOnDisconnect(Fellags);
                    break;
                case Team.Zellag:
                    Zellags.Remove(disconnectedPlayer);
                    disconnectedPlayer.RpcUpdateClientsNetManagerZellagsListOnDisconnect(Zellags);
                    break;
            }

        }


        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            AddPlayerToSpawnPoint(conn);

            if (numPlayers == 2)
            {
                //start round
                // Rounds.Add(currentRound);
                Debug.Log($"new player is ready = {conn.isReady}");
                
                OnMatchStarted?.Invoke();
                
                Debug.Log("Round one started !");

            }


        }

        public void AddPlayerToSpawnPoint(NetworkConnection newPlayerConnection)
        {

            if(nextFellagSpawnPointIndex > 5) nextFellagSpawnPointIndex = 0;

            if(nextZellagSpawnPointIndex > 5) nextZellagSpawnPointIndex = 0;

            

            if(Fellags.Count <= Zellags.Count){
                      
                    startPos = spawnLocationsF[nextFellagSpawnPointIndex].transform;
                    nextFellagSpawnPointIndex++;

            }
            else{

                    startPos = spawnLocationsZ[nextZellagSpawnPointIndex].transform;
                    nextZellagSpawnPointIndex++;
   
            }
                

            GameObject playerObj = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

                
            Debug.Log(startPos);

            NetworkServer.AddPlayerForConnection(newPlayerConnection, playerObj);

            Player newPlayer = newPlayerConnection.identity.GetComponent<Player>();

            newPlayer.SetTeam();

            playersId.Add(newPlayerConnection.identity);

        }
        public override void OnStopServer()
        {
            playersId.Clear();
            Fellags.Clear();
            Zellags.Clear();
        }

   

        #endregion

        #region Client
        public override void OnStopClient()
        {
            playersId.Clear();
            Fellags.Clear();
            Zellags.Clear();

        }

        #endregion
    }
}
