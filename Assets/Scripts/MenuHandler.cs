using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuHandler : MonoBehaviour 
{

    public Texture menuText;

    public static bool IsEnabled;
    public static bool TemporaryEndDisabled;

    //Renderer renderer;
    public Font customFont;

    bool isCentered;

    public Texture endKey;

    int currentStringIndex;
    List<string> currentStringQueue;
    string currentTotalString;
    string currentWriteString;

    float stringTypeTimer;
    float typeSpeed = 0.01f;
    int characterTickAmount = 1;

    bool showEndSymbol;
    bool drawEndSymbol;
    float flashTimer;
    float showDuration = 0.5f;

    float temporaryDisableTimer;
    float temporaryDisableDuration = 0.5f;

	void Start () 
    {
        //renderer = GetComponent<Renderer>();
        
	}
	
    void OnGUI()
    {
        if (IsEnabled)
        {
            Vector2 menuSize = new Vector2(menuText.width / 960f * Screen.width, menuText.height / 600f * Screen.height);

            GUI.DrawTexture(new Rect(0, Screen.height - menuText.height / 600f * Screen.height, menuText.width / 960f * Screen.width, menuText.height / 600f * Screen.height), menuText);

            GUI.skin.font = customFont;
            Vector2 menuPos = Vector2.zero;
            menuPos.y = Screen.height - menuText.height / 600f * Screen.height + 15;
            //menuPos.y -= 48;
            //menuPos.x -= Screen.width / 2f;
            menuPos.x += 14;


            Vector2 totalExtraSize = Vector2.zero;
            if (isCentered)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                GUIContent content = new GUIContent(currentWriteString);
                Vector2 size = style.CalcSize(content);
                totalExtraSize.x = Screen.width/2f - size.x / 2f;
                totalExtraSize.y = 30;
            }


            GUI.Label(new Rect(menuPos.x + totalExtraSize.x, menuPos.y + totalExtraSize.y, menuSize.x-50, menuSize.y-50), currentWriteString);

            if (showEndSymbol && drawEndSymbol)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.935f, Screen.height * 0.935f, endKey.width * 3, endKey.height * 3), endKey);
            }
        }
       
    }

    public void SetString(List<string> stringQueue, bool _isCentered = false)
    {
        isCentered = _isCentered;
        //renderer.enabled = true;
        IsEnabled = true;
        currentStringIndex = 0;
        currentStringQueue = stringQueue;
        currentTotalString = currentStringQueue[currentStringIndex];
        currentWriteString = "";
        //
        drawEndSymbol = true;
    }

    void NextString()
    {
        currentStringIndex++;
        currentTotalString = currentStringQueue[currentStringIndex];
        currentWriteString = "";
        drawEndSymbol = true;
        showEndSymbol = false;
    }

    void DisableMenu()
    {
        drawEndSymbol = true;
        showEndSymbol = false;
        //renderer.enabled = false;
        IsEnabled = false;
        TemporaryEndDisabled = true;
        currentStringQueue = null;
    }

	void Update ()
    {
        if (currentStringQueue != null)
        {
            if (currentTotalString.Length > 0)
            {
                stringTypeTimer += Time.deltaTime;
                if (stringTypeTimer > typeSpeed)
                {
                    stringTypeTimer = 0;

                    for (int i = 0; i < characterTickAmount; i++)
                    {
                        char character = currentTotalString[0];
                        currentWriteString += character;
                        currentTotalString = currentTotalString.Remove(0, 1);
                    }

                }
                
            }
            else
            {
                showEndSymbol = true;
            }
            
        }

        if (showEndSymbol)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer > showDuration)
            {
                flashTimer = 0;
                drawEndSymbol = !drawEndSymbol;
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            {
                if (currentStringQueue.Count-1 > currentStringIndex)
                {
                    NextString();
                }
                else
                {
                    DisableMenu();
                }
            }
        }

        if (TemporaryEndDisabled)
        {
            temporaryDisableTimer += Time.deltaTime;
            if (temporaryDisableTimer > temporaryDisableDuration)
            {
                temporaryDisableTimer = 0;
                TemporaryEndDisabled = false;
            }
        }

	}
}
