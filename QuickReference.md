# GameLogger Wrapper Functions - Quick Reference

## 1. LogDialogueNodeEvent
```csharp
public void LogDialogueNodeEvent(int conversationId, int nodeId)
```

---

## 2. LogDialogueEvent
```csharp
public void LogDialogueEvent(string dialogueEventType, int conversationId)
```
**dialogueEventType:** `"DialogueStartEvent"` | `"DialogueFinishEvent"`

---

## 3. LogTopoMapEvent
```csharp
public void LogTopoMapEvent(string featureUsed, string actionType, Vector3 location = null)
```
**featureUsed:** `"Map"` | `"Waypoint"`

**actionType (Map):** `"MapOpenEvent"` | `"MapCloseEvent"`

**actionType (Waypoint):** `"DragStart"` | `"DragEnd"`

**location:** Vector3 (Waypoint only)

---

## 4. LogArgumentationEvent
```csharp
public void LogArgumentationEvent(string eventType, string title, string description)
```
**eventType:** `"ArgumentationSessionOpen"` | `"ArgumentationSessionClose"`

---

## 5. LogArgumentationNodeEvent
```csharp
public void LogArgumentationNodeEvent(string actionType, string title, string description, string nodeName)
```

---

## 6. LogArgumentationToolEvent
```csharp
public void LogArgumentationToolEvent(string actionType, string title, string toolName, string toolState)
```
**actionType:** `"ArgumentationToolOpen"` | `"ArgumentationToolClose"`

---

## 7. LogPlayerPositionEvent
```csharp
public void LogPlayerPositionEvent(Vector3 position)
```

---

## 8. LogQuestEvent
```csharp
public void LogQuestEvent(string questEventType, int questId, string questName, string questSuccessOrFailure = null)
```
**questEventType:** `"QuestActiveEvent"` | `"QuestFinishEvent"`

**questSuccessOrFailure:** `"success"` | `"failure"` (QuestFinishEvent only)
