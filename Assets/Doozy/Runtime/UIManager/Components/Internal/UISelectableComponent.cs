// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Components.Internal
{
    public abstract class UISelectableComponent<T> : UISelectable where T : UISelectable
    {
        public static HashSet<T> database { get; private set; } = new HashSet<T>();

        [ExecuteOnReload]
        private static void OnReload()
        {
            database = new HashSet<T>();
        }

        /// <summary> Component reference </summary>
        public T component { get; private set; }

        [SerializeField] private UIBehaviours Behaviours;
        /// <summary> Manages UIBehaviour components </summary>
        public UIBehaviours behaviours => Behaviours;

        /// <summary> TRUE is this selectable is selected by EventSystem.current, FALSE otherwise </summary>
        public bool isSelected => EventSystem.current.currentSelectedGameObject == gameObject;

        protected UISelectableComponent()
        {
            Behaviours =
                new UIBehaviours()
                    .SetSelectable(this);
        }

        protected override void Awake()
        {
            database.Add(component = GetComponent<T>());
            base.Awake();
            Behaviours
                .SetSelectable(component)
                .SetSignalSource(gameObject);
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            CleanDatabase();
            base.OnEnable();
            StartCoroutine(ConnectBehaviours());
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying)
                return;
            
            CleanDatabase();
            base.OnDisable();
            behaviours.Disconnect();
        }

        protected override void OnDestroy()
        {
            database.Remove(component);
            CleanDatabase();
            base.OnDestroy();
        }

        /// <summary> Remove all null references from the database </summary>
        protected static void CleanDatabase() =>
            database.Remove(null);

        private IEnumerator ConnectBehaviours()
        {
            yield return null;
            behaviours?
                .SetSelectable(component)
                .SetSignalSource(gameObject)
                .Connect();
        }

        /// <summary>
        /// Add the given behaviour and get a reference to it (automatically connects)
        /// If the behaviour already exists, the reference to it will get automatically returned. 
        /// </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public UIBehaviour AddBehaviour(UIBehaviour.Name behaviourName) =>
            behaviours.AddBehaviour(behaviourName);

        /// <summary> Remove the given behaviour (automatically disconnects) </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public void RemoveBehaviour(UIBehaviour.Name behaviourName) =>
            behaviours.RemoveBehaviour(behaviourName);

        /// <summary> Check if the given behaviour has been added (exists) </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public bool HasBehaviour(UIBehaviour.Name behaviourName) =>
            behaviours.HasBehaviour(behaviourName);

        /// <summary>
        /// Get the behaviour with the given name.
        /// Returns null if the behaviour has not been added (does not exist)
        /// </summary>
        /// <param name="behaviourName"> UIBehaviour.Name </param>
        public UIBehaviour GetBehaviour(UIBehaviour.Name behaviourName) =>
            behaviours.GetBehaviour(behaviourName);
    }
}
