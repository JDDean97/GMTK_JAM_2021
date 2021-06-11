using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class keybinds
{
    public static Dictionary<string, KeyCode> kb = new Dictionary<string, KeyCode>()
    {
        {"1left",KeyCode.A},
        {"1right", KeyCode.D},
        {"1up", KeyCode.W},
        {"1down", KeyCode.S},

        {"2left",KeyCode.LeftArrow},
        {"2right", KeyCode.RightArrow},
        {"2up", KeyCode.UpArrow},
        {"2down", KeyCode.DownArrow}
    };

    public static void save()
    {
        foreach (KeyValuePair<string, KeyCode> pair in kb)
        {
            PlayerPrefs.SetString(pair.Key, pair.Value.ToString());
        }
    }

    public static void load()
    {
        Dictionary<string, KeyCode> newKB = new Dictionary<string, KeyCode>();
        foreach (KeyValuePair<string, KeyCode> pair in kb)
        {
            if (PlayerPrefs.HasKey(pair.Key))
            {
                string keyname = PlayerPrefs.GetString(pair.Key, pair.Value.ToString());
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (k.ToString() == keyname)
                    {
                        newKB.Add(pair.Key, k);
                    }
                }
            }
            else
            {
                newKB.Add(pair.Key, pair.Value);
            }
        }
        kb = newKB;
    }
}
