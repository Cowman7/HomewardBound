using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using SDG.Unturned;

namespace HomewardBound
{
    public class Teleport : MonoBehaviour
    {
        private const float DelaySeconds = 10f;
        private const float TeleportCancelDistance = 1.0f;

        private static readonly Color InfoColor = Color.yellow;
        private static readonly Color ErrorColor = Color.red;
        private static readonly Color SuccessColor = Color.green;

        private readonly Dictionary<CSteamID, Coroutine> mdctPending = new Dictionary<CSteamID, Coroutine>();

        public void RequestHome(SteamPlayer player)
        {
            CSteamID steamId = player.playerID.steamID;

            if (mdctPending.TryGetValue(steamId, out Coroutine existing))
            {
                return;
            }

            if (!BarricadeManager.tryGetBed(steamId, out _, out _))
            {
                Helper.SendPrivateMessage(player, "You haven't claimed a bed. Place a bed and interact with it to claim it first.", ErrorColor);
                return;
            }

            Coroutine coroutine = StartCoroutine(CountdownThenTeleport(player));
            mdctPending.Add(steamId, coroutine);
        }

        private IEnumerator CountdownThenTeleport(SteamPlayer player)
        {
            CSteamID steamId = player.playerID.steamID;
            Vector3 startPosition = player.player.transform.position;

            Helper.SendPrivateMessage(player, $"Teleporting home in {DelaySeconds:0} seconds. Don't move.", InfoColor);

            yield return new WaitForSeconds(DelaySeconds);

            bool cancelled = false;
            string cancelReason = string.Empty;
            if (player == null || player.player == null)
            {
                cancelled = true;
                mdctPending.Remove(steamId);
                yield break;
            }

            Vector3 currentPosition = player.player.transform.position;
            if (Vector3.Distance(currentPosition, startPosition) > TeleportCancelDistance)
            {
                cancelled = true;
                cancelReason = "Home teleport cancelled - you moved.";
                mdctPending.Remove(steamId);
                yield break;
            }

            if (cancelled)
            {
                if (cancelReason != null && player != null && player.player != null)
                {
                    Helper.SendPrivateMessage(player, cancelReason, ErrorColor);
                }
                yield break;
            }

            player.player.teleportToBed();
            Helper.SendPrivateMessage(player, "Teleport Complete.", SuccessColor);

            mdctPending.Remove(steamId);
        }
    }
}
