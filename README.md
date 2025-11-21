# UnityMHSGameLogger

[Project Summary](./ProjectSummary.md) - detailed project overview

[Quick Reference](./QuickReference.md) - quick reference for logging functions

## Unity game logger for the game Mission HydroSci

Here's how to integrate it as a proper Unity singleton:
1. Create an empty GameObject in your scene called "GameLogger"
2. Create a C# script called GameLogger.cs and attach it to that GameObject.

The singleton pattern handles creation and persistence across scenes automatically.

Now from any other script, use it like:

```
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float lastPositionLogTime = 0f;
    private float positionLogInterval = 2f; // Log position every 2 seconds

    void Update()
    {
        // Log player position periodically
        if (Time.time - lastPositionLogTime >= positionLogInterval)
        {
            LogPlayerPositionExamples();
            lastPositionLogTime = Time.time;
        }
    }

    /// <summary>
    /// Example 1: Dialogue Events
    /// </summary>
    public void DialogueExamples()
    {
        // Log when player reaches a dialogue node
        GameLogger.Instance.LogDialogueNodeEvent(
            conversationId: 1,
            nodeId: 5
        );

        // Log when dialogue starts
        GameLogger.Instance.LogDialogueEvent(
            dialogueEventType: "DialogueStartEvent",
            conversationId: 1
        );

        // Log when dialogue finishes
        GameLogger.Instance.LogDialogueEvent(
            dialogueEventType: "DialogueFinishEvent",
            conversationId: 1
        );
    }

    /// <summary>
    /// Example 2: Topographic Map Events
    /// </summary>
    public void TopoMapExamples()
    {
        // Log when map opens
        GameLogger.Instance.LogTopoMapEvent(
            featureUsed: "Map",
            actionType: "MapOpenEvent"
        );

        // Log when map closes
        GameLogger.Instance.LogTopoMapEvent(
            featureUsed: "Map",
            actionType: "MapCloseEvent"
        );

        // Log when waypoint drag starts
        GameLogger.Instance.LogTopoMapEvent(
            featureUsed: "Waypoint",
            actionType: "DragStart",
            location: new Vector3(100.5f, 50.2f, -25.0f)
        );

        // Log when waypoint drag ends
        GameLogger.Instance.LogTopoMapEvent(
            featureUsed: "Waypoint",
            actionType: "DragEnd",
            location: new Vector3(105.5f, 50.2f, -20.0f)
        );
    }

    /// <summary>
    /// Example 3: Argumentation Session Events
    /// </summary>
    public void ArgumentationSessionExamples()
    {
        // Log when argumentation session opens
        GameLogger.Instance.LogArgumentationEvent(
            eventType: "ArgumentationSessionOpen",
            title: "Climate Change Debate",
            description: "Students construct arguments about climate policy impacts"
        );

        // Log when argumentation session closes
        GameLogger.Instance.LogArgumentationEvent(
            eventType: "ArgumentationSessionClose",
            title: "Climate Change Debate",
            description: "Students construct arguments about climate policy impacts"
        );
    }

    /// <summary>
    /// Example 4: Argumentation Node Events
    /// </summary>
    public void ArgumentationNodeExamples()
    {
        // Log when creating a claim node
        GameLogger.Instance.LogArgumentationNodeEvent(
            actionType: "CreateNode",
            title: "Climate Change Debate",
            description: "Player created a new claim",
            nodeName: "claim_fossil_fuels"
        );

        // Log when adding evidence to a node
        GameLogger.Instance.LogArgumentationNodeEvent(
            actionType: "AddEvidence",
            title: "Climate Change Debate",
            description: "Player added supporting evidence",
            nodeName: "claim_fossil_fuels"
        );

        // Log when modifying a node
        GameLogger.Instance.LogArgumentationNodeEvent(
            actionType: "ModifyNode",
            title: "Climate Change Debate",
            description: "Player modified the argument",
            nodeName: "claim_fossil_fuels"
        );

        // Log when deleting a node
        GameLogger.Instance.LogArgumentationNodeEvent(
            actionType: "DeleteNode",
            title: "Climate Change Debate",
            description: "Player deleted a node",
            nodeName: "claim_fossil_fuels"
        );
    }

    /// <summary>
    /// Example 5: Argumentation Tool Events
    /// </summary>
    public void ArgumentationToolExamples()
    {
        // Log when opening the Evidence Builder tool
        GameLogger.Instance.LogArgumentationToolEvent(
            actionType: "ArgumentationToolOpen",
            title: "Climate Change Debate",
            toolName: "EvidenceBuilder",
            toolState: "active"
        );

        // Log when opening the Counter Argument tool
        GameLogger.Instance.LogArgumentationToolEvent(
            actionType: "ArgumentationToolOpen",
            title: "Climate Change Debate",
            toolName: "CounterArgumentTool",
            toolState: "active"
        );

        // Log when minimizing a tool
        GameLogger.Instance.LogArgumentationToolEvent(
            actionType: "ArgumentationToolClose",
            title: "Climate Change Debate",
            toolName: "EvidenceBuilder",
            toolState: "minimized"
        );

        // Log when closing a tool completely
        GameLogger.Instance.LogArgumentationToolEvent(
            actionType: "ArgumentationToolClose",
            title: "Climate Change Debate",
            toolName: "CounterArgumentTool",
            toolState: "closed"
        );
    }

    /// <summary>
    /// Example 6: Player Position Events
    /// </summary>
    public void LogPlayerPositionExamples()
    {
        // Log using current transform position
        GameLogger.Instance.LogPlayerPositionEvent(
            position: transform.position
        );

        // Log using explicit coordinates
        GameLogger.Instance.LogPlayerPositionEvent(
            position: new Vector3(150.5f, 10.0f, -200.3f)
        );
    }

    /// <summary>
    /// Example 7: Quest Events
    /// </summary>
    public void QuestExamples()
    {
        // Log when player activates a quest
        GameLogger.Instance.LogQuestEvent(
            questEventType: "QuestActiveEvent",
            questId: 28,
            questName: "Investigate the Mountain Pass"
        );

        // Log when player successfully completes a quest
        GameLogger.Instance.LogQuestEvent(
            questEventType: "QuestFinishEvent",
            questId: 28,
            questName: "Investigate the Mountain Pass",
            questSuccessOrFailure: "success"
        );

        // Log when player fails a quest
        GameLogger.Instance.LogQuestEvent(
            questEventType: "QuestFinishEvent",
            questId: 28,
            questName: "Investigate the Mountain Pass",
            questSuccessOrFailure: "failure"
        );

        // Log activation of another quest
        GameLogger.Instance.LogQuestEvent(
            questEventType: "QuestActiveEvent",
            questId: 42,
            questName: "Find the Hidden Treasure"
        );

        // Log successful completion of another quest
        GameLogger.Instance.LogQuestEvent(
            questEventType: "QuestFinishEvent",
            questId: 42,
            questName: "Find the Hidden Treasure",
            questSuccessOrFailure: "success"
        );
    }

    /// <summary>
    /// Master example showing all logging functions in sequence
    /// (Call this method to test all logging variations)
    /// </summary>
    public void LogAllExamples()
    {
        Debug.Log("=== Logging All Event Examples ===");
        
        DialogueExamples();
        Debug.Log("✓ Dialogue events logged");
        
        TopoMapExamples();
        Debug.Log("✓ Topographic map events logged");
        
        ArgumentationSessionExamples();
        Debug.Log("✓ Argumentation session events logged");
        
        ArgumentationNodeExamples();
        Debug.Log("✓ Argumentation node events logged");
        
        ArgumentationToolExamples();
        Debug.Log("✓ Argumentation tool events logged");
        
        LogPlayerPositionExamples();
        Debug.Log("✓ Player position events logged");
        
        QuestExamples();
        Debug.Log("✓ Quest events logged");
        
        Debug.Log("=== All logging examples complete ===");
    }
}
```

