// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor
{
    [AddComponentMenu("Doozy/Reactor/Progressor")]
    public class Progressor : MonoBehaviour
    {
        [SerializeField] private List<ProgressTarget> ProgressTargets;
        public List<ProgressTarget> progressTargets
        {
            get
            {
                Initialize();
                return ProgressTargets;
            }
        }

        [SerializeField] protected float FromValue = 0f;
        /// <summary> Start Value </summary>
        public float fromValue
        {
            get => FromValue;
            set
            {
                FromValue = value;
                if (reaction.isActive) reaction.SetFrom(fromValue);
            }
        }

        [SerializeField] protected float ToValue = 1f;
        /// <summary> End Value </summary>
        public float toValue
        {
            get => ToValue;
            set
            {
                ToValue = value;
                if (reaction.isActive) reaction.SetTo(toValue);
            }
        }

        [SerializeField] protected float CurrentValue;
        /// <summary> Current Value </summary>
        public float currentValue => CurrentValue;

        [SerializeField] protected float Progress;
        /// <summary> Current Progress </summary>
        public float progress => Progress;

        [SerializeField] protected float EasedProgress;
        /// <summary> Current Eased Progress </summary>
        public float easedProgress => EasedProgress;

        [SerializeField] protected float CustomResetValue;
        /// <summary> Custom Reset Value </summary>
        public float customResetValue
        {
            get => CustomResetValue;
            set => CustomResetValue = Mathf.Clamp(value, FromValue, ToValue);
        }

        [SerializeField] protected FloatReaction Reaction;
        /// <summary> Reaction that runs this progressor </summary>
        public FloatReaction reaction
        {
            get
            {
                Initialize();
                return Reaction;
            }
        }

        public ResetValue ResetValueOnEnable = ResetValue.FromValue;

        public FloatEvent OnValueChanged;
        public FloatEvent OnProgressChanged;
        public FloatEvent OnEasedProgressChanged;

        private bool initialized { get; set; }

        protected virtual void Initialize()
        {
            if (initialized) return;
            ProgressTargets ??= new List<ProgressTarget>();
            Reaction = Reaction ?? ReactionPool.Get<FloatReaction>();
            Reaction.SetFrom(fromValue);
            Reaction.SetTo(toValue);
            Reaction.SetValue(fromValue);
            Reaction.OnUpdateCallback = UpdateProgressor;
            initialized = true;
        }

        protected virtual void Awake()
        {
            if(!Application.isPlaying) return;
            Initialize();
        }

        protected virtual void OnEnable()
        {
            if(!Application.isPlaying) return;
            ValidateTargets();
            ResetCurrentValue(ResetValueOnEnable);
        }

        protected virtual void OnDisable()
        {
            if(!Application.isPlaying) return;
            ValidateTargets();
            reaction.Stop();
        }

        protected void OnDestroy()
        {
            if(!Application.isPlaying) return;
            Reaction?.Recycle();
        }

        /// <summary> Remove null and duplicate targets </summary>
        private void ValidateTargets() =>
            ProgressTargets = progressTargets.Where(t => t != null).Distinct().ToList();

        protected void ResetCurrentValue(ResetValue resetValue)
        {
            if (resetValue == ResetValue.Disabled) return;
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            switch (resetValue)
            {
                case ResetValue.FromValue:
                    SetProgressAtZero();
                    break;
                case ResetValue.EndValue:
                    SetProgressAtOne();
                    break;
                case ResetValue.CustomValue:
                    SetProgressAt(reaction.GetProgressAtValue(CustomResetValue));
                    break;
                case ResetValue.Disabled:
                default:
                    throw new ArgumentOutOfRangeException(nameof(resetValue), resetValue, null);
            }
        }


        /// <summary> Update current value and trigger callbacks </summary>
        public virtual void UpdateProgressor()
        {
            Progress = reaction.progress;
            EasedProgress = reaction.easedProgress;
            CurrentValue = reaction.currentValue;

            OnValueChanged?.Invoke(CurrentValue);
            OnProgressChanged?.Invoke(Progress);
            OnEasedProgressChanged?.Invoke(EasedProgress);

            bool foundNullTarget = false;
            foreach (ProgressTarget target in ProgressTargets)
            {
                if (target == null)
                {
                    foundNullTarget = true;
                    continue;
                }
                target.UpdateTarget(this);
            }
            if (foundNullTarget)
                ProgressTargets = ProgressTargets.Where(t => t != null).ToList();
        }

        /// <summary> Set the progressor's value at the given target value </summary>
        /// <param name="value"> Target value </param>
        public void SetValueAt(float value)
        {
            SetProgressAt(reaction.GetProgressAtValue(Mathf.Clamp(value, fromValue, toValue)));
        }

        /// <summary> Set the progressor's progress at the given target progress </summary>
        /// <param name="targetProgress"> Target progress </param>
        public void SetProgressAt(float targetProgress)
        {
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            reaction.SetProgressAt(targetProgress);
            UpdateProgressor();
        }

        /// <summary> Set the progressor's progress at 1 </summary>
        public void SetProgressAtOne() =>
            SetProgressAt(1f);

        /// <summary> Set the progressor's progress at 0 </summary>
        public void SetProgressAtZero() =>
            SetProgressAt(0f);

        /// <summary> Play from the given start from value to the end to value </summary>
        /// <param name="direction"> Play direction </param>
        public void Play(PlayDirection direction) =>
            Play(direction == PlayDirection.Reverse);

        /// <summary> Play from the given start from value to the end to value </summary>
        /// <param name="inReverse"> Play in reverse? </param>
        public void Play(bool inReverse = false) =>
            reaction.Play(FromValue, ToValue, inReverse);

        /// <summary> Play from the current value to the given valur </summary>
        /// <param name="value"> To value </param>
        public void PlayToValue(float value) =>
            PlayToProgress(reaction.GetProgressAtValue(Mathf.Clamp(value, fromValue, toValue)));

        /// <summary> Play from the current progress to the given end progress (to) </summary>
        /// <param name="toProgress"> To (end) progress </param>
        public void PlayToProgress(float toProgress)
        {
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            reaction.PlayToProgress(toProgress);
        }

        public void Stop() =>
            reaction.Stop();

        public void Reverse() =>
            reaction.Reverse();

        public void Rewind() =>
            reaction.Rewind();
    }
}
