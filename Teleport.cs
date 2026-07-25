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
                Helper.SendPrivateMessage(player, "Home teleport in progress. Move to cancel.", InfoColor);
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


            double elapsedTime = 0.0f;
            do
            {
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;

                if (player == null || player.player == null)
                {
                    mdctPending.Remove(steamId);
                    yield break;
                }

                Vector3 currentPosition = player.player.transform.position;
                if (Vector3.Distance(currentPosition, startPosition) > TeleportCancelDistance)
                {
                    Helper.SendPrivateMessage(player, "Home teleport cancelled - you moved.", ErrorColor);
                    mdctPending.Remove(steamId);
                    yield break;
                }
            } while (elapsedTime < DelaySeconds);

            bool completed = player.player.teleportToBed();
            mdctPending.Remove(steamId);

            if (completed)
            {
                Helper.SendPrivateMessage(player, "Teleport Complete.", SuccessColor);
            } 
            else
            {
                Helper.SendPrivateMessage(player, "Teleport Failed.", ErrorColor);
            }
        }
    }
}
