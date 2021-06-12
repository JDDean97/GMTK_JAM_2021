using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class keybinds
{
    public static Dictionary<string, KeyCode> kb1 = new Dictionary<string, KeyCode>()
    {
        {"left",KeyCode.A},
        {"right", KeyCode.D},
        {"up", KeyCode.W},
        {"down", KeyCode.S},
    };

    public static Dictionary<string, KeyCode> kb2 = new Dictionary<string, KeyCode>()
    {
        {"left",KeyCode.J},
        {"right", KeyCode.L},
        {"up", KeyCode.I},
        {"down", KeyCode.K}
    };

    public static void save()
    {
        foreach (KeyValuePair<string, KeyCode> pair in kb1)
        {
            PlayerPrefs.SetString(pair.Key, pair.Value.ToString());
        }
        foreach (KeyValuePair<string, KeyCode> pair in kb2)
        {
            PlayerPrefs.SetString(pair.Key, pair.Value.ToString());
        }
    }

    public static void load()
    {
        Dictionary<string, KeyCode> newKB = new Dictionary<string, KeyCode>();
        foreach (KeyValuePair<string, KeyCode> pair in kb1)
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
        kb1 = newKB;

        newKB = new Dictionary<string, KeyCode>();
        foreach (KeyValuePair<string, KeyCode> pair in kb2)
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
        kb2 = newKB;
    }
}
