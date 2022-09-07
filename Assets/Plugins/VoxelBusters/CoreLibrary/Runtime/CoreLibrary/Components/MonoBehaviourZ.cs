using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    public class MonoBehaviourZ : MonoBehaviour
    {
        #region Fields

        private     bool    m_isInitialized = false;

        #endregion

        #region Unity methods

        private void Awake()
        {
            EnsureInitialized();
        }

        protected virtual void Start()
        { }

        #endregion

        #region Private methods

        protected void EnsureInitialized()
        {
            if (m_isInitialized) return;

            m_isInitialized = true;
            Init();
        }

        protected virtual void Init()
        { }

        #endregion
    }
}