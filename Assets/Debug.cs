using UnityEngine;

public class ShowDevConsole : MonoBehaviour
{
    private string logOutput = "";
    private Vector2 scrollPosition;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
{
    if (type == LogType.Error || type == LogType.Exception)
    {
        logOutput += "<color=red>" + logString + "\n" + stackTrace + "</color>\n";
    }
    else if (type == LogType.Warning)
    {
        logOutput += "<color=yellow>" + logString + "</color>\n";
    }
    else
    {
        logOutput += logString + "\n";
    }
}

    void OnGUI()
{
    GUIStyle style = new GUIStyle();
    style.fontSize = 20;
    style.wordWrap = true;
    style.richText = true; // add this line
    style.normal.textColor = Color.white;

    scrollPosition = GUILayout.BeginScrollView(
        scrollPosition,
        GUILayout.Width(Screen.width),
        GUILayout.Height(200)
    );

    GUILayout.Label(logOutput, style); // single label instead of loop

    GUILayout.EndScrollView();

    if (GUILayout.Button("Clear")) logOutput = "";
}
}