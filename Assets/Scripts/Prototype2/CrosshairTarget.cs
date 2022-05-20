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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype2
{
    public class CrosshairTarget : MonoBehaviour
    {
        public Camera PlayerCamera;
        Ray ray;
        RaycastHit hitInfo;
     
        
        

        ///<summary>Update is called once per frame</summary>
        ///<remarks>Updates the crosshair impact position</remarks>
        void Update()
        {

            //find the starting position of the player's camera
            ray.origin = PlayerCamera.transform.position;

            //find the aim direction
            ray.direction = PlayerCamera.transform.forward;

            //checks if we hit something with the crosshair
            //if yes update crosshair's position
            Physics.Raycast(ray,out hitInfo);        
            transform.position = hitInfo.point;
        }
    }

}