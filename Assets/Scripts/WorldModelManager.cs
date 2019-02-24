using System.Collections.Generic;
using HTNplanner;
using UnityEngine;

    public class WorldModelManager : MonoBehaviour
    {
        // FIELDS

        private State worldState;


        void Awake()
        {
            worldState = new State("start");
        }

        public State GetWorldStateCopy()
        {
            if (worldState != null)
            {
                return new State(worldState);
            }
            return null;
        }

        public void UpdateKnowledge(string variable, string value, bool truthValue)
        {
            if (truthValue)
            {
                worldState.Add(variable, value);
            }
            else
            {
                worldState.Remove(variable, value);
            }
        }

        public void UpdateKnowledge(string relation, string firstElement, string secondElement, bool truthValue)
        {
            if (truthValue)
            {
                worldState.Add(relation, firstElement, secondElement);
            }
            else
            {
                worldState.Remove(relation, firstElement, secondElement);
            }
        }
    }