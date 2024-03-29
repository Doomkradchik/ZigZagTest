using UnityEngine;
using System.IO;

[System.Serializable]
public struct StatsConfig
{
    public int _bestScore;
    public int _diamonds;
    public int _games;
}

public static class StatsRepository
{
    public static readonly string _path = Application.persistentDataPath + "/stats.json";
    public static void Save(StatsConfig config)
    {
        FileStream fileStream = new FileStream(_path, FileMode.OpenOrCreate);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            string data = JsonUtility.ToJson(config);
            writer.Write(data);
        }
    }

    public static StatsConfig Load()
    {
        if (File.Exists(_path) == false)
            return new StatsConfig();
        string json;
        using (StreamReader reader = new StreamReader(_path))
        {
            json = reader.ReadToEnd();
        }
        if (string.IsNullOrEmpty(json))
            return new StatsConfig();

        return JsonUtility.FromJson<StatsConfig>(json);
    }
}
