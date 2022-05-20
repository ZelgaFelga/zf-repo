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
    public class RoundTimer : NetworkBehaviour
    {
        [SerializeField]
        public TMP_Text roundTimer;
        private float MaxRoundTime = 300f;

        
        [SerializeField]

        [SyncVar]
        bool timeStarted = false;

        [SerializeField]
        [SyncVar(hook = nameof(HandleRoundTimerState))]
        bool timerUp = true;

        [SerializeField]
        [SyncVar]
        bool timerStoped = false;

        [SyncVar(hook = nameof(HandleRoundTimerChange))]
        float time ;
        
        public static RoundTimer singleton {get; set;}
        public static event Action OnRoundTimeElapsed;
        public static event Action OnPlantTreeTimerStart;
        

        private RoundTimer(){}

        [ServerCallback]
        private void OnEnable()
        {
            RoundSystem.OnMatchStarted += HandleTimerStart;

            RoundSystem.OnRoundEnd += HandleTimerStop;
            RoundSystem.OnRoundWin += ResetRoundTimerData;
            RoundSystem.OnTeamSwapTimerStart += HandleSwapTimerStart;
            RoundSystem.OnUnPauseRoundTimer += HandleTimerUnPause;

            PlantTimer.OnPlantTimeElapsed += HandPlantTimeElapse;

            Player.OnTreePlanted += HandlePlantTree;
        }
        
        [ServerCallback]
        private void OnDisable()
        {
            RoundSystem.OnMatchStarted -= HandleTimerStart;

            RoundSystem.OnRoundEnd -= HandleTimerStop;
            RoundSystem.OnRoundWin -= ResetRoundTimerData;
            RoundSystem.OnTeamSwapTimerStart -= HandleSwapTimerStart;
            RoundSystem.OnUnPauseRoundTimer -= HandleTimerUnPause;

            PlantTimer.OnPlantTimeElapsed -= HandPlantTimeElapse;

            Player.OnTreePlanted -= HandlePlantTree;

        }

   
        private void Awake(){

            if(singleton != null && singleton != this)
                Destroy(gameObject);
            
            singleton = this;

        }

        
        public override void OnStartClient()
        {
            time = MaxRoundTime;
        }

        public override void OnStopClient()
        {
            ResetMatchTimerData();
        }
        public override void OnStartServer()
        {
            time = MaxRoundTime;
        }

        public override void OnStopServer()
        {
            ResetMatchTimerData();

        }




        private void HandleRoundTimerState(bool oldState, bool newState){
            roundTimer.enabled = newState;
        }

        private void HandleRoundTimerChange(float oldTime, float newTime){
            
            if (newTime < 0)
                newTime = 0;
       
            int minutes = (int) (newTime / 60);
            int seconds = (int) (newTime % 60);
            roundTimer.text = String.Format("{0:00}:{1:00}", minutes, seconds);   
        }

        [ServerCallback]
        private void HandleTimerStart()
        {
            timerStoped = false;
            timeStarted = true;
            time = MaxRoundTime;
        }

        [Server]
        private void HandleTimerStop(){
            timerStoped = true;
            timeStarted = false;
            time = MaxRoundTime;
        }

        [Server]
        private void HandleSwapTimerStart(){
            RpcDisplaySwitchingTeam();   
        }

        [ClientRpc]
        private void RpcDisplaySwitchingTeam()
        {
            roundTimer.text = "Switching team in 5 seconds";
        }


        [Server]//
        private void HandleTimerUnPause()
        {
            time = MaxRoundTime;
            timerStoped = false;
            timeStarted = true;
        }

        [Server]
        private void HandPlantTimeElapse(){
            RpcEnableClientsRoundTimer();
        }

        [ClientRpc]
        private void RpcEnableClientsRoundTimer(){
            roundTimer.enabled = true;
        }


        [ServerCallback]
        void HandlePlantTree(GameObject plantedTree, GameObject plantZone)
        {
            OnPlantTreeTimerStart?.Invoke();
            timerUp = false;
            timeStarted = false;
            timerStoped = true;
        }


        [ServerCallback]
        public void ResetRoundTimerData()
        {        
            timerUp = true;
            timerStoped = false;
            timeStarted = true;
            time = MaxRoundTime;
            Debug.Log("Round timer data reset");
        }


        public void ResetMatchTimerData(){
            timeStarted = false;
            timerStoped = true;
            timerUp = true;
            roundTimer.enabled = true;  
            Debug.Log("Match timer data reset");
        }
        

        [ServerCallback]
        private void Update()
        { 
            if (timeStarted && !timerStoped)
            {
                if (time > 0)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    time = 0;
                    timeStarted = false;
                    OnRoundTimeElapsed?.Invoke();
                } 
                
            }

        }

        [ClientRpc]
        private void RpcUpdateTimer(float timerf)
        {
            roundTimer.enabled = timerUp;

            if (time < 0)
            {
                time = 0;
            }

            int minutes = (int) (time / 60);
            int seconds = (int) (time % 60);
            roundTimer.text = String.Format("{0:00}:{1:00}", minutes, seconds);   
        }




    }
}