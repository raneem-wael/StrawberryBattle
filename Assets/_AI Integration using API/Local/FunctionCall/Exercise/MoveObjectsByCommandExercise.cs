using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class MoveObjectsByCommandExercise : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public InputField playerText;
    public RectTransform blueSquare;
    public RectTransform redSquare;
    public float moveDistance = 50f;
    void Start()
    {
        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();
    }

    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) 
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    string ConstructDirectionPrompt(string message)
    {
        // construct the prompt for the AI to understand direction commands
        return "";
    }
    string ConstructPromptColor(string message)
    {
        string prompt = "Which of the following choices matches best the input?\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<ColorFunctions>()) prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice";
        return prompt;
    }
    string ConstructPromptDirection(string message)
    {
        string prompt = "Which of the following choices matches best the input?\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<DirectionFunctions>()) prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice";
        return prompt;
    }

    async void onInputFieldSubmit(string message)
    {
        /* Example prompts and test cases for students:
         * 
         * Test inputs:
         * - "move the blue square up"
         * - "move red square to the right"
         * - "make the blue square go down"
         * - "move the red square left"
         * 
         * Expected AI responses examples:
         * - Direction: "MoveUp", "MoveRight", "MoveDown", "MoveLeft", "NoDirectionsMentioned"
         * - Color: "BlueColor", "RedColor", "NoColorMentioned"
         */

        //TODO: Student Exercise
        //     1.Disable the input field
        //     2.Get direction and color from AI using llmCharacter.Chat
        //     3.Convert AI responses to actual Vector3 and Color using reflection
        //           Color color = (Color)typeof(ColorFunctions).GetMethod(MethodName).Invoke(null, null);
        //    Vector3 direction = (Vector3)typeof(DirectionFunctions).GetMethod(MethodName).Invoke(null, null);
        //    4.Move the correct square in the specified direction
        //     5.Re - enable the input field


        playerText.interactable = false;

        string Direction = ConstructPromptDirection(message);
        string Color = ConstructPromptColor(message);
        string aiResponsedir = await llmCharacter.Chat(Direction);
        string aiResponsecolor = await llmCharacter.Chat(Color);


        Color color = (Color)typeof(ColorFunctions).GetMethod(aiResponsecolor)?.Invoke(null, null);
        Vector3 direction = (Vector3)typeof(DirectionFunctions).GetMethod(aiResponsedir)?.Invoke(null, null);

        RectTransform targetSquare = GetObjectByColor(color);
        if (targetSquare != null && direction != Vector3.zero)
        {
            targetSquare.anchoredPosition += new Vector2(direction.x, direction.y) * moveDistance;
        }

        playerText.interactable = true;
        playerText.Select();
    }

    private RectTransform GetObjectByColor(Color color)
    {
        if (color == Color.blue)
            return blueSquare;
        else if (color == Color.red)
            return redSquare;
        
        return null;
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }
} 