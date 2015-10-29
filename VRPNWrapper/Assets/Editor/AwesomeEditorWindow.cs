using UnityEngine;
using System.Collections;
using UnityEditor;

public class AwesomeEditorWindow : EditorWindow
{
    [MenuItem("Awesome Stuff/Awesome Window")]
    public static void ShowAwesomeEditorWindow()
    {
        AwesomeEditorWindow window = GetWindow<AwesomeEditorWindow>();
        window.titleContent.text = "The Window";
    }

    private Color[] pixels;
    private int width;
    private int height;
    private Texture pixelTexture;
    private bool isEnabled = false;

    public void OnEnable()
    {
        if (!isEnabled)
        {
            isEnabled = true;

            width = height = 8;
            pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color(Random.value, Random.value, Random.value, 1.0f);

            pixelTexture = EditorGUIUtility.whiteTexture;
        }
        
    }

    private Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1.0f);
    }

    public void OnGUI()
    {
        Event evt = Event.current;

        Color oldColor = GUI.color;
        GUILayout.BeginHorizontal();
        for (int i = 0; i < width; i++)
        {
            GUILayout.BeginVertical();
            for (int j = 0; j < height; j++)
            {
                int index = j + i * height;

                GUI.color = pixels[index];
                Rect pixelRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUI.DrawTexture(pixelRect, pixelTexture);
                if (evt.type == EventType.MouseDown && pixelRect.Contains(evt.mousePosition))
                {
                    pixels[index] = RandomColor();

                    evt.Use();
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUI.color = oldColor;
    }
}
