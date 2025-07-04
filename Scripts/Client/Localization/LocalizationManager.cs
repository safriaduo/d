using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    private const string RESOURCE_PATH = "Localization";

    private static LocalizationManager instance;
    public static LocalizationManager Instance => instance;

    public static event Action OnLanguageChanged;

    private readonly Dictionary<string, Dictionary<string, string>> languages = new();
    private Dictionary<string, string> currentLanguage;

    public string CurrentLanguageId { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguages();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void LoadLanguages()
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>(RESOURCE_PATH);
        foreach (var file in files)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(file.text);
                languages[file.name] = dict ?? new Dictionary<string, string>();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse language file {file.name}: {e}");
            }
        }

        if (files.Length > 0 && currentLanguage == null)
        {
            // try to use the system language first
            string systemLang = Application.systemLanguage.ToString();
            foreach (var id in languages.Keys)
            {
                if (string.Equals(id, systemLang, StringComparison.OrdinalIgnoreCase))
                {
                    SetLanguage(id);
                    return;
                }
            }

            // fallback to the first language we loaded
            SetLanguage(files[0].name);
        }
    }

    public void SetLanguage(string id)
    {
        if (string.IsNullOrEmpty(id) || !languages.ContainsKey(id))
        {
            Debug.LogWarning($"Language '{id}' not found");
            return;
        }

        CurrentLanguageId = id;
        currentLanguage = languages[id];
        OnLanguageChanged?.Invoke();
    }

    public string Get(string key)
    {
        if (currentLanguage == null)
        {
            return key;
        }

        if (currentLanguage.TryGetValue(key, out var value))
        {
            return value;
        }

        return key;
    }
}

