namespace Gestures {

    public class Gesture {

        protected GestureType type;

        public Gesture(GestureType type) { this.type = type; }

        public GestureType Type { get { return type; } }
    }
}
