using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


public class GameSaveManager
{
    /// <summary>
    /// 오브젝트를 JSON으로 직렬화하여 파일로 저장합니다.
    /// </summary>
    /// <param name="data">저장할 오브젝트</param>
    /// <param name="relativePath">Application.persistentDataPath 기준 상대경로 (예: "saves/player.json")</param>
    /// <param name="overwrite">이미 파일이 존재할 경우 덮어쓸지 여부</param>
    /// <returns>저장 성공 여부</returns>
    public bool Write<T>(T data, string relativePath, bool overwrite = true)
    {
        string fullPath = GetFullPath(relativePath);

        if (!overwrite && File.Exists(fullPath))
        {
            Debug.LogWarning($"[GameSaveManager] 파일이 이미 존재하며 덮어쓰기가 비활성화되어 있습니다: {fullPath}");
            return false;
        }

        try
        {
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(fullPath, json);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameSaveManager] 파일 저장 실패 ({fullPath}): {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// JSON 파일을 읽어 오브젝트로 역직렬화하여 반환합니다.
    /// </summary>
    /// <param name="relativePath">Application.persistentDataPath 기준 상대경로 (예: "saves/player.json")</param>
    /// <returns>역직렬화된 오브젝트. 파일이 없거나 오류 시 default 반환</returns>
    public T Read<T>(string relativePath)
    {
        string fullPath = GetFullPath(relativePath);

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"[GameSaveManager] 파일을 찾을 수 없습니다: {fullPath}");
            return default;
        }

        try
        {
            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameSaveManager] 파일 읽기 실패 ({fullPath}): {e.Message}");
            return default;
        }
    }

    private string GetFullPath(string relativePath)
    {
        return Path.Combine(Application.persistentDataPath, relativePath);
    }
}