# Strata Hub Logging Functions - Quick Reference

## 1. LogDialogueNodeEvent

**Function Signature:**
```csharp
public void LogDialogueNodeEvent(int conversationId, int nodeId)
```

**Example Call:**
```csharp
GameLogger.Instance.LogDialogueNodeEvent(
    conversationId: 12,
    nodeId: 5
);
```

---

## 2. LogDialogueEvent

**Function Signature:**
```csharp
public void LogDialogueEvent(string dialogueEventType, int conversationId)
```

**Parameter Options:**
- `dialogueEventType`: `"DialogueStartEvent"` | `"DialogueFinishEvent"`

**Example Call:**
```csharp
GameLogger.Instance.LogDialogueEvent(
    dialogueEventType: "DialogueStartEvent",
    conversationId: 12
);
```

---

## 3. LogTopoMapEvent

**Function Signature:**
```csharp
public void LogTopoMapEvent(string featureUsed, string actionType, Vector3 location = null)
```

**Parameter Options:**
- `featureUsed`: `"Map"` | `"Waypoint"`
- `actionType` (for Map): `"MapOpenEvent"` | `"MapCloseEvent"`
- `actionType` (for Waypoint): `"DragStart"` | `"DragEnd"`
- `location`: Vector3 position (only used with Waypoint feature)

