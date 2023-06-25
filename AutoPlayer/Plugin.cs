using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;
using Conf = IPA.Config.Config;
using HarmonyLib;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using AutoPlayer.Installers;
using AutoPlayer;

namespace AutoPlayer
{
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();



        [Init]
        public Plugin(IPALogger logger, Conf conf, Zenjector zenjector)
        {

            Config.Instance = conf.Generated<Config>();

            zenjector.UseLogger(logger);
            zenjector.Install<MenuInstaller>(Location.Menu);
            zenjector.Install<PlayerInstaller>(Location.Player);

         
        }

        [OnStart]
        public void OnStart()
        {
            var harmony = new Harmony("Pink.Autoplay");
            harmony.PatchAll(Assembly);
         
        }
    }
}
