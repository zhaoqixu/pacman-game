using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace HTNplanner
{

    public class Domain
    {

        private static void AddTask(List<List<string>> returnVal, params string[] values)
        {
            try
            {
                returnVal.Add(new List<string>(values));
            }
            catch (StackOverflowException)
            {
            }
        }

        // COMPOSITE TASKS

        public static List<List<string>> CollectNearestItem(State state)
        {
            List<List<string>> returnVal = new List<List<string>>();

            if (state.ContainsVar("safe"))
            {
                string item = state.GetStateOfVar("at")[0];
                if (state.CheckVar("spawn", item))
                {
                    AddTask(returnVal, "Colect", item);
                }
                else
                {
                    string collectedItem = state.GetStateOfVar("collected")[0];
                    AddTask(returnVal, "MoveTo", collectedItem);
                }
            }
            else
            {
                AddTask(returnVal, "Hide");
                if (UnityEngine.Random.Range(0, 7) == 0)
                {
                    AddTask(returnVal, "UseTrap");
                }
            }

            return returnVal;
        }

        public Dictionary<string, MethodInfo[]> GetMethodsDict()
        {
            Dictionary<string, MethodInfo[]> myDict = new Dictionary<string, MethodInfo[]>();
            MethodInfo[] moveInfos = new MethodInfo[] { this.GetType().GetMethod("CollectNearestItem") };
            myDict.Add("MoveTo", moveInfos);
            return myDict;
        }


        // PRIMITIVE TASKS
        public static State MoveTo(State state, string item)
        {
            State newState = state;
            if (state.ContainsVar("at") && !state.CheckVar("at", item))
            {
                string location = state.GetStateOfVar("at")[0];
                if (state.CheckRelation("adjecent", location, item))
                {
                    newState.Remove("at", location);
                    newState.Add("at", item);
                    newState.Add("checked", item);
                }
                else
                    return null;
            }
            return newState;
        }

        public static State Hide(State state, string opponengDist)
        {
            State newState = state;
            if (state.CheckVar("at", opponengDist))
            {
                newState.Remove("far", opponengDist);
                newState.Add("nearby", opponengDist);

                List<string> checkedList = new List<string>(newState.GetStateOfVar("checked"));
                foreach (string checkedItem in checkedList)
                {
                    if (checkedItem != opponengDist)
                        newState.Remove("checked", checkedItem);
                }
            }
            return newState;
        }

        public static State UseTrap(State state, string opponent)
        {
            State newState = state;
            if (state.ContainsVar("at") && !state.CheckVar("opponent", opponent))
            {
                string location = state.GetStateOfVar("at")[0];
                if (state.CheckRelation("near", location, opponent))
                {
                    newState.Remove("at", location);
                    newState.Add("at", opponent);
                    newState.Add("checked", opponent);
                }
                else
                    return null;
            }
            return newState;
        }

        public static State Finish(State state)
        {
            State newState = state;

            newState.Add("finished", "true");

            return newState;
        }
    }
}
