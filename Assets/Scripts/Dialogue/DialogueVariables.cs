using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;
    private const string saveVariablesKey = "INK_VARIABLES";

    public DialogueVariables(TextAsset loadGlobalsJSON) 
    {
        // create the story
        globalVariablesStory = new Story(loadGlobalsJSON.text);
        
        // try to load data
        SaveObject loadedSave = SaveHelper.currentSaveObject();

        if (loadedSave != null) {
            string jsonState = loadedSave.inkVariablesJson;
            globalVariablesStory.state.LoadJson(jsonState);
        }
        

        // initialize the dictionary
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log("Initialized global dialogue variable: " + name + " = " + value);
        }
    }

    public SaveObject SaveVariablesIntoObject(SaveObject so) 
    {
        if (globalVariablesStory != null && so != null) 
        {
            // Load the current state of all of our variables to the globals story
            VariablesToStory(globalVariablesStory);
            
            // Serialize ink json state to the current save file
            so.inkVariablesJson = globalVariablesStory.state.ToJson();
            return so;
        } else {
            Debug.Log("SaveObject null");
        }
        return null;
    }

    public Ink.Runtime.Object GetFromInkGlobals(string search) {
        return globalVariablesStory.variablesState.GetVariableWithName(search);
    }

    public void StartListening(Story story) 
    {
        // it's important that VariablesToStory is before assigning the listener!
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }

    public void StopListening(Story story) 
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value) 
    {
        // only maintain variables that were initialized from the globals ink file
        if (variables.ContainsKey(name)) 
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story) 
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables) 
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

}
