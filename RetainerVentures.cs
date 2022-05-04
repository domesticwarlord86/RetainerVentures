using Buddy.Coroutines;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Enums;
using ff14bot.NeoProfiles;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Collections.Generic;
using TreeSharp;
using System;
using System.Collections.Generic;
using LlamaLibrary.Helpers;
using LlamaLibrary.Logging;


namespace RetainerVentures
{
    public class RetainerVentures : BotPlugin
    {
        private Composite _coroutine;

        public override string Author
        {
            get { return "DomesticWarlord"; }
        }

        public override string Name { get; } = NameValue;
        private static readonly string NameValue = "Retainer Ventures";

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        private static readonly LLogger Log = new LLogger(NameValue, Colors.Aquamarine);
        private DateTime _lastChecked = new DateTime(1970, 1, 1);

        public override async void OnInitialize()
        {
            _coroutine = new ActionRunCoroutine(r => PluginTask());
        }

        public override void OnEnabled()
        {
            TreeRoot.OnStart += OnBotStart;
            TreeRoot.OnStop += OnBotStop;
            TreeHooks.Instance.OnHooksCleared += OnHooksCleared;

            if (TreeRoot.IsRunning)
            {
                AddHooks();
            }
        }

        public override void OnDisabled()
        {
            TreeRoot.OnStart -= OnBotStart;
            TreeRoot.OnStop -= OnBotStop;
            RemoveHooks();
        }

        public override void OnShutdown()
        {
            OnDisabled();
        }

        private RetainerVentureSettings _settings;
        public override bool WantButton => true;
        public override string ButtonText => "Settings";
        private Form1 _form;
        public static RetainerVentureSettings RetainerVentureSettings => RetainerVentureSettings.Instance;

        public override void OnButtonPress()
        {
            if (_form == null)
            {
                _form = new Form1()
                {
                    Text = "Retainer Venture Settings v" + Version,
                };
                _form.Closed += (o, e) => { _form = null; };
            }

            try
            {
                _form.Show();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void AddHooks()
        {
            Logging.Write(Colors.Aquamarine, "Adding Retainer Hook");
            TreeHooks.Instance.AddHook("TreeStart", _coroutine);
        }

        private void RemoveHooks()
        {
            Logging.Write(Colors.Aquamarine, "Removing Retainer Hook");
            TreeHooks.Instance.RemoveHook("TreeStart", _coroutine);
        }

        private void OnBotStop(BotBase bot)
        {
            RemoveHooks();
        }

        private void OnBotStart(BotBase bot)
        {
            AddHooks();
        }

        private void OnHooksCleared(object sender, EventArgs e)
        {
            RemoveHooks();
        }

        private static async Task PluginTask()
        { 
            if ((DateTime.Now - RetainerVentureSettings.Instance.LastChecked).TotalMinutes > RetainerVentureSettings.Instance.CheckTime)
            {
                if (!Core.Me.InCombat || Core.Me.IsAlive || !FateManager.WithinFate)
                    await LlamaLibrary.Retainers.HelperFunctions.CheckVentureTask();
            }
        }
    }
}