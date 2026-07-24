using System;
using UnityEngine;
using SDG.Unturned;
using SDG.Framework.Modules;

namespace HomewardBound
{
    public class Nexus : IModuleNexus
    {
        private GameObject RunnerObject;
        private Teleport TeleportRunner;
        public void initialize()
        {
            Helper.Print("Initialized Module");

            ChatManager.onChatted += OnChatted;

            RunnerObject = new GameObject("TeleportModule_Runner");
            UnityEngine.Object.DontDestroyOnLoad(RunnerObject);
            TeleportRunner = RunnerObject.AddComponent<Teleport>();

            Helper.Print("Installed Module");
        }

        public void shutdown()
        {
            ChatManager.onChatted -= OnChatted;

            if (RunnerObject != null)
            {
                UnityEngine.Object.Destroy(RunnerObject);
            }

            Helper.Print("Shutdown Module");
        }

        private void OnChatted(
            SteamPlayer player,
            EChatMode mode,
            ref Color chatted,
            ref bool isRich,
            string text,
            ref bool isVisible)
        {
            if (!TryParseCommand(text, out string commandName, out string argsText))
            {
                return;
            }

            if (!commandName.Equals("home", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            isVisible = false;

            TeleportRunner.RequestHome(player); ;
        }

        private static bool TryParseCommand(string text, out string commandName, out string argsText)
        {
            commandName = string.Empty;
            argsText = string.Empty;

            if (string.IsNullOrWhiteSpace(text) || text[0] != '/')
            {
                return false;
            }

            string withoutSlash = text.Substring(1).Trim();
            if (withoutSlash.Length == 0)
            {
                return false;
            }

            int spaceIndex = withoutSlash.IndexOf(' ');
            if (spaceIndex < 0)
            {
                commandName = withoutSlash;
            }
            else
            {
                commandName = withoutSlash.Substring(0, spaceIndex);
                argsText = withoutSlash.Substring(spaceIndex + 1).Trim();
            }

            return commandName.Length > 0;
        }
    }
}
