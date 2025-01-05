using UnityEngine;

[CreateAssetMenu(fileName = "ChestNotifications", menuName = "ScriptableObjects/ChestNotifications", order = 1)]
public class ChestNotificationData : ScriptableObject
{
    [TextArea]
    public string[] messages;
}
