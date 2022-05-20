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

namespace Prototype2
{
    
    public class Weapon : NetworkBehaviour
    {

        #region Weapon Attribute

        [SerializeField]
        private ParticleSystem[] muzzleFlash;
        // bullet impact particles
        [SerializeField]
        private ParticleSystem impactEffect;
        protected RaycastHit hitInfo;
        protected Ray ray;

        ///<summary>Raycast starts here</summary>
        [SerializeField]
        private Transform raycastOrigin;

        ///<summary>Raycase ends here</summary>
        public Transform raycastImpact;
        #endregion
        
        #region Server

        ///<summary>Starts firing</summary>
        [Command]
        public virtual void CmdStartFiring(NetworkConnectionToClient playerConn = null)
        {


            ray.origin = raycastOrigin.position;

            ray.direction = (raycastImpact.position - raycastOrigin.position).normalized;

            RpcPlayMuzzleFlash();

            if (Physics.Raycast(ray, out hitInfo))
            {
                RpcPlayImpactEffect(hitInfo.point, hitInfo.normal);

            }

        }

        #endregion

        #region Client

        ///<summary>Play impact effect on the network</summary>
        ///<param name="hitPoint">The hit point</param>
        ///<param name="direction">The hit point direction</param>

        [ClientRpc]
        protected void RpcPlayImpactEffect(Vector3 hitPoint, Vector3 direction)
        {
            impactEffect.transform.position = hitPoint;
            impactEffect.transform.forward = direction;
            impactEffect.Emit(1);
        }


        ///<summary>Play muzzle flash on the network</summary>
        [ClientRpc]
        protected void RpcPlayMuzzleFlash()
        {

            foreach (var particle in muzzleFlash)
            {
                particle.Emit(1);
            }
        }
        #endregion
    }

}