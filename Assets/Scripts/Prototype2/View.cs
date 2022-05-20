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
using Prototype3;
using UnityEngine;

namespace Prototype2
{
    
    public class View : NetworkBehaviour
    {
        // Start is called before the first frame update
        
        #region Mouse
        private float xRotation = 0f;

        private float mouseX;

        private float mouseY;
        public float MouseSensitivity = 100f;

        #endregion

        #region Camera
        public Transform playerCamera;

        public Transform minimapCamera;

        #endregion

        ///<summary>Start is called once before the Update</summary>
        void Start()
        {

            Cursor.lockState = CursorLockMode.Locked;

            if (!isLocalPlayer)
            {
                //deactivating other players audios and cameras
                playerCamera.GetComponent<AudioListener>().enabled = false;
                playerCamera.GetComponent<Camera>().enabled = false;
                minimapCamera.GetComponent<Camera>().enabled = false;
            }
        }

        ///<summary>Update is called once per frame</summary>
        [ClientCallback]
        public virtual void Update()
        {
            if (!hasAuthority) return;
            if (isLocalPlayer)
            {
                Look();
            }
            
        }


        ///<summary>Let the player looks around</summary>
        void Look(){
            
            mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);

            transform.Rotate(Vector3.up * mouseX);
        }

 
    }
}
