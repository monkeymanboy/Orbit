﻿using Atlas.Orbit.Parser;
using System;
using System.Collections;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.Components
{
    public class ListItem : MonoBehaviour
    {
        public event Action<int> OnFocusCell;

        private UIRenderData renderData;

        public int LoopedIndex { get; private set; }
        public int Index { get; private set; }

        private UIValue expansionValue;
        private UIValue focusedValue;

        public void Parse(UIParser parser, XmlNode rootNode, object host, UIRenderData parentData)
        {
            renderData = parser.Parse(rootNode, gameObject, host, parentData, (renderData) =>
            {
                renderData.SetValue("ExpansionFactor", 0f);
                expansionValue = renderData.GetValueFromID("ExpansionFactor");
                renderData.SetValue("Focused", false);
                focusedValue = renderData.GetValueFromID("Focused");
            });
            renderData.AddEvent("FocusCell", OnReceiveFocusCellEvent);
            renderData.AddEvent("ScrollUp", OnReceiveScrollUpEvent);
            renderData.AddEvent("ScrollDown", OnReceiveScrollDownEvent);
        }

        public void SetExpansionFactor(float expansionFactor)
        {
            expansionValue.SetValue(expansionFactor);
        }

        public void SetFocused(bool focused)
        {
            focusedValue.SetValue(focused);
        }

        private void OnReceiveFocusCellEvent()
        {
            OnFocusCell?.Invoke(Index);
        }

        private void OnReceiveScrollUpEvent()
        {
            OnFocusCell?.Invoke(Index + 1);
        }

        private void OnReceiveScrollDownEvent()
        {
            OnFocusCell?.Invoke(Index - 1);
        }

        public void SetHost(int index, int loopedIndex, object host)
        {
            if (renderData == null)
                throw new Exception("Cannot set host of ListItem that has not been parsed");
            Index = index;
            LoopedIndex = loopedIndex;
            renderData.Host = host;
        }

        public void ScaleTo(float scale)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleToSizeCoroutine(scale));
        }

        private IEnumerator ScaleToSizeCoroutine(float scale)
        {
            float
                startScale =
                    transform.GetChild(0).localScale
                        .x; //TODO(David): Relying on the first child containing everything here, should wrap the object around a scaler
            float t = 0;
            while (t < 1)
            {
                t = Mathf.Clamp(t, 0, 1);
                float scaleThisFrame = t;//UtilEasingFunction.EaseOutElastic(startScale, scale, t);
                transform.GetChild(0).localScale = new Vector3(scaleThisFrame, scaleThisFrame, scaleThisFrame);
                t += Time.deltaTime * 4;
                yield return null;
            }
        }
    }
}