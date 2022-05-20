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

using Prototype2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype3
{
    public class PauseMenu3 : PauseMenu2
{
    [SerializeField]
    private GameObject VictoryUI;
    [SerializeField]
    private GameObject TieUI;
    [SerializeField]
    private GameObject DefeatUI;
    [SerializeField]
    private Canvas GameOverUI;

    [SerializeField]
    private Canvas UIplayer;
    [SerializeField]
    private Canvas RoundTimerUI;
    [SerializeField]
    private Canvas PlantTimerUI;

    public override void Resume()
    {
        base.Resume();
        UIplayer.enabled = true;
        RoundTimerUI.enabled = true;
        PlantTimerUI.enabled = true;
    }
    
    
    public override void Pause()
    {
        base.Pause();
        UIplayer.enabled = false;
        RoundTimerUI.enabled = false;
        PlantTimerUI.enabled = false;
    }
    
    public override void GoToOptionsMenu()
    {
        base.GoToOptionsMenu();
        UIplayer.enabled = false;
    }
    
    public override void GoBackToMainMenu()
    {
        base.GoBackToMainMenu();

        UIplayer.enabled = false;
        RoundTimerUI.enabled = false;
        PlantTimerUI.enabled = false;
        VictoryUI.SetActive(false);
        TieUI.SetActive(false);
        DefeatUI.SetActive(false);
        GameOverUI.enabled = false;
    }

}

}