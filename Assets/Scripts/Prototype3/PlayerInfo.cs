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

using Prototype3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace Prototype3
{
    public class PlayerInfo : MonoBehaviour
    {
        
        [SerializeField]
        TMP_Text _name, team, kills, deaths;

        ///<summary>Sets the player's info to display on the score board</summary>
        public void SetInfo(string name_,Team team_, int kills_, int deaths_)
        {
            _name.text = name_;
            team.text = team_.ToString();
            kills.text = kills_.ToString();
            deaths.text = deaths_.ToString();
        }
    }

}