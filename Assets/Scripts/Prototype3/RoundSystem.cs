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
using Prototype3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Prototype3
{
    public class RoundSystem : NetworkBehaviour
    {

        #region Events

        public static event Action OnMatchStarted;
        public static event Action OnRoundWin;
        public static event Action OnRoundEnd;
        public static event Action OnTreePlantedStartTimer;
        public static event Action OnTeamSwapTimerStart;
        public static event Action OnTeamSwapTimerEnd;
        public static event Action OnUnPauseRoundTimer;

        #endregion

        #region Subscribing / Unsubscribing to event

        [ServerCallback]
        public override void OnStartServer()
        {
            
            #region Match reset server code
            ResetMatch();
            #endregion

            #region Subscribing to Gestionnaire du Match 3 events
            ZFNetworkManager3.OnMatchStarted += HandleMatchStart;
            #endregion

            RoundTimer.OnRoundTimeElapsed += HandleRoundTimeElapsed;
            PlantTimer.OnPlantTimeElapsed += HandlePlantTimeElapsed;
            Player.OnPlayerKilled += HandlePlayerKilled;

            Player.OnTreePlanted += HandleTreePlanted;
            Player.OnTreeCutDown += HandleTreeCutDown;
        }

        [ServerCallback]
        private void OnDestroy()
        {
           
            ZFNetworkManager3.OnMatchStarted -= HandleMatchStart;
            RoundTimer.OnRoundTimeElapsed -= HandleRoundTimeElapsed;
            PlantTimer.OnPlantTimeElapsed -= HandlePlantTimeElapsed;

            Player.OnPlayerKilled -= HandlePlayerKilled;

            Player.OnTreePlanted -= HandleTreePlanted;
            Player.OnTreeCutDown -= HandleTreeCutDown;
        }

        #endregion

        #region Attributes
        
            #region Player UI attributes
        
        [SerializeField]
        TMP_Text ZellagsScore;
        [SerializeField]
        TMP_Text FellagsScore;
        [SerializeField]
        TMP_Text TeamRoundWinTxt;

        #endregion

            #region Gameover UI attributes
        [SerializeField]
        Canvas GameoverUI;
        [SerializeField]
        GameObject victory;
        [SerializeField]
        GameObject tie;
        [SerializeField]
        GameObject defeat;

            #endregion

            #region Network Manager attributes
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

            #region Round attributes
        public int teamSwapTimer = 5;
        private int  roundWinnerTxtDisplayTime = 2;
        
        [SyncVar]
        public  GameState gameState = GameState.WaitingForPlayers;
        
        [SyncVar]
        public int maxRounds = 10;
        [SyncVar]
        public int roundNumber = 0;

        [SyncVar]
        public bool isAllFellagsDead = false;
        [SyncVar]
        public bool isAllZellagsDead = false;
        
        [SyncVar(hook = nameof(UpdateFellagsScore))]
        public int roundsWonByFellags = 0;

        [SyncVar(hook = nameof(UpdateZellagsScore))]
        public int roundsWonByZellags = 0;
        [SyncVar][SerializeField]
        private bool isTreePlanted = false;
        
        
        public bool IsTreePlanted  {
            get{ 
                return isTreePlanted;
            }           
        } 

        [SyncVar]
        public GameObject PlantZone;

        [SyncVar]
        public GameObject PlantedTree;
            #endregion

        #endregion

        #region init
        public static RoundSystem singleton { get; private set; } = null;

        private RoundSystem(){}
        
        private void Awake(){
            
            if(singleton != null && singleton != this)
                Destroy(gameObject);
            
            singleton = this;

            maxRounds = 10;
        }
        #endregion


        #region Server


            #region Match server code

        [Server]
        private void HandleMatchStart()
        {

            StartCoroutine(DisplayMatchStartCoroutine(3));

            gameState = GameState.InProgress;

        }

        IEnumerator<WaitForSeconds> DisplayMatchStartCoroutine(int secondsToDisplay){
            
            RpcDisplayMatchStartTxt();

            yield return new WaitForSeconds(secondsToDisplay);

            RpcResetMatchStartTxt();

            OnMatchStarted?.Invoke();
        }

        [Server]
        private void CheckForMatchEnd(){

            //switch gamestate
            if(maxRounds == (roundsWonByFellags + roundsWonByZellags))
                gameState = GameState.Tie;
            
            if(roundsWonByFellags > maxRounds / 2)
                gameState = GameState.MatchWonByFellags;

            if(roundsWonByZellags > maxRounds / 2)
                gameState = GameState.MatchWonByZellags;


            // TeamSwap:

            switch (gameState)
            {
                case GameState.RoundWonByFellags:
                
                    DisplayRoundWinnerTxt(roundWinnerTxtDisplayTime,Team.Fellag);

                    break;
                
                case GameState.RoundWonByZellags:

                    DisplayRoundWinnerTxt(roundWinnerTxtDisplayTime,Team.Zellag);

                    break;

                case GameState.Tie:
                    DisplayTieScreen();
                    break;

                case GameState.MatchWonByFellags:
                    DisplayVictoryScreen(Team.Fellag);
                    DisplayDefeatScreen(Team.Zellag);
                    break;

                case GameState.MatchWonByZellags:
                    DisplayVictoryScreen(Team.Zellag);
                    DisplayDefeatScreen(Team.Fellag);
                    break;
                

            }


        }


            #endregion

            #region Tree Plant/ CutDown  server code
        [Server]
        private void HandleTreePlanted(GameObject tree, GameObject plantZone)
        {
            isTreePlanted = true;
            PlantedTree = tree;
            PlantZone = plantZone;
            OnTreePlantedStartTimer?.Invoke();
        }

        [Server]
        private void HandlePlantTimeElapsed()
        {
            Debug.Log("PlantTimerElapsed");

            if(gameState == GameState.InProgress){

                if(IsTreePlanted){
                

                    gameState = GameState.RoundWonByFellags;
                    roundsWonByFellags++;
                    roundNumber++;
                    CutDownTree();
                    
                }

            }

            CheckForMatchEnd();
        } 

        [Server]
        private void CutDownTree()
        {
            if(PlantedTree != null) NetworkServer.Destroy(PlantedTree);
            else Debug.Log("tree is null");
            isTreePlanted = false;
        }

        [Server]
        private void HandleTreeCutDown()
        {
            Debug.Log("Tree cut down");
            isTreePlanted = false;
            
            if(gameState == GameState.InProgress){
                isTreePlanted = false;

                gameState = GameState.RoundWonByZellags;
                roundsWonByZellags++;
                roundNumber++;
            }

            CheckForMatchEnd();

        }
            #endregion

            #region Teams swap server code
        [Server]
        private bool IsTimeToTeamSwap(){

            if(roundsWonByZellags + roundsWonByFellags == (maxRounds / 2) )
                gameState = GameState.TeamSwap;

            if(gameState == GameState.TeamSwap) return true;
            else return false;

        }

        [Server]
        private void SetupTeamsSwap(){

            SwitchTeam();

            SwitchTeamScore();

            gameState = GameState.InProgress;
        }

        [Server]
        private void SwitchTeam(){

            foreach (Player fellag in GameNetworkManager.Fellags)
                fellag.SwitchTeam();
        
            foreach (Player zellag in GameNetworkManager.Zellags)
                zellag.SwitchTeam();

            var t = GameNetworkManager.Fellags;
            GameNetworkManager.Fellags = GameNetworkManager.Zellags;
            GameNetworkManager.Zellags = t;

            RpcUpdateClientsNetManagerPlayersList(GameNetworkManager.Fellags,
            GameNetworkManager.Zellags);

        }



        [Server]
        void SwitchTeamScore()
        {
            int t;

            t = roundsWonByZellags;
            roundsWonByZellags = roundsWonByFellags;
            roundsWonByFellags = t;

        }
            #endregion
       
            #region Killing player server code
        
        [Server]
        private void HandlePlayerKilled(Team killedPlayerTeam)
        {

            switch (killedPlayerTeam)
            {
                case Team.Fellag:
                //Zellags winning logic

                    isAllFellagsDead = 
                        GameNetworkManager.DeadFellags.Count == GameNetworkManager.Fellags.Count
                        &&
                        GameNetworkManager.Fellags.Count > 0;

                    if(isAllFellagsDead) {
                        
                        Debug.Log("All Fellags Dead");

                        if(!IsTreePlanted){
                       
                            gameState = GameState.RoundWonByZellags;

                            roundNumber++;

                            roundsWonByZellags++;

                        }

                    }else
                        gameState = GameState.InProgress;



                    break;

                case Team.Zellag:
                    //Fellags winning logic
                    
                    isAllZellagsDead = 

                        GameNetworkManager.DeadZellags.Count == GameNetworkManager.Zellags.Count
                        &&
                        GameNetworkManager.Zellags.Count > 0;

                    
                    if(isAllZellagsDead) {
                        
                        Debug.Log("All Zellags Dead");

                        if(!IsTreePlanted){

                            gameState = GameState.RoundWonByFellags;

                            roundNumber++;

                            roundsWonByFellags++;

                        }
                        
                    }else
                        gameState = GameState.InProgress;
                                       
                    break;
            }

            CheckForMatchEnd();

        }

            #endregion
        
            #region Round server code
        [Server]
        private void HandleRoundTimeElapsed()
        {
            Debug.Log("RoundTimeElapsed");

            if(gameState == GameState.InProgress){

                if(IsTreePlanted){
                    //Tree is planted
                    //Fellags win the round
                    gameState = GameState.RoundWonByFellags;
                    roundsWonByZellags++;
                    roundNumber++;
                    CutDownTree();

                }else{
                    //Tree is not planted
                    //Zellags win the round
                    gameState = GameState.RoundWonByZellags;
                    roundsWonByZellags++;
                    roundNumber++;
                }
                
            }

            CheckForMatchEnd();
        }

        [Server]
        private void DisplayRoundWinnerTxt(int secondsToDisplay,Team team){
            StartCoroutine(RoudWinnerTxtDisplayCoroutine(secondsToDisplay,team));
        }

        [Server]
        IEnumerator<WaitForSecondsRealtime> RoudWinnerTxtDisplayCoroutine(int roundWinnerTxtDisplayTime, Team team){
            
            OnRoundEnd?.Invoke();

            RpcDisplayRoundWinnerTxt(team);

            yield return new WaitForSecondsRealtime(roundWinnerTxtDisplayTime);


            if(IsTimeToTeamSwap()) {
                //Display switching team text on RoundTimer
                OnTeamSwapTimerStart?.Invoke();

                yield return new WaitForSecondsRealtime(teamSwapTimer);
                SetupTeamsSwap();
                OnTeamSwapTimerEnd?.Invoke();
                OnUnPauseRoundTimer?.Invoke();

            }else{

                OnRoundWin?.Invoke();

            }
            
            gameState = GameState.InProgress;

        }
            #endregion
       
      
        
        #endregion


        #region Client

            #region Round's client code

        [ClientRpc]
        private void RpcDisplayRoundWinnerTxt(Team team)
        {

            StartCoroutine(RoundWinnerTxtDisplayClientCoroutine(roundWinnerTxtDisplayTime,team));
            
        }

        [Client]
        IEnumerator<WaitForSeconds> RoundWinnerTxtDisplayClientCoroutine(int displayTime,Team team){

            switch (team)
            {
                case Team.Fellag:
                    TeamRoundWinTxt.text = "Les Fellags gagnent le round";
                    TeamRoundWinTxt.color = Color.green;
                    break;
                case Team.Zellag:
                    TeamRoundWinTxt.text = "Les Zellags gagnent le round";
                    TeamRoundWinTxt.color = Color.red;
                    break;

            }
            yield return new WaitForSeconds(displayTime);

            TeamRoundWinTxt.text = "";
        }   

            #endregion

            #region GameOver UI's client code
        [Server]
        private void DisplayTieScreen(){
            
            foreach (Player fellag in GameNetworkManager.Fellags)
                TargetHandleTieGame(fellag.connectionToClient);

            foreach (Player zellag in GameNetworkManager.Zellags)
                TargetHandleTieGame(zellag.connectionToClient);
        }

        [Server]
        private void DisplayVictoryScreen(Team team){

            switch (team)
            {
                case Team.Fellag:
                    foreach (Player fellag in GameNetworkManager.Fellags)
                        TargetHandleVictoryGame(fellag.connectionToClient);
                    
                break;

                case Team.Zellag:
                    foreach (Player zellag in GameNetworkManager.Zellags)
                        TargetHandleVictoryGame(zellag.connectionToClient);
                break;
                
            }
        }

        [Server]

        private void DisplayDefeatScreen(Team team){
            
            switch (team)
            {
                case Team.Fellag:
                    foreach (Player fellag in GameNetworkManager.Fellags)
                        TargetHandleDefeatGame(fellag.connectionToClient);
                    
                break;

                case Team.Zellag:
                    foreach (Player zellag in GameNetworkManager.Zellags)
                        TargetHandleDefeatGame(zellag.connectionToClient);
                break;
                
            }
        }
        
        
        [TargetRpc]
        void TargetHandleTieGame(NetworkConnection conn)
        {
            GameoverUI.enabled = true;
            tie.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        [TargetRpc]
        void TargetHandleVictoryGame(NetworkConnection conn)
        {
            GameoverUI.enabled = true;
            victory.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

        }

        [TargetRpc]
        void TargetHandleDefeatGame(NetworkConnection conn)
        {
            GameoverUI.enabled = true;
            defeat.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

        }

        [ClientRpc]
        void DisableEndGameUI()
        {
            GameoverUI.enabled = false;
            tie.SetActive(false);
            defeat.SetActive(false);
            victory.SetActive(false);
        }

            #endregion
       
            #region Sync vars hooks
        private void UpdateFellagsScore(int oldRoundsWonByFellags, int newRoundsWonByFellags){
            FellagsScore.text = $"{newRoundsWonByFellags}";
        }
        private void UpdateZellagsScore(int oldRoundsWonByZellags, int newRoundsWonByZellags){
            ZellagsScore.text = $"{newRoundsWonByZellags}";
        }

    
            #endregion
            
            #region NetManager's Players list update on Teams swap
        
        [ClientRpc]
        private void RpcUpdateClientsNetManagerPlayersList(List<Player> fellags, List<Player> zellags){
            GameNetworkManager.Fellags = fellags;
            GameNetworkManager.Zellags = zellags;
        }
            #endregion

            #region Match start display txt

            [ClientRpc]

            private void RpcDisplayMatchStartTxt(){
                TeamRoundWinTxt.text = "Le Match commence !";
                TeamRoundWinTxt.color = Color.cyan;
            }

            [ClientRpc]

            private void RpcResetMatchStartTxt(){
                TeamRoundWinTxt.text = "";
                TeamRoundWinTxt.color = Color.white;
            }
            #endregion

            #region Match reset on client
        public override void OnStopClient(){
            ResetMatch();
        }

            #endregion
        
        #endregion

        #region Server / Client code

        public void ResetMatch(){
            
            Debug.Log("Reset Match on client");

            roundNumber = 0;

            roundsWonByFellags = 0;
            roundsWonByZellags = 0;

            FellagsScore.text = $"{roundsWonByFellags}";
            ZellagsScore.text = $"{roundsWonByZellags}";
            

            TeamRoundWinTxt.text = "";
            
            PlantedTree = null;

            isTreePlanted = false;

            gameState = GameState.WaitingForPlayers;
        }

        
   

        #endregion




    }

}