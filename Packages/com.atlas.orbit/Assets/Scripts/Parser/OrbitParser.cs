namespace Atlas.Orbit.Parser {
    public class OrbitParser : UIParser {
        private static OrbitParser parser;
        public static OrbitParser DefaultParser {
            get {
                if(parser == null) {
                    parser = new OrbitParser();
                }

                return parser;
            }
        }
    }
}