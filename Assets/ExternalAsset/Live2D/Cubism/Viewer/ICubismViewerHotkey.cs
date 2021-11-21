﻿/*
 * Copyright(c) Live2D Inc. All rights reserved.
 * 
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at http://live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */
 
 
namespace Live2D.Cubism.Viewer
{
    /// <summary>
    /// Common hotkey interface.
    /// </summary>
    public interface ICubismViewerHotkey
    {
        /// <summary>
        /// Evaluates hotkey.
        /// </summary>
        /// <returns><see langword="true"/> if hotkey pressed; <see langword="false"/> otherwise.</returns>
        bool Evaluate();

        /// <summary>
        /// Evaluates if hotkey just pressed.
        /// </summary>
        /// <returns><see langword="true"/> if hotkey pressed; <see langword="false"/> otherwise.</returns>
        bool EvaluateJust();
    }
}
