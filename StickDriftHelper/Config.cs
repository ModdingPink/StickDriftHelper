using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

namespace StickDriftHelper
{
    public class Config
    {
        public static Config? Instance { get; set; }

        public virtual bool Enabled { get; set; } = true;
        public virtual float deadzone { get; set; } = 0.75f;

    }
}