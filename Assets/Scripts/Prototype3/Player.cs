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
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype3
{
    public class Player : NetworkBehaviour
    {

        #region Events

        public static event Action<Team> OnPlayerKilled;

        public static event Action<GameObject,GameObject> OnTreePlanted;
        public static event Action OnTreeCutDown;
        
        public static event Action OnTryPlantTree;
        public static event Action OnTryCutDownTree;
        public static event Action OnCancelPlantOrCutTree;

        #endregion
        
        #region Subscribing / Unsubscribing to events

        
        public override void OnStartServer(){

            // #region Subscribing to RoundSystem events
            RoundSystem.OnRoundWin += ReturnToBase;
            // RoundSystem.OnTeamSwapTimerEnd += ReturnToBase;
            // #endregion

            #region Subscribing to NetworkManager events
            RoundSystem.OnMatchStarted += ReturnToBase;
            #endregion

            #region  Subscribing to PlantTimer events
            PlantTimer.OnPlantTimeElapsed += ReturnToBase;
            #endregion
        }

        [ServerCallback]
        private void OnDestroy(){
            
            // #region Subscribing to RoundSystem events
            RoundSystem.OnRoundWin -= ReturnToBase;
            // RoundSystem.OnTeamSwapTimerEnd -= ReturnToBase;
            // #endregion

            #region UnSubscribing to NetworkManager events
            RoundSystem.OnMatchStarted -= ReturnToBase;
            #endregion

            #region  UnSubscribing to PlantTimer events
            PlantTimer.OnPlantTimeElapsed -= ReturnToBase;
            #endregion
        }
        private void OnEnable()
        {

            #region Subscribing to ConfigMenu events
            ConfigMenu3.OnPlayerNameChange += HandlePlayerNameChange;
            #endregion

            #region Subscribing to RoundSystem events
            RoundSystem.OnRoundWin += ReturnToBase;
            RoundSystem.OnTeamSwapTimerEnd += ReturnToBase;
            #endregion

            #region Subscribing to ProgressBar events
            ProgressBar.OnProgressTreeCutComplete += CutDownTree;
            ProgressBar.OnProgressTreePlantComplete += PlantTree;

            #endregion

            #region Subscribing to Tree events
            OnTreeCutDown += ReturnToBase;
            #endregion
            
        }
        private void OnDisable()
        {
            #region Unsubscribing to ConfigMenu events
            ConfigMenu3.OnPlayerNameChange -= HandlePlayerNameChange;
            #endregion
         
            
            #region UnSubscribing to RoundSystem events
            RoundSystem.OnRoundWin -= ReturnToBase;
            RoundSystem.OnTeamSwapTimerEnd -= ReturnToBase;

            #endregion

            #region UnSubscribing to ProgressBar events
            ProgressBar.OnProgressTreeCutComplete -= CutDownTree;
            ProgressBar.OnProgressTreePlantComplete -= PlantTree;
            #endregion

            #region Subscribing to Tree events
            OnTreeCutDown -= ReturnToBase;
            #endregion
        }
        
        #endregion

        #region Attributes

            #region Player Attributes
        [SerializeField] private Material[] characterColors;

        [SerializeField] private Renderer characterColorRenderer;

        [SyncVar]
        private string playerName;


        [SerializeField]
        [SyncVar(hook = nameof(OnPlayerTeamChanged))]
        private Team team;

        public Team Team_ { get { return team; } }
        public int Kills_ { get { return kills; } }
        public int Deaths_ { get { return deaths; } }

        private static int HP_MAX = 100;

        [SyncVar(hook = nameof(OnPlayerHpChanged))]
        [SerializeField]
        private int hp = HP_MAX;

        [SerializeField] [SyncVar] private int kills = 0;
        [SerializeField] [SyncVar] private int deaths = 0;

        //new code 
        [SyncVar(hook = nameof(OnPlayerPositionChanged))]
        private Vector3 position;

        [SyncVar(hook = nameof(OnPlayerRotationChanged))]
        private Quaternion rotation;

        //

        [SerializeField]
        public bool IsDead
        {
            get
            {
                if (hp == 0) return true;
                else return false;
            }
        }

        //scoreboard

        [SerializeField]
        private GameObject scoreTab;

        [SerializeField]
        List<PlayerInfo> playersInfoList;

        [SerializeField]
        PlayerInfo line;

        //Healthbar
        GameObject healthBar;

        [SerializeField]
        Slider slider;
        [SerializeField]
        TMP_Text hpNumber;
        [SerializeField]
        Image fill;
        [SerializeField]
        Gradient gradient;

            #endregion

            #region Network Manager singleton reference

        private ZFNetworkManager3 gameNetworkManager;

        private ZFNetworkManager3 GameNetworkManager
        {
            get
            {
                if (gameNetworkManager != null) return gameNetworkManager;
                return gameNetworkManager = NetworkManager.singleton as ZFNetworkManager3;
            }
        }
        
            #endregion

            #region Tree Attributes
        [SerializeField] private bool canPlant = false;

        [SerializeField] private bool canCutdown = false;

        [SerializeField] private PlantingZone CurrentPlantingZone = null;

        #endregion

            #region Spectator mode attributes
        [SerializeField]
        List<Player> teamAlivePlayers;

        int nextTeamAlivePlayerIndex = 0;

        private bool isAlreadySpectating = false;

        private Player lastTeammateViewed = null;
            #endregion

        #endregion

        #region Server

            #region Player's server code


        [Server]
        public void SwitchTeam()
        {
            if (team == Team.Fellag)
                team = Team.Zellag;
            
            else
                team = Team.Fellag;
            
        }

        [Server]
        public void SetTeam()
        {
            if (GameNetworkManager.Fellags.Count <= GameNetworkManager.Zellags.Count){

                team = Team.Fellag;

                GameNetworkManager.Fellags.Add(this);

                RpcUpdateClientsNetManagerFellagsListOnAddPlayer(
                    GameNetworkManager.Fellags
                );

                TargetUpdateNewClientNetManagerZellagsListOnAddPlayer(
                    connectionToClient,
                    GameNetworkManager.Zellags
                );

            }
            else{

                team = Team.Zellag;
                
                GameNetworkManager.Zellags.Add(this);
                
                RpcUpdateClientsNetManagerZellagsListOnAddPlayer(
                    GameNetworkManager.Zellags
                );

                TargetUpdateNewClientNetManagerFellagsListOnAddPlayer(
                    connectionToClient,
                    GameNetworkManager.Fellags
                );

            }

            SetPlayerName();
        }


        [Server]
        public void SetPlayerName()
        {

            TargetSetName(connectionToClient);
        }

        [Command]

        private void CmdSetName(string playerName){

            int numOfTeamPlayers;
        
            switch (team)
            {
                case Team.Fellag:

                    if(playerName != ""){
                        SetName(playerName);
                        
                    }
                    else{

                        numOfTeamPlayers = GameNetworkManager.Fellags.Count;
                        SetName($"Fellag {numOfTeamPlayers}");
                       
                    }

          
                    
                    break;
                case Team.Zellag:
                    
                    if(playerName != ""){
                        SetName(playerName);

                    }
                    else{

                        numOfTeamPlayers = GameNetworkManager.Zellags.Count;
                        SetName($"Zellag {numOfTeamPlayers}");
                    }

         
                    break;
            }
        }

        [Server]
        private void SetName(string playerName)
        {
            this.playerName = playerName;
            name = playerName;
        }

        public string Name{

            get {
                return playerName;
            }
        }


        [Server]
        public void SetDamageBy(Player killer, int amount)
        {

            if (hp == 0)
                return;

            hp -= amount;

            // TargetSetUIhealth();

            if (hp == 0)
            {

                deaths++;

                RpcHideDeadPlayer();

                killer.UpdateKills();

                OnPlayerKilled?.Invoke(team);
                Debug.Log("killed");
            }

        }

        [Server]
        public void UpdateKills()
        {
            kills++;
        }

        [Server]
        public void ResetHp()
        {

            Debug.Log("hp reset!");
            hp = HP_MAX;

            TargetResetHealthBar();
            
        }

        [Command]
        public void CmdHideDeadPlayers(NetworkConnectionToClient clientConn = null){
            
            if(GameNetworkManager.DeadFellags.Count > 0)
                foreach (Player fellag in GameNetworkManager.DeadFellags)
                    fellag.TargetHideDeadPlayerFor(clientConn);
                
            if(GameNetworkManager.DeadZellags.Count > 0)
                foreach (Player zellag in GameNetworkManager.DeadZellags)
                    zellag.TargetHideDeadPlayerFor(clientConn);

        }

        [Server]
        private void ReturnToBase(){
            
            if(IsDead){
                ResetHp();
                RpcUnHideDeadPlayer();
            }

        

            if(GameNetworkManager.nextFellagSpawnPointIndex > 5) GameNetworkManager.nextFellagSpawnPointIndex = 0;
            if(GameNetworkManager.nextZellagSpawnPointIndex > 5) GameNetworkManager.nextZellagSpawnPointIndex = 0;

   
            switch (team)
            {
                case Team.Fellag:

                    // transform.position = 
                    //     GameNetworkManager.spawnLocationsF[GameNetworkManager.nextFellagSpawnPointIndex].
                    //     transform.position;  
                    position = 
                        GameNetworkManager.spawnLocationsF[GameNetworkManager.nextFellagSpawnPointIndex].
                        transform.position;  

                    // transform.rotation = GameNetworkManager.spawnLocationsF[GameNetworkManager.nextFellagSpawnPointIndex].
                    //     transform.rotation;

                    rotation = GameNetworkManager.spawnLocationsF[GameNetworkManager.nextFellagSpawnPointIndex].
                        transform.rotation;

                    // RpcReturnToBase(transform.position,transform.rotation);

                        GameNetworkManager.nextFellagSpawnPointIndex++;
                    break;

                case Team.Zellag:

                    transform.position = 
                        GameNetworkManager.spawnLocationsZ[GameNetworkManager.nextZellagSpawnPointIndex].
                        transform.position;

                    transform.rotation = GameNetworkManager.spawnLocationsZ[GameNetworkManager.nextZellagSpawnPointIndex].
                        transform.rotation;
                        
                    RpcReturnToBase(transform.position,transform.rotation);

                    GameNetworkManager.nextZellagSpawnPointIndex++;

                    break;

            }
                            
           
        }


            #endregion

            #region Player Planting Tree Server Code

       
        [Command]
        private void CmdPlantTree()
        {

            GameObject treeGameObject = Instantiate(GameNetworkManager.TreePrefab);
            NetworkServer.Spawn(treeGameObject);

            RpcPlantTree(treeGameObject);

            OnTreePlanted?.Invoke(treeGameObject,CurrentPlantingZone.gameObject);

        }


        [Command]
        private void CmdCutDownTree()
        {
            try
            {
                Tree tree = CurrentPlantingZone.GetComponentInChildren<Tree>();

                NetworkServer.Destroy(tree.gameObject);

            }
            catch (System.Exception)
            {

                Debug.Log("The Tree is not in this Zone");
            }

            RpcCutDownTree();

            OnTreeCutDown?.Invoke();
        }


        [Command]
        public void CmdRefreshPlantedTree(NetworkConnectionToClient newClient = null){
            
            if(RoundSystem.singleton.IsTreePlanted)
                TargetRefreshPlantedTree(
                    newClient,
                    RoundSystem.singleton.PlantZone
                );
        }


        #endregion

         

        #endregion

        #region Client

            #region init
            private void Awake(){
                
                scoreTab = GameObject.FindGameObjectWithTag("PlayerUI")
                            .GetComponentInChildren<ScoreTab>(true).gameObject;

                line = GameObject.FindGameObjectWithTag("PlayerUI").
                        GetComponentInChildren<PlayerInfo>(true);


                healthBar = GameObject.FindGameObjectWithTag("SliderHp");
                slider = healthBar.GetComponent<Slider>();
                fill = healthBar.GetComponentInChildren<Image>();
                hpNumber = healthBar.GetComponentInChildren<TMP_Text>();
                
            }

            public override void OnStartLocalPlayer(){

                if(!hasAuthority) return;

                CmdRefreshPlantedTree();

                CmdHideDeadPlayers();
            }

            #endregion

            #region Player's name client code

        [TargetRpc]

        private void TargetSetName(NetworkConnection clientConn){

            string playerPrefName = "";

            if(PlayerPrefs.HasKey("playerName"))
               playerPrefName = PlayerPrefs.GetString("playerName");
            
            CmdSetName(playerPrefName);
        
        }
            #endregion

            #region Player client's base relative code

        [ClientRpc]
        private void RpcReturnToBase(Vector3 spawnLocationPostion,Quaternion spawnLocationRotation){
            transform.position = spawnLocationPostion;
            transform.rotation = spawnLocationRotation;
        }
            #endregion


            #region Player Planting Tree Client Code


            [TargetRpc]
            public void TargetRefreshPlantedTree(NetworkConnection connToNewClient,
                 GameObject plantZone)
            {

                RoundSystem.singleton.PlantedTree.transform.parent = 
                plantZone.transform;

                RoundSystem.singleton.PlantedTree.transform.localPosition = Vector3.zero;

                RoundSystem.singleton.PlantedTree.transform.localRotation = Quaternion.identity;
            
            }

      

            [Client]
            private void PlantTree()
            {
                if (!hasAuthority) return;
                    CmdPlantTree();
            } 

            [ClientRpc]
            private void RpcPlantTree(GameObject tree)
            {

                tree.transform.parent = CurrentPlantingZone.transform;

                tree.transform.localPosition = Vector3.zero;

                tree.transform.localRotation = Quaternion.identity;

            }


            [Client]
            private void CutDownTree()
            {
                if (!hasAuthority) return;
                    CmdCutDownTree();
            }

            [ClientRpc]
            private void RpcCutDownTree()
            {
                try
                {
                    Tree tree = CurrentPlantingZone.GetComponentInChildren<Tree>();

                    NetworkServer.Destroy(tree.gameObject);


                }
                catch (System.Exception)
                {

                    Debug.Log("The Tree is not in this Zone");
                }

            }
            #endregion

            #region Updating Player's respective Network manager players list

            [ClientRpc]
            public void RpcUpdateClientsNetManagerFellagsListOnAddPlayer(List<Player> fellags){
               
                GameNetworkManager.Fellags = fellags;
            }

            [ClientRpc(includeOwner = false)]

            public void RpcUpdateClientsNetManagerFellagsListOnDisconnect(List<Player> fellags){
                GameNetworkManager.Fellags = fellags;
            }

            [TargetRpc]

            public void TargetUpdateNewClientNetManagerFellagsListOnAddPlayer(
                NetworkConnection target,List<Player> fellags){
                GameNetworkManager.Fellags = fellags;
            }

            [ClientRpc]
            public void RpcUpdateClientsNetManagerZellagsListOnAddPlayer(List<Player> zellags){
               
                GameNetworkManager.Zellags = zellags;

            }

            [ClientRpc(includeOwner = false)]
        
            public void RpcUpdateClientsNetManagerZellagsListOnDisconnect(List<Player> zellags){
                GameNetworkManager.Zellags = zellags;
            }

            [TargetRpc]

            public void TargetUpdateNewClientNetManagerZellagsListOnAddPlayer(
                NetworkConnection target,List<Player> zellags){
                GameNetworkManager.Zellags = zellags;
            }

            #endregion

            #region Planting Tree Client code


        public void AuthorizePlantIn(GameObject plantingZoneObj)
        {
            CurrentPlantingZone = plantingZoneObj.GetComponent<PlantingZone>();

            if (!RoundSystem.singleton.IsTreePlanted)
                canPlant = true;
        }

        public void AuthorizeCutDownIn(GameObject plantingZoneObj)
        {
            CurrentPlantingZone = plantingZoneObj.GetComponent<PlantingZone>();
            if (RoundSystem.singleton.IsTreePlanted)
                canCutdown = true;
        }

        public void UnauthorizePlant()
        {
            if (team == Team.Fellag)
                canPlant = false;

            CurrentPlantingZone = null;
        }


        public void UnauthorizeCutDown()
        {
            if (team == Team.Zellag)
                canCutdown = false;

            CurrentPlantingZone = null;
        }


            #endregion
           
            #region Player's Spectator mode

            private void Spectate()
            {

            if(isAlreadySpectating && !Input.GetButtonDown("Fire1")){

                transform.position = 
                    lastTeammateViewed.
                    transform.position;

                transform.rotation = 
                    lastTeammateViewed.
                    transform.rotation;

                GetComponent<View3>().playerCamera.localRotation =
                        lastTeammateViewed.GetComponent<View3>().
                        playerCamera.localRotation;
            

            }
            else{
                //player !isAlreadySpectating || Input.GetButtonDown("Fire1")
                
                switch (team)
                {
                    case Team.Fellag:
                        teamAlivePlayers = GameNetworkManager.AliveFellags;
                        break;
                    case Team.Zellag:
                        teamAlivePlayers = GameNetworkManager.AliveZellags;
                        break;
                }
                
                nextTeamAlivePlayerIndex++;
            
                if(nextTeamAlivePlayerIndex >= teamAlivePlayers.Count)
                    nextTeamAlivePlayerIndex = 0;

                if(teamAlivePlayers.Count != 0){

                    lastTeammateViewed = 
                        teamAlivePlayers[nextTeamAlivePlayerIndex];

                    transform.position = 
                        lastTeammateViewed.
                        transform.position;

                    transform.rotation = 
                        lastTeammateViewed.
                        transform.rotation;
                    
                    GetComponent<View3>().playerCamera.localRotation =
                        lastTeammateViewed.GetComponent<View3>().playerCamera.localRotation;
            
                    isAlreadySpectating = true;
                }

            
            }
        
        }

        //hiding dead players for new clients
        
        [TargetRpc]
        public void TargetHideDeadPlayerFor(NetworkConnection clientConn){
            GetComponent<CharacterController>().enabled = false;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        
        [ClientRpc]
        void RpcHideDeadPlayer()
        {
            
            GetComponent<CharacterController>().enabled = false;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;

        }


        [ClientRpc]
        public void RpcUnHideDeadPlayer()
        {
            GetComponent<CharacterController>().enabled = true;
            
        
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            
            
            GetComponentInChildren<MeshRenderer>().enabled = true;
            
        }
            #endregion

            #region Players's healthbar
       [TargetRpc]
        void TargetSetUIhealth()
        {
            slider.value = hp;
            hpNumber.text = hp.ToString();
            fill.color = gradient.Evaluate(slider.normalizedValue);
            
        }


        [TargetRpc]
        public void TargetResetHealthBar()
        {
            Debug.Log("Reset health UI");

            slider.value = HP_MAX;
            hpNumber.text = HP_MAX.ToString();
            fill.color = gradient.Evaluate(1f);
        }
            #endregion

            #region Player's Scoreboard
        [Client]
        private void AddPlayerToScoreBoard(){

            foreach (var item in playersInfoList)
            {
                Destroy(item.gameObject);
            }

            playersInfoList.Clear();

            foreach (var player in GameNetworkManager.Fellags)
            {
                PlayerInfo playerInfo = Instantiate(line, line.transform.parent);

                playerInfo.SetInfo(player.Name,
                player.Team_,
                player.Kills_,
                player.Deaths_);

                playerInfo.gameObject.SetActive(true);
                playersInfoList.Add(playerInfo);
            }

            foreach (var player in GameNetworkManager.Zellags)
            {
                PlayerInfo playerInfo = Instantiate(line, line.transform.parent);

                playerInfo.SetInfo(player.Name,
                player.Team_,
                player.Kills_,
                player.Deaths_);

                playerInfo.gameObject.SetActive(true);
                playersInfoList.Add(playerInfo);
            }
        }
            #endregion

            #region Player' name change

        [Client]
        private void HandlePlayerNameChange(string newName){
            
            if(!hasAuthority && !isLocalPlayer) return;

            CmdSetName(newName);
        }

        #endregion 

        #region Update loop


        private void Update()
        {

            if (!hasAuthority) return;

            //PLANT OR CUT THE TREE

            if(RoundSystem.singleton.gameState == GameState.InProgress){


                if (Input.GetKeyDown(KeyCode.E))
                {

                    if (canPlant)
                        OnTryPlantTree?.Invoke();

                    if (canCutdown)
                        OnTryCutDownTree?.Invoke();

                }
                //CANCEL PLANT OR CUT PROGRESS
                if (Input.GetKeyUp(KeyCode.E))
                {
                    OnCancelPlantOrCutTree?.Invoke();
                }

            }


            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Tab pressed");
                AddPlayerToScoreBoard();
                scoreTab.SetActive(true);
                
            }
            
            if(Input.GetKeyUp(KeyCode.Tab))
            {
                Debug.Log("Tab released ");
                scoreTab.SetActive(false);

            }

            if(IsDead)
                Spectate();
        }


    

        #endregion

            #region Sync vars hooks

        private void OnPlayerTeamChanged(Team oldTeam, Team newTeam)
        {
            canCutdown = false;
            canPlant = false;

            switch (newTeam)
            {
                case Team.Fellag:
                    characterColorRenderer.material = characterColors[0];
                    break;
                case Team.Zellag:
                    characterColorRenderer.material = characterColors[1];
                    break;
            }

        }


        private void OnPlayerHpChanged(int oldHp, int newHp)
        {
            if(!isLocalPlayer) return;

            Debug.Log($"{playerName} -------- Health points changed to {newHp}hp");

            slider.value = hp;
            hpNumber.text = hp.ToString();
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        private void OnPlayerPositionChanged(Vector3 oldPosition, Vector3 newPosition){
            transform.position = newPosition;
        }
    
        private void OnPlayerRotationChanged(Quaternion oldRotation, Quaternion newRotation){
            transform.rotation = newRotation;
        }

        #endregion

            #region Reset Player's data

            
        public override void OnStopClient(){
            ResetPlayerData();
        }

        //To restart new match on disconnect
        private void ResetPlayerData()
        {
            kills = 0;
            deaths = 0;
            hp = HP_MAX;

            slider.value = HP_MAX;
            hpNumber.text = HP_MAX.ToString();
            fill.color = gradient.Evaluate(1f);

            canPlant = false;
            canCutdown = false;

            isAlreadySpectating = false;

            //Reset the scoreboard

            foreach (var playerInfo in playersInfoList)
            {
                Destroy(playerInfo.gameObject);
            }

            playersInfoList.Clear();

        }
            
            
            
            #endregion
      
        #endregion

    }
}
