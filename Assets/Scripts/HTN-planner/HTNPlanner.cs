using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HTNplanner
{
    public class HTNPlanner
    {
        // FIELDS

        private List<string> operators = new List<string>();

        private Dictionary<string, List<string>> methods = new Dictionary<string, List<string>>();
        private Type methodsType;

        private Type operatorsType;

        private int searchDepth = 20;

        private bool cancelSearch;


        // PROPERTIES

        public bool CancelSearch
        {
            set
            {
                cancelSearch = value;
            }
        }


        // CONSTRUCTORS
        public HTNPlanner(Type methodsType, Dictionary<string, MethodInfo[]> methodsDict, Type operatorsType)
        {
            this.methodsType = methodsType;
            this.operatorsType = operatorsType;

            InitializePlanner(methodsDict);
        }


        // METHODS

        private void InitializePlanner(Dictionary<string, MethodInfo[]> methodsDict)
        {
            DeclareOperators();
            foreach (KeyValuePair<string, MethodInfo[]> method in methodsDict)
            {
                DeclareMethods(method.Key, method.Value);
            }
        }

        public List<string> DeclareOperators()
        {
            MethodInfo[] methodInfos = operatorsType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            operators = new List<string>();

            foreach (MethodInfo info in methodInfos)
            {
                if (info.ReturnType.Name.Equals("State"))
                {
                    string methodName = info.Name;
                    if (!operators.Contains(methodName))
                        operators.Add(methodName);
                }
            }

            return operators;
        }

        public List<string> DeclareMethods(string taskName, MethodInfo[] methodInfos)
        {
            List<string> methodList = new List<string>();

            foreach (MethodInfo info in methodInfos)
            {
                if (info != null && info.ReturnType.Name.Equals("List`1"))
                {
                    methodList.Add(info.Name);
                }
            }

            if (methods.ContainsKey(taskName))
                methods.Remove(taskName);
            methods.Add(taskName, methodList);

            return methods[taskName];
        }

        public List<string> SolvePlanningProblem(State state, List<List<string>> tasks, int verbose = 0)
        {

            List<string> result = SeekPlan(state, tasks, new List<string>(), 0, verbose);

            if (cancelSearch)
            {
                if (verbose > 0)
                cancelSearch = false;
            }

            return result;
        }

        public List<string> SeekPlan(State state, List<List<string>> tasks, List<string> plan, int depth, int verbose = 0)
        {
            if (cancelSearch)
                return null;

            // safety
            if (searchDepth > 0)
            {
                if (depth >= searchDepth)
                    return null;
            }

            if (verbose > 1)
            if (tasks.Count == 0)
            {
                if (verbose > 2)
                return plan;
            }
            List<string> task = tasks[0];
            if (operators.Contains(task[0]))
            {
        

                MethodInfo info = operatorsType.GetMethod(task[0]);
                object[] parameters = new object[task.Count];
                parameters[0] = new State(state);
                if (task.Count > 1)
                {
                    int x = 1;
                    List<string> paramets = task.GetRange(1, (task.Count - 1));
                    foreach (string param in paramets)
                    {
                        parameters[x] = param;
                        x++;
                    }
                }
                State newState = (State)info.Invoke(null, parameters);

                if (newState != null)
                {
                    string toAddToPlan = "(" + task[0];
                    if (task.Count > 1)
                    {
                        List<string> paramets = task.GetRange(1, (task.Count - 1));
                        foreach (string param in paramets)
                        {
                            toAddToPlan += (", " + param);
                        }
                    }
                    toAddToPlan += ")";
                    plan.Add(toAddToPlan);
                    List<string> solution = SeekPlan(newState, tasks.GetRange(1, (tasks.Count - 1)), plan, (depth + 1), verbose);
                    if (solution != null)
                        return solution;
                }
            }
            if (methods.ContainsKey(task[0]))
            {

                List<string> relevant = methods[task[0]];
                foreach (string method in relevant)
                {
                    // Decompose non-primitive task into subtasks by use of a HTN method
                    MethodInfo info = methodsType.GetMethod(method);
                    object[] parameters = new object[task.Count];
                    parameters[0] = new State(state);
                    if (task.Count > 1)
                    {
                        int x = 1;
                        List<string> paramets = task.GetRange(1, (task.Count - 1));
                        foreach (string param in paramets)
                        {
                            parameters[x] = param;
                            x++;
                        }
                    }
                    List<List<string>> subtasks = null;
                    try
                    {
                        subtasks = (List<List<string>>)info.Invoke(null, parameters);
                    }
                    catch (Exception e)
                    {

                    }


                    if (subtasks != null)
                    {
                        List<List<string>> newTasks = new List<List<string>>(subtasks);
                        newTasks.AddRange(tasks.GetRange(1, (tasks.Count - 1)));
                        try
                        {
                            List<string> solution = SeekPlan(state, newTasks, plan, (depth + 1), verbose);
                            if (solution != null)
                                return solution;
                        }
                        catch (StackOverflowException e)
                        {

                        }
                    }
                }
            }

            return null;
        }
    }
}