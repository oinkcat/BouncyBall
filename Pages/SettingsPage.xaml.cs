using System;
using System.IO;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BouncyBall;

/// <summary>
/// Customization options page
/// </summary>
public partial class SettingsPage : ContentPage
{
    public PlayerSkinOption[] SkinOptions { get; } =
    {
        new PlayerSkinOption("ball", "Simple red ball"),
        new PlayerSkinOption("oink", "A pig's snout")
    };
    
    public PlayerSkinOption SelectedSkin { get; set; }

    public SettingsPage()
    {
        NavigationPage.SetHasBackButton(this, true);
        InitializeComponent();

        BindingContext = this;
    }
    
    protected override void OnAppearing()
    {            
        SelectedSkin = SkinOptions.First(s => s.ImageName == Settings.Instance.SkinName);
        OnPropertyChanged(nameof(SelectedSkin));
        
        base.OnAppearing();
    }
    
    protected override bool OnBackButtonPressed()
    {
        Settings.Instance.SkinName = SelectedSkin.ImageName;
        Settings.Instance.Save();
        
        return base.OnBackButtonPressed();
    }
}
