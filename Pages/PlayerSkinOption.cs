using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace BouncyBall;

public record PlayerSkinOption(string ImageName, string Description)
{
    public ImageSource SkinImage
    {
        get => ImageSource.FromResource($"BouncyBall.resources.{ImageName}.png");
    }
}