using Runemark.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runemark.VisualEditor
{
	public class VisualEditorBehaviour : MonoBehaviour 
	{
        public string ID
        {
            get
            {
                return transform.position.sqrMagnitude + "-" + Graph.ID;
            }
        }
		public FunctionGraph Graph;
		public bool isDisabled = false;
		public Dictionary<string, ExecutableNode> _executionFlows = new Dictionary<string, ExecutableNode>();

        #region Debug
        public bool _debugExecChange;
        #endregion


        void OnEnable()
        {          

            // If no graph is set, disable the component
            if (Graph == null) { enabled = false; isDisabled = true; return; }

            // Clone Graph for runtime
            var cloneGraph = (FunctionGraph)Graph.Copy(true);                
            cloneGraph.name += "_" + name;
            cloneGraph.RuntimeInit(this);

            Graph = cloneGraph;
        }
		void OnDisable()
        {
            if (Graph != null) { Graph.Exit(); }
            if (onInactivated != null)
                onInactivated();
        }
        void Update()
        {
            if (isDisabled) return;

            // Iterate through every active flow.
            foreach (var flow in _executionFlows)
            {
                // If the flow entered into this node, but the node didn't yet finished, update.
                if (flow.Value.IsEntered && !flow.Value.IsFinished)
                    flow.Value.Update();

                // If the current action is finished...
                if (flow.Value.IsFinished)
                {
                    // ...continue with the next node.
                    var nextNode = flow.Value.NextNode;
                                    
                    if (nextNode != null)
                        SetActiveExec(flow.Key, nextNode);

                    // if the next node doesn't exists, the flow ends.
                    else
                    {
                        if (_debugExecChange)
                            Debug.Log(flow.Key + " flow is finished with no next node from "+flow.Value);
                        _executionFlows.Remove(flow.Key);
                        OnEventFinished(flow.Key);
                    }

                    return;
                }
            }
            OnUpdate();
        }
       
        /// <summary>
        /// Set the active execution node to the given node in the specified execution flow.
        /// </summary>
        /// <param name="flowName">Flow name equals the event name that started the flow.</param>
        /// <param name="node"></param>
        void SetActiveExec(string eventName, ExecutableNode node)
        {
            if (node != null)
            {
                // If the dictionary doesn't have a key with this flowName, create one.
                if (!_executionFlows.ContainsKey(eventName))
                {
                    _executionFlows.Add(eventName, null);
                    OnEventStarted(eventName); // Trigger the on flow started event
                }

                string log = "SET ACTIVE EXEC ("+eventName+"): from ";
                // Exit the current active node in this event flow, if any.
                if (_executionFlows[eventName] != null)
                {
                    log += _executionFlows[eventName].Name;
                    _executionFlows[eventName].Exit();
                }
                else log += "none";

                _executionFlows[eventName] = node;
                _executionFlows[eventName].Enter();
                OnNodeActivated(eventName, node);
                log += " to " + _executionFlows[eventName].Name;

                if (_debugExecChange) Debug.Log(log);            
            }
            else
            {
                if (_debugExecChange) Debug.Log("EXIT " + eventName + " flow");
                OnEventFinished(eventName);
            }
        }

        /// <summary>
        /// Calls the event by it's name.
        /// </summary>
        /// <param name="eventName"></param>
        public virtual void CallEvent(string eventName)
        {
            // If the event was called before and still in progress don't do anything.
            if (_executionFlows.ContainsKey(eventName))
                return;       
            
            // Set the active executable node to the eventNode with the given name.
            // We uses event name as flow name later.
            SetActiveExec(eventName, Graph.OnEvent(eventName));
        }

        /// <summary>
        /// Returns the active executable node from the given execution flow
        /// </summary>
        /// <param name="eventName">This is the name of the event that started the flow.</param>
        /// <returns></returns>
        protected ExecutableNode GetActiveNode(string eventName)
        {
            if (_executionFlows.ContainsKey(eventName))
                return _executionFlows[eventName];
            return null;
        }

        /// <summary>
        /// Returns the event name of the given executable node, if it's the active node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected string EventNameofNode(ExecutableNode node)
        {
            foreach (KeyValuePair<string, ExecutableNode> pair in _executionFlows)
            {
                if (pair.Value == node)
                    return pair.Key;
            }
            return "";
        }

        /// <summary>
        /// Raises the OnEventFinished event.
        /// </summary>
        /// <param name="eventName">This is the name of the event that started the flow.</param>
        protected virtual void OnEventFinished(string eventName)
        {
            // Removes the flow from the active flows.
            _executionFlows.Remove(eventName);
        }


        public Variable GetLocalVariable(string variableName)
        {
            return Graph.LocalVariables.GetByName(variableName);
        }
        
        /// <summary>
        /// Gets the local variable with the given name of the loaded graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public T GetLocalVariable<T>(string variableName)
        {
            var variable = Graph.LocalVariables.GetByName(variableName);
            if (variable == null)
            {
                RunemarkDebug.Error("Root variable with name of {0} doesn't exists in this dialogue graph({1})",
                      variableName, Graph.Name);
                return default(T);
            }
            
            if (variable.CanConvertTo(typeof(T)))
                return variable.ConvertedValue<T>();

            RunemarkDebug.Error("You can't get root variable ({0}, {1}) as {2} becouse types are not matching",
                    variableName, variable.type, typeof(T));
            return default(T);
        }



        /// <summary>
        /// Sets the local variable with the given name of the loaded graph to the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public void SetLocalVariable<T>(string variableName, T value)
        {
            var variable = Graph.LocalVariables.GetByName(variableName);
            if (variable == null)
            {
                RunemarkDebug.Error("Root variable with name of {0} doesn't exists in this dialogue graph({1})",
                      variableName, Graph.Name);
                return;
            }

            if (variable.CanConvertFrom(typeof(T)))
                variable.Value = value;
            else
                RunemarkDebug.Error("You can't set root variable ({0}, {1}) to {2} ({3}) becouse types are not matching",
                    variableName, variable.type, value, value.GetType());

        }
               		

        /// <summary>
        /// Raises the OnUpdate event.
        /// </summary>
        protected virtual void OnUpdate(){}
        protected virtual void OnEventStarted(string eventName) { }
        protected virtual void OnNodeActivated(string eventName, Node node) { }


        public System.Action onApplicationQuit;
        public System.Action onInactivated;

        private void OnApplicationQuit()
        {
            if (onApplicationQuit != null)
                onApplicationQuit();
        }

        private void OnDestroy()
        {
            if (onInactivated != null)
                onInactivated();
        }

        
    }
}