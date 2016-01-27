﻿/*
 * Copyright (c) 2013-2015, SteamDB. All rights reserved.
 * Use of this source code is governed by a BSD-style license that can be
 * found in the LICENSE file.
 */

using System;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using SteamKit2;

namespace SteamDatabaseBackend
{
    class PlayersCommand : Command
    {
        public PlayersCommand()
        {
            Trigger = "players";
        }

        public override async Task OnCommand(CommandArguments command)
        {
            await Task.Yield();

            if (command.Message.Length == 0)
            {
                command.Reply("Usage:{0} players <appid or partial game name>", Colors.OLIVE);
                command.Notice("Use {0}^{1} and {2}${3} just like in regex to narrow down your match, e.g:{4} !players Portal$", Colors.BLUE, Colors.NORMAL, Colors.BLUE, Colors.NORMAL, Colors.OLIVE);

                return;
            }

            uint appID;
            string name;

            if (!uint.TryParse(command.Message, out appID))
            {
                name = command.Message;

                if (!Utils.ConvertUserInputToSQLSearch(ref name))
                {
                    command.Reply("Your request is invalid or too short.");

                    return;
                }

                using (var db = Database.GetConnection())
                {
                    appID = db.ExecuteScalar<uint>("SELECT `AppID` FROM `Apps` LEFT JOIN `AppsTypes` ON `Apps`.`AppType` = `AppsTypes`.`AppType` WHERE (`AppsTypes`.`Name` IN ('game', 'application', 'video', 'hardware') AND (`Apps`.`StoreName` LIKE @Name OR `Apps`.`Name` LIKE @Name)) OR (`AppsTypes`.`Name` = 'unknown' AND `Apps`.`LastKnownName` LIKE @Name) ORDER BY `LastUpdated` DESC LIMIT 1", new { Name = name });
                }

                if (appID == 0)
                {
                    command.Reply("Nothing was found matching your request.");

                    return;
                }
            }

            KeyValue result;

            using (dynamic userStats = WebAPI.GetInterface("ISteamUserStats"))
            {
                userStats.Timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;

                try
                {
                    result = userStats.GetNumberOfCurrentPlayers(
                        appid: appID
                    );
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.Timeout)
                    {
                        throw new TaskCanceledException();
                    }

                    var response = (HttpWebResponse)e.Response;

                    command.Reply("Unable to request player count: {0}{1}", Colors.RED, response.StatusDescription);

                    return;
                }
            }

            var eResult = (EResult)result["result"].AsInteger();

            if (eResult != EResult.OK)
            {
                command.Reply("Unable to request player count: {0}{1}", Colors.RED, eResult);

                return;
            }

            if (appID == 0)
            {
                appID = 753;
            }

            string appType, type = "playing";
            name = Steam.GetAppName(appID, out appType);

            switch (appType)
            {
                case "Tool":
                case "Config":
                case "Application":
                    type = "using";
                    break;

                case "Legacy Media":
                case "Video":
                    type = "watching";
                    break;

                case "Guide":
                    type = "reading";
                    break;

                case "Hardware":
                    type = "bricking";
                    break;
            }

            command.Reply(
                "People {0} {1}{2}{3} right now: {4}{5:N0}{6} -{7} {8}",
                type,
                Colors.BLUE, name, Colors.NORMAL,
                Colors.OLIVE, result["player_count"].AsInteger(), Colors.NORMAL,
                Colors.DARKBLUE, SteamDB.GetAppURL(appID, "graphs")
            );
        }
    }
}
