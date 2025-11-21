/*
Here's how to integrate it as a proper Unity singleton:
1. Create an empty GameObject in your scene called "GameLogger"
2. Create a C# script called GameLogger.cs and attach it to that GameObject.

The singleton pattern handles creation and persistence across scenes automatically.

Now from any other script, use it like:

public class PlayerController : MonoBehaviour
{
    void Update()
    {
        // Log player position
        GameLogger.Instance.LogPlayerPositionEvent(transform.position);
        
        // Log dialogue event
        GameLogger.Instance.LogDialogueNodeEvent(1, 5);
    }
}

*/

/*
1. It uses the deferred addition of the device info. device info is added just before sending the log entry to the server.
2. the save/load of cached pending log entries is implemented using a hybrid approach that uses PlayerPrefs for WebGL and normal file io for standalone.

Deferred device info — Device info is NOT stored in the queue. 
Look at this line in SendQueuedLogs():

   logData["device"] = GetDeviceInfo();
   
This adds device info right before sending, so the queued entries stay 
small (~100-150 characters each instead of 600+). This effectively doubles 
storage capacity in PlayerPrefs.

Hybrid save/load approach. The #if UNITY_WEBGL conditional compilation handles
the platform differences:

 - WebGL builds: Use PlayerPrefs (browser storage)
 - Standalone builds: Use traditional File.WriteAllText() / File.ReadAllText() (disk I/O)
 
We'll never have to think about which storage mechanism is being used—Unity handles 
it automatically based on the build target.
*/

