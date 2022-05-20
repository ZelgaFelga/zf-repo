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
    
    public class Movement : NetworkBehaviour
    {

       
        #region Movement attributes

        [SerializeField]
        private CharacterController controller;
        private float speed = 5;
        private float jumpHeight = 2f;
        private float gravityModifier = 1f;       
        private Vector3 move;

        #endregion
        
        ///<summary>Update is called once per frame</summary>
        [ClientCallback]
        public virtual void Update()
        {
            if(!hasAuthority) return;
            if (isLocalPlayer)
            {
                Move();
            }           
        }

        ///<summary>Move the player inside the scene</summary>
        private void Move(){
            
            //store y velocity value
            float yValueStore = move.y;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            move = ((transform.forward * z) + (transform.right * x)).normalized;

            move.y = yValueStore;

            //appliquer la gravite
            move.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
            
            if (controller.isGrounded)
            {
                move.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
                
                if (Input.GetButtonDown("Jump"))
                {
                    move.y = jumpHeight;
                }
            }

            if (controller.enabled)
            {
                controller.Move(move * speed * Time.deltaTime);
            }
                    
        }

 
    }
}
