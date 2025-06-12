using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dawnshard.Database;
using UnityEngine;

public class TextFormatter {
    public static string BoldifyText(string text, List<string> searchPhrases)
    {
        if (string.IsNullOrEmpty(text) || searchPhrases == null || searchPhrases.Count == 0)
            return text;

        // Build a single regex pattern that matches any of the search phrases.
        // Each phrase is escaped to handle special regex characters.
        string pattern = string.Join("|", searchPhrases.Select(phrase => Regex.Escape(phrase)));

        // Use Regex.Replace with a MatchEvaluator to wrap each match in <b> tags.
        // RegexOptions.IgnoreCase makes the search case-insensitive.
        string result = Regex.Replace(text, pattern, match => $"<b>{match.Value}</b>", RegexOptions.IgnoreCase);
        return result;
    }
    
    public static string ItalicizeText(string text, List<string> searchPhrases)
    {
        if (string.IsNullOrEmpty(text) || searchPhrases == null || searchPhrases.Count == 0)
            return text;

        // Build a single regex pattern that matches any of the search phrases.
        // Each phrase is escaped to handle special regex characters.
        string pattern = string.Join("|", searchPhrases.Select(phrase => Regex.Escape(phrase)));

        // Use Regex.Replace with a MatchEvaluator to wrap each match in <b> tags.
        // RegexOptions.IgnoreCase makes the search case-insensitive.
        string result = Regex.Replace(text, pattern, match => $"<i>{match.Value}</i>", RegexOptions.IgnoreCase);
        return result;
    }
    
    public static string ReplaceTextWithSpriteTags(string text, Dictionary<string,string> searchPhrases)
    {
        if (string.IsNullOrEmpty(text) || searchPhrases == null || searchPhrases.Count == 0)
            return text;
        
        // Build a single regex pattern that matches any of the search phrases.
        // Regex.Escape is used to escape any special characters.
        string pattern = string.Join("|", searchPhrases.Select(phrase => Regex.Escape(phrase.Key)));
        
        // Use Regex.Replace with a callback to wrap each match with the sprite tag.
        // The sprite tag uses the matched string as the sprite name.
        string result = Regex.Replace(
            text,
            pattern,
            match => $"<sprite name=\"{searchPhrases[match.Value]}\">",
            RegexOptions.IgnoreCase
        );
        
        return result;
    }
}
