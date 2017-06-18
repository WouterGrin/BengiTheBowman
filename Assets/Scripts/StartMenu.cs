using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour 
{

    public Texture cursorTex;
    public Font customFont;

    int selectionInt;
	void Start ()
    {
        selectionInt = 0;
	}

    void OnGUI()
    {
        GUI.skin.font = customFont;

        string newGameString = "New Game";
        GUIStyle style = new GUIStyle(GUI.skin.button);
        GUIContent content = new GUIContent(newGameString);
        Vector2 size = style.CalcSize(content);
        GUI.Label(new Rect(Screen.width / 2f - size.x / 2f, Screen.height * 0.4f, 400, 200), newGameString);


        string exitString = "Exit Game";
        style = new GUIStyle(GUI.skin.button);
        content = new GUIContent(newGameString);
        size = style.CalcSize(content);
        GUI.Label(new Rect(Screen.width / 2f - size.x / 2f, Screen.height * 0.4f + 50, 400, 200), exitString);

        GUI.DrawTexture(new Rect(Screen.width * 0.35f, 5 + Screen.height * 0.4f + selectionInt * (cursorTex.height * 5f + 10), cursorTex.width * 5f, cursorTex.height * 5f), cursorTex);

    }
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectionInt--;
            if (selectionInt < 0)
            {
                selectionInt = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectionInt++;
            if (selectionInt > 1)
            {
                selectionInt = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            if (selectionInt == 0)
            {
                Application.LoadLevel("Scene001");
            }
            else if (selectionInt == 1)
            {
                Application.Quit();
            }

        }
	}
}
