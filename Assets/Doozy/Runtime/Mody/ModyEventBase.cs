// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.Mody
{
    /// <summary> Base class for ModyEvents designed to trigger one or more <see cref="ModyActionRunner"/>s </summary>
    [Serializable]
    public abstract class ModyEventBase
    {
        /// <summary> Default event name </summary>
        public const string k_DefaultEventName = "Unnamed";

        /// <summary> Enabled state for the event. If FALSE it will not Execute </summary>
        public bool Enabled;

        /// <summary> Name of the event </summary>
        public string EventName;

        /// <summary> List of action runners that trigger set actions on referenced modules </summary>
        public List<ModyActionRunner> Runners;

        /// <summary> Returns TRUE if the Runners count is greater than zero </summary>
        public bool hasRunners => Runners.Count > 0;

        protected ModyEventBase() : this(k_DefaultEventName) {}

        protected ModyEventBase(string eventName)
        {
            Enabled = false;
            EventName = eventName;
            Runners = new List<ModyActionRunner>();
        }

        /// <summary> Execute the event. Note that if the mody event is not enabled, this method does nothing </summary>
        public virtual void Execute(Signal signal = null)
        {
            if (!Enabled) return;
            foreach (ModyActionRunner runner in Runners)
                runner?.Execute();
        }

        /// <summary> Run the action with the given action name on the target <see cref="ModyModule"/> </summary>
        /// <param name="module"> Target ModyModule </param>
        /// <param name="actionName"> Name of the action </param>
        /// <returns> True if the operation was successful and false otherwise </returns>
        public bool RunsAction(ModyModule module, string actionName) =>
            Runners.Where(runner => runner.Module == module).Any(runner => runner.ActionName.Equals(actionName));

        /// <summary> Runs the actions on the given target <see cref="ModyModule"/> </summary>
        /// <param name="module"> Target ModyModule </param>
        /// <returns> True if the operation was successful and false otherwise </returns>
        public bool RunsModule(ModyModule module) =>
            Runners.Any(runner => runner.Module == module);
    }

    /// <summary> Extension methods for <see cref="ModyEventBase"/> </summary>
    public static class ModyEventBaseExtensions
    {
        
        /// <summary> Set enabled state for the target <see cref="ModyEventBase"/> </summary>
        /// <param name="target"> Target ModyEventBase </param>
        /// <param name="enabled"> Enabled state </param>
        /// <typeparam name="T"> Type of ModyEventBase </typeparam>
        public static T SetEnabled<T>(this T target, bool enabled) where T : ModyEventBase
        {
            target.Enabled = enabled;
            return target;
        }

        /// <summary> Set the event name for the target <see cref="ModyEventBase"/> </summary>
        /// <param name="target"> Target ModyEventBase </param>
        /// <param name="eventName"> Event name </param>
        /// <typeparam name="T"> Type of ModyEventBase </typeparam>
        public static T SetEventName<T>(this T target, string eventName) where T : ModyEventBase
        {
            target.EventName = eventName;
            return target;
        }
    }
}
