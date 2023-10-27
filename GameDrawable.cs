using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using BouncyBall.Game;

namespace BouncyBall;

/// <summary>
/// Draws game graphics
/// </summary>
public class GameDrawable : IDrawable
{
    private const int BlockTexturesCount = 7;
        
    private readonly GameLogic game;
    
    private float width;
    
    private float height;
    
    private IImage[] blockTextures;
    
    private IImage backTexture;
    
    private IImage ballTexture;
    
    public GameDrawable(GameLogic game)
    {
        this.game = game;
    }
    
    public void Initialize(double width, double height, string skinName)
    {
        const string BackTextureName = "BouncyBall.resources.back.png";
        
        this.width = (float)width;
        this.height = (float)height;
        
        blockTextures = Enumerable.Range(1, BlockTexturesCount)
			.Select(i => LoadImage($"BouncyBall.resources.brick{i}.png"))
			.ToArray();
            
        backTexture = LoadImage(BackTextureName);
        ballTexture = LoadImage($"BouncyBall.resources.{skinName}.png");
    }
    
    private IImage LoadImage(string name)
    {
        var imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        return PlatformImage.FromStream(imgStream);
    }
    
    public void Draw(ICanvas canvas, RectF rect)
    {
        if(width <= 0 || height <= 0) { return; }
        
        canvas.Antialias = false;
        
        DrawBackground(canvas);
        DrawBlocks(canvas);
        DrawBall(canvas);
        DrawInfo(canvas);
    }
    
    private void DrawBackground(ICanvas canvas)
    {
        float r = backTexture.Height / backTexture.Width;
        float tileHeight = width * r;
        
        float startY = (int)game.BaseLine / 2 % (int)tileHeight;
        startY = (startY > 0) ? startY - tileHeight : 0;
        
        for(float y = startY; y < height; y += tileHeight)
        {
            canvas.DrawImage(backTexture, 0, y, width, tileHeight);
        }
    }
    
    private void DrawBlocks(ICanvas canvas)
    {
        const int CollapsingTextureIndex = 4;
        
        foreach(var entity in game.Obstacles.ToArray())
        {
            float x = (float)entity.X;
            float y = ConvertEntityYCoord(entity);
            
            if(entity is MovingBlock)
            {
                float ly = y + entity.Height / 2;
                canvas.StrokeColor = Colors.DarkGray;
                canvas.DrawLine(0, ly, width, ly);
                canvas.StrokeColor = Colors.Gray;
                canvas.DrawLine(0, ly + 1, width, ly + 1);
            }
            
            var texture = entity switch
			{
				MovingBlock mb => (mb.Velocity.X < 0)
                                        ? blockTextures[1]
                                        : blockTextures[2],
				BouncyBlock => blockTextures[3],
                CollapsingBlock cb => blockTextures[CollapsingTextureIndex + cb.Phase],
				_ => blockTextures[0]
			};
            canvas.DrawImage(texture, x, y, entity.Width, entity.Height);
        }
    }
    
    private void DrawBall(ICanvas canvas)
    {    
        float ballX = (float)game.Ball.X;
        float ballY = ConvertEntityYCoord(game.Ball);
        canvas.Alpha = game.Ball.JumpsLeft > 0 ? 1.0f : 0.5f;
        canvas.DrawImage(ballTexture, ballX, ballY, game.Ball.Width, game.Ball.Height);
    }
    
    private void DrawInfo(ICanvas canvas)
    {
        const float ScoreX = 5;
        const float ScoreY = 20;
        const float JumpsInfoMargin = 20;
        
        canvas.FontSize = 16;
        
        // Jumps count
        string jumpsLeftText = game.Ball.JumpsLeft.ToString();
        float ballY = ConvertEntityYCoord(game.Ball);
        float textX = (float)game.Ball.X + game.Ball.Width / 2;
        float textY = ballY + JumpsInfoMargin;
        canvas.FontColor = Colors.Yellow;
        canvas.DrawString(jumpsLeftText, textX, textY, HorizontalAlignment.Center);
        
        // Score
        var scoreText = game.Score.ToString().PadLeft(5, '0');
        canvas.FontColor = Colors.White;
        canvas.DrawString(scoreText, ScoreX, ScoreY, HorizontalAlignment.Left);
    }
    
    private float ConvertEntityYCoord(Entity entity)
    {
        return (float)(height - entity.Y - entity.Height + game.BaseLine);
    }
}