**Example Calls:**

Without location (Map feature):
```csharp
GameLogger.Instance.LogTopoMapEvent(
    featureUsed: "Map",
    actionType: "MapOpenEvent"
);
```

With location (Waypoint feature):
```csharp
GameLogger.Instance.LogTopoMapEvent(
    featureUsed: "Waypoint",
    actionType: "DragStart",
    location: new Vector3(100.5f, 50.2f, -25.0f)
);
```

---

## 4. LogArgumentationEvent

**Function Signature:**
```csharp
public void LogArgumentationEvent(string eventType, string title, string description)
```

**Parameter Options:**
- `eventType`: `"ArgumentationSessionOpen"` | `"ArgumentationSessionClose"`

**Example Call:**
```csharp
GameLogger.Instance.LogArgumentationEvent(
    eventType: "ArgumentationSessionOpen",
    title: "Climate Change Debate",
    description: "Students construct arguments about climate policy impacts"
);
```

---

## 5. LogArgumentationNodeEvent

**Function Signature:**
```csharp
public void LogArgumentationNodeEvent(string actionType, string title, string description, string nodeName)
```

**Example Call:**
```csharp
GameLogger.Instance.LogArgumentationNodeEvent(
    actionType: "CreateNode",
    title: "Climate Change Debate",
    description: "Player created a new claim",
    nodeName: "claim_fossil_fuels"
);
```

---

## 6. LogArgumentationToolEvent

**Function Signature:**
```csharp
public void LogArgumentationToolEvent(string actionType, string title, string toolName, string toolState)
```

**Parameter Options:**
- `actionType`: `"ArgumentationToolOpen"` | `"ArgumentationToolClose"`

**Example Call:**
```csharp
GameLogger.Instance.LogArgumentationToolEvent(
    actionType: "ArgumentationToolOpen",
    title: "Climate Change Debate",
    toolName: "EvidenceBuilder",
    toolState: "active"
);
```

---

## 7. LogPlayerPositionEvent

**Function Signature:**
```csharp
public void LogPlayerPositionEvent(Vector3 position)
```

**Example Calls:**

Using current transform:
```csharp
GameLogger.Instance.LogPlayerPositionEvent(
    position: transform.position
);
```

Using explicit coordinates:
```csharp
GameLogger.Instance.LogPlayerPositionEvent(
    position: new Vector3(150.5f, 10.0f, -200.3f)
);
```

---

## 8. LogQuestEvent

**Function Signature:**
```csharp
public void LogQuestEvent(string questEventType, int questId, string questName, string questSuccessOrFailure = null)
```

**Parameter Options:**
- `questEventType`: `"QuestActiveEvent"` | `"QuestFinishEvent"`
- `questSuccessOrFailure`: `"success"` | `"failure"` (only used with `questEventType: "QuestFinishEvent"`, optional for other types)

**Example Calls:**

Quest activation:
```csharp
GameLogger.Instance.LogQuestEvent(
    questEventType: "QuestActiveEvent",
    questId: 28,
    questName: "Investigate the Mountain Pass"
);
```

Quest completion (success):
```csharp
GameLogger.Instance.LogQuestEvent(
    questEventType: "QuestFinishEvent",
    questId: 28,
    questName: "Investigate the Mountain Pass",
    questSuccessOrFailure: "success"
);
```

Quest completion (failure):
```csharp
GameLogger.Instance.LogQuestEvent(
    questEventType: "QuestFinishEvent",
    questId: 28,
    questName: "Investigate the Mountain Pass",
    questSuccessOrFailure: "failure"
);
```

