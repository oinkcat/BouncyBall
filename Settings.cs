﻿using System;
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
    
    private const string RandomnessKey = "randomness";
    
    private const string ExtraBouncyKey = "extra_bouncy";
    
    private const string DefaultSkinName = "ball";
    
    private const int DefaultRandomness = 5;
    
    public static Settings Instance { get; }
    
    public string SkinName { get; set; }
    
    public int RandomnessLevel { get; set; }
    
    public bool ExtraBouncy { get; set; }
    
    static Settings()
    {
        Instance = new Settings();
    }
    
    private Settings()
    {
        SkinName = Preferences.Get(SkinSettingKey, DefaultSkinName);
        RandomnessLevel = Preferences.Get(RandomnessKey, DefaultRandomness);
        ExtraBouncy = Preferences.Get(ExtraBouncyKey, false);
    }
    
    public void Save()
    {
        string skinNameToSave = SkinName ?? DefaultSkinName;
        Preferences.Set(SkinSettingKey, skinNameToSave);
        Preferences.Set(RandomnessKey, RandomnessLevel);
        Preferences.Set(ExtraBouncyKey, ExtraBouncy);
    }
}