/*
This is the calculation *with* device info in the queued JSON.

Rough character count for JSON object: ~600 characters (minified)

Considerations:

Each entry includes the full device object (repeated for every log)
When stored in PlayerPrefs via LogListWrapper, strings are JSON-escaped, 
adding ~50-100 characters per entry (quotes become \")
Total per entry when serialized: ~650-750 characters

Conservative calculation:
1 MB = 1,024,000 bytes
Average entry size = 700 characters
1,024,000 ÷ 700 ≈ ~1,460 entries
Conservative estimate: approximately 1,000-1,200 log entries
This is a fairly generous amount for Strata Log's educational use case. 

By deferring the addition of device info until just before the JSON is sent
to the server the queue capacity is effectively double the  to 2,000-2,400 log entries.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.IO;
using Newtonsoft.Json;

public class GameLogger : MonoBehaviour
{
    private static GameLogger _instance;
    private Queue<Dictionary<string, object>> logQueue = new Queue<Dictionary<string, object>>();
    private bool isSendingLogs = false;
    private object queueLock = new object();
    
    private const string logCacheFile = "game_logs_cache.json";
    private const string logServerUrl = "<LOG_SERVER_URL_HERE>";
    private const string apiKey = "<API_KEY_HERE>";

    public static GameLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameLogger>();
                if (_instance == null)
                {
                    GameObject loggerObject = new GameObject("GameLogger");
                    _instance = loggerObject.AddComponent<GameLogger>();
                    DontDestroyOnLoad(loggerObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Cache device info once at startup
        cachedDeviceInfo = BuildDeviceInfo();
        DontDestroyOnLoad(gameObject);
        LoadCachedLogs();
    }

    private void Start()
    {
        lock (queueLock)
        {
            if (logQueue.Count > 0 && !isSendingLogs)
            {
                StartCoroutine(SendQueuedLogs());
            }
        }
    }

    public void SendToServer(Dictionary<string, object> logData)
    {
        lock (queueLock)
        {
            logQueue.Enqueue(logData);
            SaveCachedLogs();
            
            if (!isSendingLogs)
            {
                StartCoroutine(SendQueuedLogs());
            }
        }
    }

    public void LogEvent(string eventType, Dictionary<string, object> data, string eventKey = null)
    {
        var json = new Dictionary<string, object>
        {
            { "game", "mhs" },
            { "playerId", GetPlayerId() },
            { "timestamp", DateTime.UtcNow.ToString("o") },
            { "eventType", eventType },
            { "data", data }
        };
        
        if (eventKey != null)
            json["eventKey"] = eventKey;
        
        SendToServer(json);
    }

    // All 8 wrapper functions...
	
	// 1. DialogueNodeEvent
	public void LogDialogueNodeEvent(int conversationId, int nodeId)
	{
		var data = new Dictionary<string, object>
		{
			{ "dialogueEventType", "DialogueNodeEvent" },
			{ "conversationId", conversationId },
			{ "nodeId", nodeId }
		};
		
		string eventKey = $"DialogueNodeEvent:{conversationId}:{nodeId}";
		LogEvent("DialogueEvent", data, eventKey);
	}
	
	// 2. DialogueEvent
	public void LogDialogueEvent(string dialogueEventType, int conversationId)
	{
		var data = new Dictionary<string, object>
		{
			{ "dialogueEventType", dialogueEventType },
			{ "conversationId", conversationId }
		};
		
		LogEvent("DialogueEvent", data);
	}
	
	// 3. Topographic Map Event
	public void LogTopoMapEvent(string featureUsed, string actionType, Vector3 location = null)
	{
		var data = new Dictionary<string, object>
		{
			{ "featureUsed", featureUsed },
			{ "actionType", actionType }
		};
		
		if (location != null)
		{
			data["location"] = new Dictionary<string, object>
			{
				{ "x", location.x },
				{ "y", location.y },
				{ "z", location.z }
			};
		}
		
		LogEvent("TopographicMapEvent", data);
	}
	
	// 4. ArgumentationEvent
	public void LogArgumentationEvent(string eventType, string title, string description)
	{
		var data = new Dictionary<string, object>
		{
			{ "eventType", eventType },
			{ "title", title },
			{ "description", description }
		};
		
		LogEvent("ArgumentationEvent", data);
	}
	
	// 5. ArgumentationNodeEvent
	public void LogArgumentationNodeEvent(string actionType, string title, string description, string nodeName)
	{
		var data = new Dictionary<string, object>
		{
			{ "actionType", actionType },
			{ "title", title },
			{ "nodeName", nodeName }
		};
		
		LogEvent("ArgumentationNodeEvent", data);
	}
	
	// 6. ArgumentationToolEvent
	public void LogArgumentationToolEvent(string actionType, string title, string toolName, string toolState)
	{
		var data = new Dictionary<string, object>
		{
			{ "actionType", actionType },
			{ "title", title },
			{ "tool", new Dictionary<string, object>
				{
					{ "name", toolName },
					{ "state", toolState }
				}
			}
		};
		
		LogEvent("ArgumentationToolEvent", data);
	}
	
	// 7. PlayerPositionEvent
	public void LogPlayerPositionEvent(Vector3 position)
	{
		var data = new Dictionary<string, object>
		{
			{ "position", new Dictionary<string, object>
				{
					{ "x", position.x },
					{ "y", position.y },
					{ "z", position.z }
				}
			}
		};
		
		LogEvent("PlayerPositionEvent", data);
	}
	
	// 8. QuestEvent
	public void LogQuestEvent(string questEventType, int questId, string questName, string questSuccessOrFailure = null)
	{
		var data = new Dictionary<string, object>
		{
			{ "questEventType", questEventType },
			{ "questId", questId },
			{ "questName", questName }
		};
		
		if (questSuccessOrFailure != null)
		{
			data["questSF"] = questSuccessOrFailure;
		}
		
		string eventKey = $"{questEventType}:{questId}";
		LogEvent("QuestEvent", data, eventKey);
	}


    private IEnumerator SendQueuedLogs()
    {
        isSendingLogs = true;

        while (true)
        {
            Dictionary<string, object> logData = null;

            lock (queueLock)
            {
                if (logQueue.Count > 0)
                {
                    logData = logQueue.Peek();
                }
            }

            if (logData == null) break;

            if (!IsNetworkAvailable())
            {
                yield return new WaitForSeconds(5);
                continue;
            }

            logData["device"] = cachedDeviceInfo;
            string jsonData = JsonConvert.SerializeObject(logData);

            using (UnityWebRequest request = new UnityWebRequest(logServerUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Log sent successfully");

                    lock (queueLock)
                    {
                        if (logQueue.Count > 0)
                        {
                            logQueue.Dequeue();
                            SaveCachedLogs();
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Log send failed: {request.error}");
                    yield return new WaitForSeconds(10);
                }
            }
        }

        isSendingLogs = false;
    }

    private string GetPlatform()
    {
#if UNITY_WEBGL
        return "UnityWebGL";
#else
        return "Standalone";
#endif
    }

    private Dictionary<string, object> BuildDeviceInfo()
    {
        var resolution = Screen.currentResolution;
        
        return new Dictionary<string, object>
        {
            { "platform", GetPlatform() },
            { "processors", SystemInfo.processorCount },
            { "memory", SystemInfo.systemMemorySize },
            { "gdName", SystemInfo.graphicsDeviceName },
            { "gMemory", SystemInfo.graphicsMemorySize },
            { "gdApiType", SystemInfo.graphicsDeviceType.ToString() },
            { "resolution", new Dictionary<string, object>
                {
                    { "width", resolution.width },
                    { "height", resolution.height },
                    { "refreshRate", resolution.refreshRate }
                }
            },
            { "dpi", Screen.dpi },
            { "os", SystemInfo.operatingSystem }
        };
    }

    private string GetPlayerId()
    {
        // Replace with your actual player ID retrieval logic
        return "player_" + SystemInfo.deviceUniqueIdentifier;
    }

    private bool IsNetworkAvailable()
    {
#if UNITY_WEBGL
        return Application.internetReachability != NetworkReachability.NotReachable;
#else
        return true;
#endif
    }

    private void SaveCachedLogs()
    {
#if UNITY_WEBGL
        SaveCachedLogsWebGL();
#else
        SaveCachedLogsStandalone();
#endif
    }

    private void LoadCachedLogs()
    {
#if UNITY_WEBGL
        LoadCachedLogsWebGL();
#else
        LoadCachedLogsStandalone();
#endif
    }

    private void SaveCachedLogsWebGL()
    {
        try
        {
            List<string> logList = new List<string>();
            foreach (var log in logQueue)
            {
                logList.Add(JsonConvert.SerializeObject(log));
            }
            string json = JsonUtility.ToJson(new LogListWrapper(logList));
            PlayerPrefs.SetString(logCacheFile, json);
            PlayerPrefs.Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save cached logs (WebGL): {e.Message}");
        }
    }

    private void SaveCachedLogsStandalone()
    {
        string path = Path.Combine(Application.persistentDataPath, logCacheFile);
        try
        {
            List<string> logList = new List<string>();
            foreach (var log in logQueue)
            {
                logList.Add(JsonConvert.SerializeObject(log));
            }
            string json = JsonUtility.ToJson(new LogListWrapper(logList));
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save cached logs (Standalone): {e.Message}");
        }
    }

    private void LoadCachedLogsWebGL()
    {
        if (PlayerPrefs.HasKey(logCacheFile))
        {
            try
            {
                string json = PlayerPrefs.GetString(logCacheFile);
                LogListWrapper wrapper = JsonUtility.FromJson<LogListWrapper>(json);
                if (wrapper?.logs != null)
                {
                    lock (queueLock)
                    {
                        logQueue.Clear();
                        foreach (string log in wrapper.logs)
                        {
                            logQueue.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, object>>(log));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load cached logs (WebGL): {e.Message}");
            }
        }
    }

    private void LoadCachedLogsStandalone()
    {
        string path = Path.Combine(Application.persistentDataPath, logCacheFile);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                LogListWrapper wrapper = JsonUtility.FromJson<LogListWrapper>(json);
                if (wrapper?.logs != null)
                {
                    lock (queueLock)
                    {
                        logQueue.Clear();
                        foreach (string log in wrapper.logs)
                        {
                            logQueue.Enqueue(JsonConvert.DeserializeObject<Dictionary<string, object>>(log));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load cached logs (Standalone): {e.Message}");
            }
        }
    }

    [Serializable]
    private class LogListWrapper
    {
        public List<string> logs;
        public LogListWrapper(List<string> logs) => this.logs = logs;
    }
}
