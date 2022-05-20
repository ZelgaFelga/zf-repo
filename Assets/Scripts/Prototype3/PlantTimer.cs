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
using TMPro;
using UnityEngine;

public class PlantTimer : NetworkBehaviour
{
    [SerializeField]
    TMP_Text plantTimer;

    public static int MaxSeconds = 45;

    [SyncVar(hook = nameof(HandlePlantTimerChange))]
    float seconds = MaxSeconds;
    [SerializeField]
    // [SyncVar]
    // bool timerStarted = false;
    [SyncVar(hook = nameof(HandlePlantTimerStateChange))]
    bool timerUp = false;

    [SyncVar]
    bool timerStarted = false;

    public static event Action OnPlantTimeElapsed;

    public static PlantTimer singleton {get; set;}

    private PlantTimer(){}
    private void Awake()
    {
        if(singleton != null && singleton != this)
                Destroy(gameObject);
            
        singleton = this;

        plantTimer = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        RoundTimer.OnPlantTreeTimerStart += HandleTimerStart;
        Player.OnTreeCutDown += ResetValues;
    }
    private void OnDisable()
    {
        RoundTimer.OnPlantTreeTimerStart -= HandleTimerStart;
        Player.OnTreeCutDown -= ResetValues;
    }


    public override void OnStartClient()
    {
        plantTimer.enabled = true;
        plantTimer.text = "";
    }

    public override void OnStopClient()
    {
        timerStarted = false;

        seconds = MaxSeconds;

        plantTimer.enabled = false;

    }
    public override void OnStopServer()
    {
        timerStarted = false;
                
        seconds = MaxSeconds;

        plantTimer.enabled = false;

    }


    [ServerCallback]
    private void Update()
    {        
       
        if (timerStarted)
        {
            if (seconds > 0)
            {
                if(seconds == MaxSeconds) RpcEnableClientsPlantTimer(true);

                seconds -= Time.deltaTime;
            }
            else if (seconds < 0)
            {
                Debug.Log("Plant timee Elapsed");
                
                OnPlantTimeElapsed?.Invoke();

                ResetValues();
            }
        }

    }

    [Server]
    private void ResetValues()
    {
        timerStarted = false;
        seconds = MaxSeconds;
        
        RpcEnableClientsPlantTimer(false);
    }

    [ClientRpc]
    private void RpcEnableClientsPlantTimer(bool value){
        plantTimer.enabled = value;
    }
  

    [ServerCallback]
    void HandleTimerStart()
    {
        timerStarted = true;
        timerUp = true;
    }

    private void HandlePlantTimerStateChange(bool oldTimerState, bool newTimerState){

        plantTimer.enabled = newTimerState;
    }

    private void HandlePlantTimerChange(float oldSeconds,float newSeconds){

        plantTimer.text = String.Format("{0:00}", newSeconds);

    }


}
