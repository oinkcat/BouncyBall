using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui.Storage;

namespace BouncyBall;

/// <summary>
/// Game settings
/// </summary>
public class Settings 
{
    private const string SkinSettingKey = "skin";
    
    private const string DefaultSkinName = "ball";
    
    public static Settings Instance { get; }
    
    public string SkinName { get; set; }
    
    static Settings()
    {
        Instance = new Settings();
    }
    
    private Settings()
    {
        SkinName = Preferences.Get(SkinSettingKey, DefaultSkinName);
    }
    
    public void Save()
    {
        string skinNameToSave = SkinName ?? DefaultSkinName;
        Preferences.Set(SkinSettingKey, skinNameToSave);
    }
}
