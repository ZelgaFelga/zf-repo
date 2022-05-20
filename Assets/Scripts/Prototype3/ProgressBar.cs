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

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Prototype3
{
    public class ProgressBar : MonoBehaviour
    {

        [SerializeField]
        GameObject progressBar;
        public Image image;
        public TMP_Text percentage;
        bool startTimer = false;
        
        [Range(0, 5)]
        public float timer = 0f;
        [Range(0, 100)]
        public float amount = 0;
        public float holdTime = 5f;
        public static Action OnProgressTreePlantComplete;
        public static Action OnProgressTreeCutComplete;
        bool CanInteract = true;
        Team _team;

        #region Subscribing to events
        private void OnEnable()
        {

            Player.OnTryPlantTree += ActivatePlantingProgress;
            Player.OnTryCutDownTree += ActivateCuttingDownProgress;
            Player.OnCancelPlantOrCutTree += CancelPlantOrCutProgress;
        }

        #endregion

        #region UnSubscribing to events
        private void OnDestroy()
        {
            Player.OnTryPlantTree -= ActivatePlantingProgress;
            Player.OnTryCutDownTree -= ActivateCuttingDownProgress;
            Player.OnCancelPlantOrCutTree -= CancelPlantOrCutProgress;
        }

        #endregion


        public void ActivateCuttingDownProgress()
        {
            Debug.Log("Trying to cut down the tree !");
            progressBar.SetActive(true);
            startTimer = true;
            _team = Team.Zellag;

        }

        public void ActivatePlantingProgress()
        {
            Debug.Log("Trying to plant the tree !");
            progressBar.SetActive(true);
            startTimer = true;
            _team = Team.Fellag;
        }

        void CancelPlantOrCutProgress()
        {
            if(_team == Team.Fellag)
                Debug.Log("Planting canceled");
            else
                Debug.Log("Cutting down canceled");
            
            startTimer = false;
            CanInteract = true;
            progressBar.SetActive(false);
            ResetActions();
        }


        private void ResetActions()
        {
            timer = 0;
            amount = 0;
            image.fillAmount = timer / holdTime;
            percentage.text = string.Format("{0}%", amount);
        }

        private void Update()
        {
            if (startTimer)
            {
                if (timer <= 5 || amount <= 100)
                {
                    timer += Time.deltaTime;
                    amount = timer * 20;
                    int amountt = (int)amount;
                    percentage.text = string.Format("{0}%", amountt);
                    image.fillAmount = timer / holdTime;

                }

                if (timer >= holdTime && CanInteract && amount > 100)
                {

                    if (_team == Team.Fellag)
                    {
                        OnProgressTreePlantComplete?.Invoke();
                    }
                    else if (_team == Team.Zellag)
                    {
                        OnProgressTreeCutComplete?.Invoke();
                    }

                    progressBar.SetActive(false);

                    CanInteract = false;
                }
            }

        }
    }

}