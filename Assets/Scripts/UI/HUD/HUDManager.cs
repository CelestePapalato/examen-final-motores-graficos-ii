using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField]
    HUD[] Players;

    Dictionary<Player, HUD> HUDs = new Dictionary<Player, HUD>();

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        foreach(var hud in Players)
        {
            hud.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        foreach (Player player in playerBuffer)
        {
            AddPlayerToHUD(player);
        }
        playerBuffer = new List<Player>();
    }

    static List<Player> playerBuffer = new List<Player>();

    public static void AddPlayer(Player player)
    {
        if (Instance)
        {
            Instance.AddPlayerToHUD(player);
            return;
        }
        if (!playerBuffer.Contains(player))
        {
            playerBuffer.Add(player);
        }
    }

    public static void RemovePlayer(Player player)
    {
        if (Instance)
        {
            Instance.RemovePlayerFromHUD(player);
            return;
        }
    }

    private bool AddPlayerToHUD(Player player)
    {
        if (HUDs.ContainsKey(player))
        {
            UpdatePlayerFromHUD(player);
            return true;
        }
        Character character = player.Character;
        if(!character) { return false; }
        HUD[] free;
        free = Players.Where(x => x.Character == null).ToArray();
        if(free.Length == 0)
        {
            return false;
        }
        free[0].Character = character;
        free[0].gameObject.SetActive(true);
        HUDs.Add(player, free[0]);
        return true;
    }

    private void RemovePlayerFromHUD(Player player)
    {
        if (!HUDs.ContainsKey(player)) { return; }
        Character character = player.Character;
        if(!character) { return; }
        HUD[] aux = Players.Where(x =>x.Character == character).ToArray();
        if(aux.Length == 0) { return; }
        HUD active = aux[0];
        active.Character = null;
        active.gameObject.SetActive(false);
        HUDs.Remove(player);
    }

    private void UpdatePlayerFromHUD(Player player)
    {
        if (player.Character == null) { return; }
        if (!HUDs.ContainsKey(player))
        {
            if (!AddPlayerToHUD(player)) { return; }
        }
        HUD aux = HUDs[player];
        aux.Character = player.Character;
    }
}
