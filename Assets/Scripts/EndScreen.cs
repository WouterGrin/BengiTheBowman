using UnityEngine;
using System.Collections;

public class EndScreen : MonoBehaviour
{

    public Texture cursorTex;
    public Font customFont;

    int selectionInt;

    bool hasWon;

    string resultString;
    void Start()
    {
        selectionInt = 0;

        resultString = "Oof, you died!";

        if (PlayerPrefs.GetInt("died") == 0)
        {
            resultString = "Congratz!";
        }

    }

    void OnGUI()
    {
        GUI.skin.font = customFont;


        GUIStyle style = new GUIStyle(GUI.skin.button);
        GUIContent content = new GUIContent(resultString);
        Vector2 size = style.CalcSize(content);
        GUI.Label(new Rect(Screen.width / 2f - size.x / 2f, Screen.height * 0.4f - size.y / 2f, 400, 200), resultString);


        string newGameString = "Retry";
        content = new GUIContent(newGameString);
        size = style.CalcSize(content);
        GUI.Label(new Rect(Screen.width / 2f - size.x / 2f, Screen.height * 0.5f - size.y /2f, 400, 200), newGameString);


        GUI.DrawTexture(new Rect(Screen.width * 0.38f, 5 + Screen.height * 0.5f + selectionInt * (cursorTex.height * 5f + 10) - size.y / 2f, cursorTex.width * 5f, cursorTex.height * 5f), cursorTex);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            Application.LoadLevel("Scene001");
        }
    }
}
