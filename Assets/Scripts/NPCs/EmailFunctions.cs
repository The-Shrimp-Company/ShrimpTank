using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

static public class EmailFunctions
{
    public enum FunctionIndexes
    {
        AddMoney,
        AddReputation,
        CompleteRequest,
        SetCompletion,
        SpawnShrimp,
        GiveAnyShrimp,
        SetFlag,
        SetTutorialFlag,
        Count
    }

    static public void ButtonFunc(this MyButton button)
    {
        button.actions = new List<UnityAction>();
        foreach(FunctionIndexes funcID in button.funcID)
        {
            switch (funcID)
            {
                case FunctionIndexes.AddMoney:
                    AddMoney(button);
                    break;
                case FunctionIndexes.AddReputation:
                    AddReputation(button);
                    break;
                case FunctionIndexes.CompleteRequest:
                    CompleteRequest(button);
                    break;
                case FunctionIndexes.SetCompletion:
                    SetCompletion(button);
                    break;
                case FunctionIndexes.SpawnShrimp:
                    SpawnShrimp(button);
                    break;
                case FunctionIndexes.GiveAnyShrimp:
                    GiveAnyShrimp(button);
                    break;
                case FunctionIndexes.SetFlag:
                    SetFlag(button);
                    break;
                case FunctionIndexes.SetTutorialFlag:
                    SetTutorialFlag(button);
                    break;
                default:
                    break;
            }
        }
    }

    
    static private void AddMoney(MyButton button)
    {
        button.actions.Add(() => 
        {
            Money.instance.AddMoney(button.data[(int)FunctionIndexes.AddMoney].data[0].TryCast<float>());
        });
    }

    static private void AddReputation(MyButton button)
    {
        button.actions.Add(() => 
        {
            Reputation.AddReputation(button.data[(int)FunctionIndexes.AddReputation].data[0].TryCast<float>());
        });
    }

    static private void CompleteRequest(MyButton button) 
    {
        button.actions.Add(() =>
        {
            button.data[(int)FunctionIndexes.CompleteRequest].data[0].TryCast<Request>().OpenShrimpSelection();
        });
    }

    static private void SetCompletion(MyButton button)
    {
        button.actions.Add(() => 
        {
            List<object> objs = button.data[(int)FunctionIndexes.SetCompletion].data;
            NPCManager.Instance.NPCs.Find((x) => { return x.name == objs[0].TryCast<string>(); })?.SetCompletion(objs[1].TryCast<int>());
        });
    }

    static private void SpawnShrimp(MyButton button)
    {
        button.actions.Add(() =>
        {
            ShrimpManager.instance.destTank.SpawnShrimp(button.data[(int)FunctionIndexes.SpawnShrimp].data[0].TryCast<ShrimpStats>());
        });
    }

    static private void GiveAnyShrimp(MyButton button)
    {
        button.actions.Add(() =>
        {
            CustomerManager.Instance.emailScreen.OpenFullSelection(button.data[(int)FunctionIndexes.GiveAnyShrimp].data[0].TryCast<float>(), EmailManager.instance.emails
                .Find((x) => { return x.ID == button.emailID; }));
        });
    }

    static private void SetFlag(MyButton button)
    {
        button.actions.Add(() =>
        {
            // for each object in the functions data pool past the first (we drop the first object with the GetRange function, because it should be the
            // name of the NPC calling the function), we then find the NPC in question, and add the flag to them. For the most part, this will only
            // add one flag at a time, but this accounts for someone setting multiple flags in a single function call.
            foreach (object obj in button.data[(int)FunctionIndexes.SetFlag].data.GetRange(1, button.data[(int)FunctionIndexes.SetFlag].data.Count - 1))
                NPCManager.Instance.NPCs.Find((x) => { return x.name == button.data[(int)FunctionIndexes.SetFlag].data[0].TryCast<string>(); }).SetFlag(obj.TryCast<string>());
        });
    }

    static private void SetTutorialFlag(MyButton button)
    {
        button.actions.Add(() =>
        {
            foreach (object obj in button.data[(int)FunctionIndexes.SetTutorialFlag].data) Tutorial.instance.flags.Add(obj.TryCast<string>());
        });
    }
}
