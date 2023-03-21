using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace BouncyBall;

/// <summary>
/// Tiled image with offset support
/// </summary>
public class OffsetTiledImage : ContentView
{
	private readonly Image testImg;
	
	private readonly StackLayout layout;
	
	private double vMargin;
	
	private double offset;
	
	public double Offset
	{
		get => offset;
		set
		{
			offset = value;
			double topMargin = (value % (int)vMargin) - vMargin;
			layout.Margin = new Thickness(0, topMargin, 0, 0);
		}
	}
	
	public void SetImageByName(string imgName)
	{	
		testImg.Source = ImageSource.FromResource(imgName);
		testImg.SizeChanged += Image_Loaded;
	}
	
	private void Image_Loaded(object sender, EventArgs e)
	{
		double imgHeight = testImg.Height;
		vMargin = Math.Ceiling(imgHeight) / 2;
		double baseHeight = Application.Current.MainPage.Height + vMargin;
		
		if(imgHeight <= 0) { return; }
		
		var stack = testImg.Parent as StackLayout;
		
		for(int i = 0; i < Math.Ceiling(baseHeight / imgHeight) - 1; i++)
		{	
			stack.Add(new Image() { Source = testImg.Source });
		}
		
		stack.Margin = new Thickness(0, -vMargin, 0, 0);
	}
	
	public OffsetTiledImage()
	{
		testImg = new Image();
		layout = new StackLayout
		{
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions = LayoutOptions.FillAndExpand,
			Children = {
				testImg
			}
		};
		
		Content = layout;
	}
}
