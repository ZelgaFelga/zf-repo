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
    
    public class Animation : NetworkBehaviour
    {
        #region Animation attributes
        public Animator animator;

        #endregion


        ///<summary>Update is called once per frame</summary>
        public virtual void Update()
        {
            if(!hasAuthority) return;
            if(isLocalPlayer)
                Animate();
        }

        ///<summary>Animates the player according to the key pressed</summary>
        public virtual void Animate(){
            
            #region Running Forward Animation
            //Running Forward Animation
            if (Input.GetKey("w"))
                animator.SetBool("isRunningForward", true);


            if (!Input.GetKey("w"))
                animator.SetBool("isRunningForward", false);

            #endregion

            #region Running Backward Animation

            //Running Backward Animation
            if (Input.GetKey("s"))
                animator.SetBool("isRunningBackward", true);

            if (!Input.GetKey("s"))
                animator.SetBool("isRunningBackward", false);
            #endregion 

            #region Running Right Animation

            //Running Right Animation
            if (Input.GetKey("d"))
                animator.SetBool("isRunningRight", true);
            if (!Input.GetKey("d"))
                animator.SetBool("isRunningRight", false);

            #endregion

            #region Running Left Animation
        //Running Left Animation
        if (Input.GetKey("a"))
            animator.SetBool("isRunningLeft", true);

        if (!Input.GetKey("a"))
            animator.SetBool("isRunningLeft", false);

        #endregion
        
        }
    }

}