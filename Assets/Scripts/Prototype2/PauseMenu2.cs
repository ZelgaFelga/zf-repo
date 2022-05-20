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

using System.Collections;
using System.Collections.Generic;
using Mirror;
using Prototype1;
using UnityEngine;

namespace Prototype2
{
    public class PauseMenu2 : PauseMenu
    {

        ///<summary>Pause the game</summary>
        public override void Pause()
        {
            base.Pause();

            PausePlayer();
        }

        ///<summary>Resume the game</summary>
        public override void Resume()
        {
            base.Resume();
            
            UnPausePlayer();
        }

        ///<summary>Disable some player scripts</summary>
        ///<remarks>Disable Movement,Animation,View and Armament</remarks>
        public virtual void PausePlayer()
        {

            NetworkIdentity player = NetworkClient.connection.identity;
            player.GetComponent<Armament>().enabled = false;
            player.GetComponent<Movement>().enabled = false;
            player.GetComponent<View>().enabled = false;
            player.GetComponent<Animation>().enabled = false;

        }
        ///<summary>Re-enable some player scripts</summary>
        ///<remarks>Enable Movement,Animation,View and Armament</remarks>
        public virtual void UnPausePlayer()
        {
            NetworkIdentity player = NetworkClient.connection.identity;
            player.GetComponent<Armament>().enabled = true;
            player.GetComponent<Movement>().enabled = true;
            player.GetComponent<View>().enabled = true;
            player.GetComponent<Animation>().enabled = true;
        }
    }
}
