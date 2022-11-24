﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace BouncyBall
{
	/// <summary>
	/// Top player's score
	/// </summary>
	public struct HighScoreEntry
	{
		public DateTime Date { get; set; }
		
		public int Score { get; set; }
		
		public override string ToString() => $"{Date.ToShortDateString()} {Score}";
	}
	
	/// <summary>
	/// Store top players' score
	/// </summary>
    public class HighScoreTable
    {         
        private const string TableFileName = "highscore.txt";
        
        private const int NUM_SCORES = 10;
        
        public List<HighScoreEntry> Scores { get; }
        
        public HighScoreTable() => Scores = new List<HighScoreEntry>();
        
        public bool TryStorePlayerScore(int score, out int position)
        {
        	bool isTop = (score > 0) &&
        				 ((Scores.Count < NUM_SCORES) ||
        				 (score > Scores.Last().Score));
        				 
        	if(isTop)
        	{
        		var newEntry = new HighScoreEntry
        		{
        			Date = DateTime.Now,
        			Score = score
        		};
        		
        		Scores.Add(newEntry);
        		var newScores = Scores
        			.OrderByDescending(s => s.Score)
        			.Take(NUM_SCORES)
        			.ToList();
        			
        		Scores.Clear();
        		Scores.AddRange(newScores);
        		
        		position = Scores.FindIndex(s => s.Date == newEntry.Date) + 1;
        	}
        	else
        	{
        		position = default;
        	}
        	
        	return isTop;
        }
        
        public void Load()
        {
        	Scores.Clear();
        	string scoreFilePath = GetTableFilePath();
        	
        	if(File.Exists(scoreFilePath))
        	{
        		var loadedTopScores = File.ReadAllLines(scoreFilePath)
        			.Select(l => {
        				var parts = l.Split();
        				return new HighScoreEntry
        				{
        					Date = DateTime.Parse(parts[0]),
        					Score = int.Parse(parts[1])
        				};
        			});
        			
        		Scores.AddRange(loadedTopScores);
        	}
        }
        
        public void Save()
        {
        	var scoreItemLines = Scores.Select(s => s.ToString()).ToArray();
        	File.WriteAllLines(GetTableFilePath(), scoreItemLines);
        }
        
        private string GetTableFilePath()
        {
        	var localDataFolder = Environment.SpecialFolder.LocalApplicationData;
        	string dataDir = Environment.GetFolderPath(localDataFolder);
        	return Path.Combine(dataDir, TableFileName);
        }
    }
}