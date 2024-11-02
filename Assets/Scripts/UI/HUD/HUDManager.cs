using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [SerializeField]
    HUD[] Players;

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

    public bool AddPlayer(Character chara)
    {
        HUD[] free;
        free = Players.Where(x => x.Character == null).ToArray();
        if(free.Length == 0)
        {
            return false;
        }
        free[0].Character = chara;
        free[0].gameObject.SetActive(true);
        return true;
    }

    public void RemovePlayer(Character chara){
        HUD[] aux = Players.Where(x =>x.Character == chara).ToArray();
        if(aux.Length == 0) { return; }
        HUD active = aux[0];
        active.Character = null;
        active.gameObject.SetActive(false);
    }
}
