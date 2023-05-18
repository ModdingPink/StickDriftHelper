using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using UnityEngine;
using Zenject;

namespace StickDriftHelper.UI
{
	public class StickDriftMenu : MonoBehaviour, IInitializable, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = null!;


        [UIValue("enabled")]
        public bool Enabled
        {
            get => Config.Instance.Enabled;
            set
            {
                Config.Instance.Enabled = value;
            }
        }        
        
        [UIValue("deadzone")]
        public float deadzone
        {
            get => Config.Instance.deadzone;
            set
            {
                Config.Instance.deadzone = value;
            }
        }     
        
        public void Initialize()
		{
            BSMLSettings.instance.AddSettingsMenu("Stick Drift Helper", "StickDriftHelper.UI.Menu.bsml", this);
		}
	}
}
