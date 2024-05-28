using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Localization;
using System.Numerics;

namespace NoScopeNotifier;

public class NoScopeNotifier : BasePlugin
{
    public override string ModuleName => "No Scope Notifier [Argentum Module]";
    public override string ModuleDescription => "NoScope Notifier - Extracted from Argentum framework - https://steamcommunity.com/id/kenoxyd";
    public override string ModuleAuthor => "kenoxyd & Astral Custom Lang";
    public override string ModuleVersion => "1.1.0";

    internal static IStringLocalizer? Stringlocalizer;
    private const char NewLine = '\u2029';



    public override void Load(bool hotReload)
    { 
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }


    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        // Early returns for invalid or irrelevant events
        if (victim == null || attacker == null || !@event.Noscope)
        {
            return HookResult.Continue;
        }

        if (!@event.Noscope)
            return HookResult.Continue;

        float distance = @event.Distance;
        string formattedDistance = distance.ToString("0.0");

        var noScopeMessages = new List<string>
        {
            $" \x0E •\x01 {Localizer["chat.Insane"]} \x04{attacker.PlayerName}\x01 {Localizer["chat.obliterated"]}\x04{victim.PlayerName}\x01 , {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}",
            $" \x0E •\x01 {Localizer["chat.Incredible"]} \x04{attacker.PlayerName}\x01 {Localizer["chat.vaporized"]}\x04{victim.PlayerName}\x01 , {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}",
            $" \x0E •\x01 {Localizer["chat.Unbelievable"]} \x04{attacker.PlayerName}\x01 {Localizer["chat.annihilated"]}\x04{victim.PlayerName}\x01, {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}"
    };

        if (@event.Noscope)
        {
            // Announce no scope kill without special conditions
            if (!@event.Thrusmoke && !@event.Attackerinair && !@event.Attackerblind)
            {
                var randomIndex = new Random().Next(noScopeMessages.Count);
                Server.PrintToChatAll(noScopeMessages[randomIndex]);
            }
            else
            {
                string message = "";

                if (@event.Attackerblind)
                {

                    if (@event.Thrusmoke)
                    {

                        attacker?.PrintToChat($" \u000e• \u0004{attacker?.PlayerName}\u0001 no-scopes, {Localizer["chat.blinded"]}, {Localizer["chat.throughsmoke"]}, {Localizer["chat.nailing"]}, \x04{victim.PlayerName}\x01, {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}");
                    }
                    if (@event.Attackerinair)
                    {

                        attacker?.PrintToChat($" \u000e• \u0004{attacker?.PlayerName}\u0001 no-scopes, {Localizer["chat.blinded"]}, {Localizer["chat.whilemidair"]}, {Localizer["chat.nailing"]}, \x04{victim.PlayerName}\x01,  {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}");
                    }
                }
                else
                {
                    if (@event.Thrusmoke)
                    {
                        attacker?.PrintToChat($" \u000e• \u0004{attacker?.PlayerName}\u0001 no-scopes, {Localizer["chat.throughsmoke"]}, {Localizer["chat.nailing"]}, \x04{victim.PlayerName}\x01, {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}");
                    }
                    if (@event.Attackerinair)
                    {
                        message += (message.Length > 0 ? " " : "") + Localizer["chat.whilemidair"];
                        attacker?.PrintToChat($" \u000e• \u0004{attacker?.PlayerName}\u0001 no-scopes, {message} , {Localizer["chat.nailing"]}, \x04{victim.PlayerName}\x01 , {Localizer["chat.from"]}, {formattedDistance}\x04, {Localizer["chat.meters"]}");
                    }
                }
                var callerName = attacker == null ? "Console" : attacker.PlayerName;
                attacker?.ExecuteClientCommand($"play sounds/frozen_music2/punishment1.vsnd_c");

              

                //Server.PrintToChatAll($" \x0E• \x04{attacker?.PlayerName}\x01 no-scopes, {message}, nailing \x04{victim.PlayerName}\x01 from {formattedDistance}\x04 meters!");
            }
        }

        return HookResult.Continue;
    }

}
