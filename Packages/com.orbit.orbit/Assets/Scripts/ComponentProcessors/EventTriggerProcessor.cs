using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Orbit.ComponentProcessors {
    using Parser;
    using TypeSetters;

    public class EventTriggerProcessor : ComponentProcessor<EventTrigger> {
        public override Dictionary<string, TypeSetter<EventTrigger>> Setters => new() {
            { "PointerEnterEvent", new StringSetter<EventTrigger>(RegisterPointerEnterEvent) },
            { "PointerExitEvent", new StringSetter<EventTrigger>(RegisterPointerExitEvent) },
            { "PointerDownEvent", new StringSetter<EventTrigger>(RegisterPointerDownEvent) },
            { "PointerUpEvent", new StringSetter<EventTrigger>(RegisterPointerUpEvent) },
            { "DragEvent", new StringSetter<EventTrigger>(RegisterDragEvent) },
            { "BeginDragEvent", new StringSetter<EventTrigger>(RegisterDragEvent) },
            { "EndDragEvent", new StringSetter<EventTrigger>(RegisterDragEvent) }
        };
        
        
        private void RegisterDragEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.Drag);
        
        private void RegisterBeginDragEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.BeginDrag);
        
        private void RegisterEndDragEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.EndDrag);
        
        private void RegisterPointerDownEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerDown);

        private void RegisterPointerUpEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerUp);
        
        private void RegisterPointerEnterEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerEnter);

        private void RegisterPointerExitEvent(EventTrigger eventTrigger, string events) =>
            RegisterEvent(eventTrigger, events, EventTriggerType.PointerExit);

        private void RegisterEvent(EventTrigger eventTrigger, string events, EventTriggerType triggerType) {
            OrbitRenderData renderData = CurrentData;

            EventTrigger.Entry entry = new();
            entry.eventID = triggerType;
            entry.callback.AddListener((data) => {
                renderData.EmitEvent(events);
            });
            eventTrigger.triggers.Add(entry);
        }
    }
}