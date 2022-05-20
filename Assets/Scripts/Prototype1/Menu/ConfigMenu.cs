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


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Prototype1
{
    public class ConfigMenu : MonoBehaviour
    {
        private Resolution[] resolutions;

        [SerializeField]
        private TMP_Dropdown resolutionsDropdown;

        [SerializeField]
        private GameObject mainMenuPanel;

        [SerializeField]
        private GameObject newMatchPanel;

        [SerializeField]
        private GameObject configPanel;
        [SerializeField]
        private Toggle fullScreenBTN;

        public virtual void Start()
        {
          
            //charger la liste des resolutions d'écran disponibles
            loadScreenResolutions();

            if (fullScreenBTN.isOn)
                setFullScreen(true);
            else
                setFullScreen(false);
        }

        private void Update()
        {

            #region Retourner au menu principal Quand Esc est pressée
            if (Input.GetKeyDown(KeyCode.Escape))
            {

                //Le jeu n'est pas en pause
                if (!PauseMenu.IsGameInPause)
                    //On retourne au menu principal
                    if (configPanel.activeSelf)
                        returnBackFromConfigPanel();



                if (newMatchPanel.activeSelf)
                    returnBackFromNewMatchPanel();

                // if(configPanel.activeSelf && !PauseMenu.IsGameInPause) 
                //     returnBackFromConfigPanel();

                // if(newMatchPanel.activeSelf && !PauseMenu.IsGameInPause)
                //     returnBackFromNewMatchPanel();

            }
            #endregion

        }

        #region Configurer le jeu

        public AudioMixer audioMixer;
        public void changeVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
        }

        public void changeGameQuality(int quality)
        {
            QualitySettings.SetQualityLevel(quality);
        }

        public void setFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;

        }

        public void loadScreenResolutions()
        {

            resolutions = Screen.resolutions;

            int currentResolution = 0;

            List<string> options = new List<string>();

            resolutionsDropdown.ClearOptions();

            for (int i = 0; i < Screen.resolutions.Length; i++)
            {

                // if(resolutions[i].refreshRate >= 60) 
                options.Add($"{resolutions[i].width} * {resolutions[i].height} @{resolutions[i].refreshRate} Hz");

                if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                {

                    currentResolution = i;

                }
            }

            resolutionsDropdown.AddOptions(options);
            resolutionsDropdown.value = currentResolution;
            resolutionsDropdown.RefreshShownValue();
        }

        public void setScreenResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, fullScreenBTN.isOn);
        }

        #endregion

        #region Retourner au menu principal

        public virtual void returnBackFromConfigPanel()
        {

            configPanel.SetActive(false);
            mainMenuPanel.SetActive(true);

        }
        public virtual void returnBackFromNewMatchPanel()
        {
            newMatchPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        #endregion
    }
}
