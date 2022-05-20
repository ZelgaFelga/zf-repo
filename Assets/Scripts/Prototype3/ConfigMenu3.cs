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
using System.Collections;
using System.Collections.Generic;
using Prototype1;
using TMPro;
using UnityEngine;

namespace Prototype3
{
    public class ConfigMenu3 : ConfigMenu
    {

        [SerializeField]
        private GameObject GameBackground;

        [SerializeField]
        private TMP_InputField playerNameInputTxtField;

        public static Action<string> OnPlayerNameChange;

        #region Retourner au Menu Principal

        ///<summary>Return to main menu from the configuration panel</summary>
        public override void returnBackFromConfigPanel()
        {
            base.returnBackFromConfigPanel();
            GameBackground.SetActive(true);
        }

        ///<summary>Return to main menu from the new match panel</summary>
        public override void returnBackFromNewMatchPanel()
        {
            base.returnBackFromNewMatchPanel();
            GameBackground.SetActive(true);

        }
        #endregion

        #region Player name
        
        ///<summary>Changes player's name from the configuration panel</summary>
        public void ChangePlayerName(string newName){

            PlayerPrefs.SetString("playerName",newName);
            PlayerPrefs.Save();

            OnPlayerNameChange?.Invoke(newName);
        }

        ///<summary>Start is called once before Update()</summary>
        public override void Start(){

            base.Start(); 

            if(PlayerPrefs.HasKey("playerName"))
                playerNameInputTxtField.text = PlayerPrefs.GetString("playerName");

        }

        #endregion
    }
}
