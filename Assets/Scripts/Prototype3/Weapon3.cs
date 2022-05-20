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
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Prototype3
{
    public class Weapon3 : Weapon
    {
        
        private Player player;

        private Team targetPlayerTeam;
        [SerializeField]
        private AudioSource audioSource;

     
       

        private void Awake(){
            audioSource = GetComponent<AudioSource>();           
        }

        [ClientRpc]
        private void RpcPlayFiringAudio(){            
            audioSource.Play();
        }

        [ClientRpc]
        private void RpcPlayImactAudio(){                
           GetComponentInChildren<ImpactSound>().GetComponent<AudioSource>().Play();          
        }


        [Command]
        public override void CmdStartFiring(NetworkConnectionToClient playerConn = null)
        { 
            base.CmdStartFiring();

            RpcPlayFiringAudio();

            if (Physics.Raycast(ray, out hitInfo))
            {
                RpcPlayImactAudio();

                if (hitInfo.collider.TryGetComponent<Player>(out Player targetPlayer))
                {

                    targetPlayerTeam = targetPlayer.Team_;

                    player = playerConn.identity.GetComponent<Player>();

                    if (player.Team_ != targetPlayerTeam)
                    {
                        targetPlayer.SetDamageBy(player, 10);
                    }
                }
            }

        }

    }
}
