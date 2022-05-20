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
using UnityEngine;

namespace Prototype3
{
    public class FootStepsAudio : MonoBehaviour
    {

        public static event Action OnPlayerMove;
 
        ///<summary>Invoke an event to play a step clip when an animation event occurs</summary>
        void SideStepLeft()
        {
            OnPlayerMove?.Invoke();
        }
        
        ///<summary>Invoke an event to play a step clip when an animation event occurs</summary>

        void SideStepRight()
        {
            OnPlayerMove?.Invoke();
        }
        
        ///<summary>Invoke an event to play a step clip when an animation event occurs</summary>

        void ForwardStep()
        {
            OnPlayerMove?.Invoke();

        }

        ///<summary>Invoke an event to play a step clip when an animation event occurs</summary>
        void BackwardStep()
        {
            OnPlayerMove?.Invoke();

        }

    }

}