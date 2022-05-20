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


namespace Prototype3
{
    public class Animation3 : Prototype2.Animation
    {
        [SerializeField]
        float velocity = 0;
        Vector3 previous;
        bool isDead;

        #region Footsteps clip audio

        [SerializeField]
        private AudioClip stepClip;
        #endregion


        #region Subscribing to events
        private void OnEnable(){
            FootStepsAudio.OnPlayerMove += PlayStepClip;
        }

        private void OnDisable(){
            FootStepsAudio.OnPlayerMove -= PlayStepClip;

        }
        #endregion

        #region Server
        
        [Command]
        private void CmdPlayStepClip()
        {
            RpcPlayStepClip();
        }

        #endregion

        #region Client

        public override void Update()
        {
        
            if (isLocalPlayer)
            {
                isDead = GetComponent<Player>().IsDead;
                if (isDead)
                {
                    return;
                }
                Animate();
            }

            velocity = (transform.position - previous).magnitude / Time.deltaTime;
            previous = transform.position;

        }
        public override void Animate()
        {
            #region Running Forward Animation
            //Running Forward Animation
            if (Input.GetKey("w") && velocity>0)
                animator.SetBool("isRunningForward", true);


            if (!Input.GetKey("w")&& velocity==0)
                animator.SetBool("isRunningForward", false);

            #endregion

            #region Running Backward Animation

            //Running Backward Animation
            if (Input.GetKey("s") && velocity > 0)
                animator.SetBool("isRunningBackward", true);

            if (!Input.GetKey("s")&& velocity == 0)
                animator.SetBool("isRunningBackward", false);
            #endregion

            #region Running Right Animation

            //Running Right Animation
            if (Input.GetKey("d") && velocity > 0)
                animator.SetBool("isRunningRight", true);
            if (!Input.GetKey("d"))
                animator.SetBool("isRunningRight", false);

            #endregion

            #region Running Left Animation
            //Running Left Animation
            if (Input.GetKey("a") && velocity > 0)
                animator.SetBool("isRunningLeft", true);

            if (!Input.GetKey("a") && velocity == 0)
                animator.SetBool("isRunningLeft", false);

            #endregion
        }

        ///<summary>Play step clip when the player moves</summary>
        void PlayStepClip()
        {
            if(!hasAuthority) return;

            if(isLocalPlayer)
                CmdPlayStepClip();
        }

        ///<summary>Play step clip on the networks</summary>
        [ClientRpc]
        private void RpcPlayStepClip()
        {
            if (!hasAuthority) return;

            if(isLocalPlayer)
            
            if (GetComponent<CharacterController>().isGrounded)
                GetComponent<AudioSource>().PlayOneShot(stepClip);
        }


        #endregion



        
    }

}