using Atlas.Orbit.Parser;
using Atlas.Orbit.TypeSetters;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Atlas.Orbit.ComponentProcessors {
    using Parser;
    using TypeSetters;

    public class EventTriggerProcessor : ComponentProcessor<EventTrigger> {
        public override Dictionary<string, TypeSetter<EventTrigger>> Setters => new() {
            {"HoveredValue", new StringSetter<EventTrigger>(RegisterHovered) },
            {"HoveredEvent", new StringSetter<EventTrigger>(RegisterHoveredEvent) }
        };

        private void RegisterHoveredEvent(EventTrigger eventTrigger, string events) {
            UIRenderData renderData = CurrentData;

            EventTrigger.Entry pointerEnter = new();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => {
                renderData.EmitEvent(events);
            });
            eventTrigger.triggers.Add(pointerEnter);
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
    }
}
