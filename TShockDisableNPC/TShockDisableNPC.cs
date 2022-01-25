using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.Localization;


namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "禁NPC";
        public override string Description => "";
        public override string Author => "hufang360";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(new List<string>() {}, InitCommand, "", "") { HelpText = ""});
        }


        private void InitCommand(CommandArgs args)
        {

        }


        protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			base.Dispose(disposing);
		}
	}

}
