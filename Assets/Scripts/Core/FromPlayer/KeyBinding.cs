using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyBinding
{
    public string name;
    public string category;
    public KeyCode keyCodeA;
    public KeyCode keyCodeB;

    public KeyCode GetKey(string slot)
    {
        switch(slot)
        {
            case "A":
                return this.keyCodeA;
            case "B":
                return this.keyCodeB;
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), "Invalid slot keycode number");
        }
    }
}
public static class KeyBindingCtr
{
    public static Dictionary<string, KeyBinding> keyBindings = new Dictionary<string, KeyBinding>();
    public static void InitKeyBindings()
    {
        keyBindings.Add("Up", new KeyBinding { name = "Move Up", category = "Movement", keyCodeA = KeyCode.W, keyCodeB = KeyCode.UpArrow });
        keyBindings.Add("Down", new KeyBinding { name = "Move Down", category = "Movement", keyCodeA = KeyCode.S, keyCodeB = KeyCode.DownArrow });
        keyBindings.Add("Left", new KeyBinding { name = "Move Left", category = "Movement", keyCodeA = KeyCode.A, keyCodeB = KeyCode.LeftArrow });
        keyBindings.Add("Right", new KeyBinding { name = "Move Right", category = "Movement", keyCodeA = KeyCode.D, keyCodeB = KeyCode.RightArrow });
    }
    public static bool GetButton(string key)
    {
        if (keyBindings.ContainsKey(key))
        {
            KeyBinding binding = keyBindings[key];
            return Input.GetKey(binding.keyCodeA) || Input.GetKey(binding.keyCodeB);
        }
        return false;
    }
    public static bool GetButtonDown(string key)
    {
        if (keyBindings.ContainsKey(key))
        {
            KeyBinding binding = keyBindings[key];
            return Input.GetKeyDown(binding.keyCodeA) || Input.GetKeyDown(binding.keyCodeB);
        }
        return false;
    }
    public static bool GetButtonUp(string key)
    {
        if (keyBindings.ContainsKey(key))
        {
            KeyBinding binding = keyBindings[key];
            return Input.GetKeyUp(binding.keyCodeA) || Input.GetKeyUp(binding.keyCodeB);
        }
        return false;
    }
    public static void SetKey(string key, KeyCode newKeyA, KeyCode newKeyB)
    {
        if (keyBindings.ContainsKey(key))
        {
            KeyBinding binding = keyBindings[key];
            binding.keyCodeA = newKeyA;
            binding.keyCodeB = newKeyB;
        }
    }
    public static void SetKey(string key, string slot, KeyCode newKey)
    {
        if (keyBindings.ContainsKey(key))
        {
            KeyBinding binding = keyBindings[key];
            switch (slot)
            {
                case "A":
                    binding.keyCodeA = newKey;
                    break;
                case "B":
                    binding.keyCodeB = newKey;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot), "Invalid slot keycode number");
            }
        }
    }

}