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
using Mirror;
using UnityEngine;

namespace Prototype3
{
        
    public class PlantingZone : NetworkBehaviour
    {
        [SerializeField]
        public string Name;

        
        private void OnTriggerEnter(Collider other){


            if(other.TryGetComponent<Player>(out Player player)){

                switch (player.Team_)
                {
                    case Team.Fellag:
                        player.AuthorizePlantIn(this.gameObject);
                        break;
                    case Team.Zellag:
                        player.AuthorizeCutDownIn(this.gameObject);
                        break;

                }

            }
        }

        private void OnTriggerExit(Collider other){
            
            if(other.TryGetComponent<Player>(out Player player)){
  
                switch (player.Team_)
                {
                    case Team.Fellag:

                        player.UnauthorizePlant();
                        break;
                    case Team.Zellag:

                        player.UnauthorizeCutDown();
                        break;

                }


            }


        }



    }



}