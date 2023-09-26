using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Atlas.Orbit.ComponentProcessors {
    using Parser;
    using TypeSetters;
    using UnityEngine;

    public class EventTriggerProcessor : ComponentProcessor<EventTrigger> {
        public override Dictionary<string, TypeSetter<EventTrigger>> Setters => new() {
            { "HoveredValue", new StringSetter<EventTrigger>(RegisterHovered) },
            { "NormalizedMouseValue", new StringSetter<EventTrigger>(RegisterNormalizedMouse) },
            { "HoveredEvent", new StringSetter<EventTrigger>(RegisterHoveredEvent) },
            { "UnHoveredEvent", new StringSetter<EventTrigger>(RegisterUnHoveredEvent) },
            { "PointerDownEvent", new StringSetter<EventTrigger>(RegisterPointerDownEvent) },
            { "PointerUpEvent", new StringSetter<EventTrigger>(RegisterPointerUpEvent) },
            { "PointerDragEvent", new StringSetter<EventTrigger>(RegisterPointerDragEvent) }
        };
        
        
        private void RegisterPointerDragEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.BeginDrag);
        
        private void RegisterPointerDownEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerDown);

        private void RegisterPointerUpEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerUp);
        
        private void RegisterHoveredEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerEnter);

        private void RegisterUnHoveredEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerExit);

        private void RegisterEvent(EventTrigger eventTrigger, string events, EventTriggerType triggerType) {
            UIRenderData renderData = CurrentData;

            EventTrigger.Entry entry = new();
            entry.eventID = triggerType;
            entry.callback.AddListener((data) => {
                renderData.EmitEvent(events);
            });
            eventTrigger.triggers.Add(entry);
        }

        private void RegisterHovered(EventTrigger eventTrigger, string valueID) {
            UIRenderData renderData = CurrentData;

            renderData.SetValue(valueID, false);

            EventTrigger.Entry pointerEnter = new();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => {
                renderData.SetValue(valueID, true);
            });
            eventTrigger.triggers.Add(pointerEnter);
            EventTrigger.Entry pointerExit = new();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => {
                renderData.SetValue(valueID, false);
            });
            eventTrigger.triggers.Add(pointerExit);
        }

        private void RegisterNormalizedMouse(EventTrigger eventTrigger, string valueID) {
            UIRenderData renderData = CurrentData; 
            
            renderData.SetValue(valueID, Vector2.zero);
            
            RectTransform rectTransform = eventTrigger.GetComponent<RectTransform>();
            EventTrigger.Entry move = new();
            move.eventID = EventTriggerType.Move;
            move.callback.AddListener((data) => {
                PointerEventData eventData = data as PointerEventData;
                if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out var localPosition)) return;
                Vector2 normalizedPosition = Rect.PointToNormalized(rectTransform.rect, localPosition);
                renderData.SetValue(valueID, normalizedPosition);
            });
            eventTrigger.triggers.Add(move);
        }
    }
}