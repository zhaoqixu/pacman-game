using System.Collections.Generic;
using HTNplanner;
using UnityEngine;

    public class PlannerInterface : MonoBehaviour
    {
        // FIELDS


        public AIController aiController;

        private HTNPlanner planner;

        private bool doneSearching;

        private bool searchSuccessful;


        // METHODS

        void Start()
        {
            planner = new HTNPlanner(typeof(Domain), new Domain().GetMethodsDict(), typeof(Domain));
        }

        public bool GetMoreItems()
        {
            List<string> plan = GetPlan();
            if (plan != null)
                return SendPlanToAI(plan);
            Debug.Log("no plan found");
            return false;
        }

        public void CleanRoomsThreaded()
        {
            SearchPlanAndSend();
        }


        private void SearchPlanAndSend()
        {
            this.doneSearching = false;
            this.searchSuccessful = false;

            if (GetComponent<WorldModelManager>())
            {
                State initialState = GetComponent<WorldModelManager>().GetWorldStateCopy();
                if (initialState.ContainsVar("at"))
                {
                    initialState.Add("checked", initialState.GetStateOfVar("at")[0]);
                }
                List<List<string>> goalTasks = new List<List<string>>();
                goalTasks.Add(new List<string>(new string[1] { "CollectItems" }));

     
                    List<string> plan = planner.SolvePlanningProblem(initialState, goalTasks);

                        this.doneSearching = true;

                        if (plan != null)
                            this.searchSuccessful = SendPlanToAI(plan);
                        else
                            Debug.Log("no plan found");

            }
        }


    private List<string> GetPlan()
    {
        if (GetComponent<WorldModelManager>())
        {
            State initialState = GetComponent<WorldModelManager>().GetWorldStateCopy();
            if (initialState.ContainsVar("at"))
            {
                initialState.Add("checked", initialState.GetStateOfVar("at")[0]);
            }
            List<List<string>> goalTasks = new List<List<string>>();
            goalTasks.Add(new List<string>(new string[1] { "CollectItems" }));

            return planner.SolvePlanningProblem(initialState, goalTasks);
        }
        return null;
    }

    private bool SendPlanToAI(List<string> plan)
        {
            if (aiController)
            {
                aiController.SearchandExecutePlan(plan);
                return true;
            }
            return false;
        }

        public void CancelSearch()
        {
            planner.CancelSearch = true;
        }

        public bool IsDoneSearching()
        {
            return doneSearching;
        }

        public bool IsSearchSuccessful()
        {
            return searchSuccessful;
        }
    }