using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;

namespace NoScopeNotifier;

public class NoScopeNotifier : BasePlugin
{
    public override string ModuleName => "No Scope Notifier [Argentum Module]";
    public override string ModuleDescription => "NoScope Notifier - Extracted from Argentum framework - https://steamcommunity.com/id/kenoxyd";
    public override string ModuleAuthor => "kenoxyd";
    public override string ModuleVersion => "1.0.0";

    public override void Load(bool hotReload)
    { 
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {

        var attacker = @event.Attacker;
        var victim = @event.Userid;

        // Retrieve the player from the event

        if (victim == null)
            return HookResult.Continue;

        List<CCSPlayerController> Players = GetPlayers();

        float distance = @event.Distance;
        string formattedDistance = distance.ToString("0.0");

        // Define a dictionary for announcer messages (limited to no scope messages)
        var noScopeMessages = new Dictionary<string, string>()
                  {
                    { "Message 1", $" \x0E •\x01 Insane Quickshot! \x04{attacker.PlayerName}\x01 obliterated\x04 {victim.PlayerName}\x01 from {formattedDistance}\x04 meters!" },
                    { "Message 2", $" \x0E •\x01 Incredible Skill! \x04{attacker.PlayerName}\x01 vaporized\x04 {victim.PlayerName}\x01 from {formattedDistance}\x04 meters!" },
                    { "Message 3", $" \x0E •\x01 Unbelievable Hail Mary! \x04{attacker.PlayerName}\x01 annihilated\x04 {victim.PlayerName}\x01 from {formattedDistance}\x04 meters!" },
                  };

        // Check for noscope
        if (@event.Noscope == true)
        {
            if (!(@event.Thrusmoke || @event.Attackerinair || @event.Attackerblind))
            {
                // Get a list of no scope message keys
                var messageKeys = noScopeMessages.Keys.ToList();

                // Choose a random index within the range (0 to messageKeys.Count-1)
                var randomIndex = new Random().Next(messageKeys.Count);

                // Select the message using the random index
                var message = messageKeys[randomIndex];

                // Retrieve the message value and print it
                foreach (var Player in Players)
                {
                    if (Player == null || !Player.IsValid)
                        continue;

                    Player.PrintToChat(noScopeMessages[message]);

                }
            }
        }

        if (@event.Noscope == true)
        {
            string message = ""; // Initialize message as empty

            if (@event.Attackerblind == true)
            {
                message = "blinded"; // Start with "blinded" if attacker is blind
                if (@event.Thrusmoke == true)
                {
                    message += " through smoke";
                }
                if (@event.Attackerinair == true)
                {
                    message += " while midair";
                }
            }
            else
            {
                // Check for smoke and midair if not blinded
                if (@event.Thrusmoke == true)
                {
                    message = "through smoke";
                    if (@event.Attackerinair == true)
                    {
                        message += " while midair";
                    }
                }
                else if (@event.Attackerinair == true)
                {
                    message = "while midair";
                }
            }

            if (message.Length > 0)
            {
                foreach (var Player in Players)
                {
                    if (Player == null || !Player.IsValid)
                        continue;

                    Player.PrintToChat($" \x0E• \x04{attacker.PlayerName}\x01 no-scopes {message}, nailing \x04{victim.PlayerName}\x01 from {formattedDistance}\x04 meters!");

                }
            }
        }

        return HookResult.Continue;
    }

    public List<CCSPlayerController> GetPlayers()
    {
        List<CCSPlayerController> players = Utilities.GetPlayers();
        return players.FindAll(player => player != null && player.IsValid && player.PlayerPawn.IsValid && player.PlayerPawn.Value?.IsValid == true && player.Connected == PlayerConnectedState.PlayerConnected);
    }
}
