using System.Collections.Generic;
using System.Diagnostics;

public class BotManager
{
    private List<Bot> _allBots = new List<Bot>();

    public int TotalBots => _allBots.Count;

    public void RegisterBot(Bot bot)
    {
        if (bot == null || _allBots.Contains(bot)) return;

        _allBots.Add(bot);
    }

    public void RemoveBot(Bot bot)
    {
       _allBots?.Remove(bot);
    }
}