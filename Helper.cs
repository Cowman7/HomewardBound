using System;
using SDG.Unturned;
using UnityEngine;

namespace HomewardBound
{
    static class Helper
    {
        public static void Print(string message)
        {
            UnturnedLog.info($"HomewardBound - [INFO] - {message}");

            Console.WriteLine(message);
        }

        public static void Warning(string message)
        {
            UnturnedLog.warn($"HomewardBound - [WARN] - {message}");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Error(string message)
        {
            UnturnedLog.error($"HomewardBound - [ERRO] {message}");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void SendPrivateMessage(SteamPlayer player, string message, Color color)
        {
            ChatManager.serverSendMessage(text: message,
                                          color: color,
                                          fromPlayer: null,
                                          toPlayer: player,
                                          mode: EChatMode.SAY,
                                          iconURL: null,
                                          useRichTextFormatting: false);
        }
    }
}
