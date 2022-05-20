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
using UnityEngine;
using System;


namespace Prototype2
{
    public class Armament : NetworkBehaviour
    {
       

        #region Weapon Config
        public Weapon[] weaponPrefab;

        [SyncVar]
        private Weapon activeWeapon;
      
        [SerializeField]
        private Transform crossHairTarget;
        // reference pour le parent de l'arme
        [SerializeField]
        private Transform weaponParent;

        [SerializeField]
        private Transform leftGrip;

        [SerializeField]
        private Transform rightGrip;

        

        #endregion

        

        #region Server

            #region Changing weapon Server's commands

        ///<summary>Switch the weapon to pistol</summary>
        [Command]
        public void CmdGetPistol(){
            Weapon newWeapon =  Instantiate(weaponPrefab[0]);
            NetworkServer.Spawn(newWeapon.gameObject,connectionToClient);
            RpcEquipe(newWeapon);
        }


        ///<summary>Switch the weapon to rifle</summary>
        [Command]
        private void CmdGetRifle(){
            Weapon newWeapon = Instantiate(weaponPrefab[1]);
            NetworkServer.Spawn(newWeapon.gameObject,connectionToClient);
            RpcEquipe(newWeapon);
        }
            #endregion

            #region Refresh weapons for new clients server's code
        
        ///<summary>Refreshes others players weapon to the new connected player</summary>
        ///<param name="newClient">The connection of the client sending the command</param>
        [Command]
        private void CmdRefreshOthersWeaponForMe(NetworkConnectionToClient newClient = null){

            List<NetworkIdentity> playersID = ((ZFNetworkManager2)NetworkManager.singleton).playersId;


            for( int i = 0 ; i < playersID.Count - 1 ; i++)
            {
                playersID[i].GetComponent<Armament>().TargetRefreshWeaponFor(newClient);
            }

        }

            #endregion
        
        #endregion

        #region Client

            #region Changing weapon client's code

        [ClientCallback]
        ///<summary>Update is called once per frame</summary>
        public virtual void Update()
        {
            if(!hasAuthority) return;
            if (isLocalPlayer){               
                #region Changing weapon on a key press

                if (Input.GetKey("1"))
                {
                    CmdGetPistol();
                }
                if (Input.GetKey("2"))
                {
                    CmdGetRifle();
                }
                #endregion
                
                #region Firing on a Mouse's Button press
                if (activeWeapon)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        activeWeapon.CmdStartFiring();
                    }
                
                }   
                #endregion      

            }
            
        }

        ///<summary>Shows other players the equiped weapon</summary>
        ///<param name="newWeapon">The new weapon to equip</param>
        [ClientRpc]
        private void RpcEquipe(Weapon newWeapon)
        {
            if (activeWeapon)
            {
                NetworkServer.Destroy(activeWeapon.gameObject);
                Destroy(activeWeapon.gameObject);
            }
            
            activeWeapon = newWeapon;
            //definir la distination du raycast
            activeWeapon.raycastImpact = crossHairTarget;
            //recupere la position du parent ainsi que la retation
            activeWeapon.transform.parent = weaponParent;
            //reset les parametres de la position et de la rotation pour placer larme correctement en position 
            activeWeapon.transform.localPosition = Vector3.zero;

            activeWeapon.transform.localRotation = Quaternion.identity;
            
        }

            #endregion

            #region Refresh weapon for new players and players in game
        public override void OnStartClient()
        {
            if(!hasAuthority) return;

            if(isLocalPlayer){
                CmdGetPistol();
                CmdRefreshOthersWeaponForMe();
            }
        }

        ///<summary>Refresh in game players weapon to the new player</summary>
        ///<param name="conn">The new player connection</param>
        [TargetRpc]
        private void TargetRefreshWeaponFor(NetworkConnection conn){
            
            activeWeapon.transform.parent = weaponParent;
            //reset les parametres de la position et de la rotation pour placer l'arme correctement en position 
            activeWeapon.transform.localPosition = Vector3.zero;

            activeWeapon.transform.localRotation = Quaternion.identity;
        }   
            
            #endregion

        #endregion

    }
}
