// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Nody
{
    /// <summary>
    /// The Flow Controller is responsible of managing a Flow Graph.
    /// It can control the graph either as a local graph (instance) or a global graph.
    /// </summary>
    [AddComponentMenu("Doozy/UI/Nody/Flow Controller")]
    public class FlowController : MonoBehaviour, IUseMultiplayerInfo
    {
        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;

        /// <summary> True if Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        [SerializeField] private bool DontDestroyOnSceneChange;
        [SerializeField] private FlowGraph Flow;
        [SerializeField] private FlowType FlowType = FlowType.Global;

        /// <summary> Don't destroy on load (when the scene changes) the GameObject this component is attached to </summary>
        public bool dontDestroyOnSceneChange
        {
            get => DontDestroyOnSceneChange;
            set => DontDestroyOnSceneChange = value;
        }

        /// <summary> Flow graph managed by this controller </summary>
        public FlowGraph flow => Flow;

        /// <summary> Type of flow </summary>
        public FlowType flowType => FlowType;

        #region Player Index

        [SerializeField] private MultiplayerInfo MultiplayerInfo;
        public MultiplayerInfo multiplayerInfo => MultiplayerInfo;
        public bool hasMultiplayerInfo => multiplayerInfo != null;
        public int playerIndex => multiplayerMode & hasMultiplayerInfo ? multiplayerInfo.playerIndex : inputSettings.defaultPlayerIndex;
        public void SetMultiplayerInfo(MultiplayerInfo info) => MultiplayerInfo = info;

        #endregion

        private bool initialized { get; set; }

        private void Awake()
        {
            if (DontDestroyOnSceneChange)
                DontDestroyOnLoad(gameObject);

            BackButton.Initialize();

            initialized = false;
        }

        private IEnumerator Start()
        {
            yield return null;
            SetFlowGraph(Flow);
        }

        private void OnEnable()
        {
            if (initialized)
                SetFlowGraph(Flow);
        }

        private void OnDisable()
        {
            if (Flow != null)
                Flow.Stop();
        }

        private void Update()
        {
            if (initialized)
                Flow.Update();
        }

        private void FixedUpdate()
        {
            if (initialized)
                Flow.FixedUpdate();
        }

        private void LateUpdate()
        {
            if (initialized)
                Flow.LateUpdate();
        }

        /// <summary> Starts the graph after 1 frame </summary>
        private IEnumerator StartGraph()
        {
            yield return null;

            if (Flow == null)
                yield break;

            initialized = true;
            Flow.controller = this;
            Flow.Start();
        }

        /// <summary> Set a new flow graph to this controller </summary>
        /// <param name="graph"> Target flow graph </param>
        public void SetFlowGraph(FlowGraph graph)
        {
            if (initialized & Flow != null)
            {
                Flow.Stop();
                Flow.controller = null;
                Flow = null;
            }

            enabled = graph != null;

            if (graph == null)
                return;

            Flow = flowType == FlowType.Local ? graph.Clone() : graph;
            StartCoroutine(StartGraph());
        }

        /// <summary> Activate the given node (if it exists in the flow graph) </summary>
        /// <param name="node"> Note to search for in the loaded flow graph </param>
        /// <param name="fromPort"> Pass a port as the source port </param>
        public void SetActiveNode(FlowNode node, FlowPort fromPort = null)
        {
            if (!initialized) return;
            if (Flow == null) return;
            if (node == null) return;
            if (!Flow.ContainsNode(node)) return;
            Flow.SetActiveNode(node, fromPort);
        }

        /// <summary> Activate the node with the given id (if it exists in the flow graph) </summary>
        /// <param name="nodeId"> Node id to search for in the loaded flow graph </param>
        public void SetActiveNodeById(string nodeId)
        {
            if (!initialized) return;
            if (Flow == null) return;
            if (nodeId.IsNullOrEmpty()) return;
            if (!Flow.ContainsNodeById(nodeId)) return;
            Flow.SetActiveNodeByNodeId(nodeId);
        }

        /// <summary> Activate the first node with the given node name (if it exists in the flow graph) </summary>
        /// <param name="nodeName"> Node name to search for in the loaded flow graph </param>
        public void SetActiveNodeByName(string nodeName)
        {
            if (!initialized) return;
            if (Flow == null) return;
            if (nodeName.IsNullOrEmpty()) return;
            if (!Flow.ContainsNodeByName(nodeName)) return;
            Flow.SetActiveNodeByNodeName(nodeName);
        }
    }
}
