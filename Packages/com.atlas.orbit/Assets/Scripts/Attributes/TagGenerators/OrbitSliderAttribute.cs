namespace Atlas.Orbit.Attributes.TagGenerators {
    using Parser;
    using System.Xml;
    using Util;

    public class OrbitSliderAttribute : TagGenerator {
        public string Text { get; set; }
        public string ValueChangedEvent { get; set; }
        private bool? wholeNumbers;
        public bool WholeNumbers {
            get => wholeNumbers.GetValueOrDefault();
            set => wholeNumbers = value;
        }

        private int? increments;
        public int Increments {
            get => increments.GetValueOrDefault();
            set => increments = value;
        }

        private bool? immediateUpdate;
        public bool ImmediateUpdate {
            get => immediateUpdate.GetValueOrDefault();
            set => immediateUpdate = value;
        }

        private float min;
        private float max;

        public OrbitSliderAttribute(float min, float max) {
            this.min = min;
            this.max = max;
        }
        
        public override XmlNode GenerateTag(XmlDocument doc, string propertyId) {
            XmlNode node = doc.CreateNode("element", "SettingSlider", null);
            node.AddAttribute("Text", Text ?? propertyId);
            node.AddAttribute("BoundValue", propertyId);
            if(ValueChangedEvent != null)
                node.AddAttribute("ValueChangedEvent", ValueChangedEvent);
            node.AddAttribute("MinValue", min);
            node.AddAttribute("MaxValue", max);
            if(wholeNumbers.HasValue) 
                node.AddAttribute("WholeNumbers", wholeNumbers.Value);
            if(increments.HasValue) 
                node.AddAttribute("Increments", increments.Value);
            if(immediateUpdate.HasValue) 
                node.AddAttribute("ImmediateUpdate", immediateUpdate.Value);
            return node;
        }
    }
}