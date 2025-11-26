using Orbit.Attributes;
using Orbit.Components;

public class ExpressionsView : OrbitView {
    private int _a;
    [ValueID]
    public int a {
        get => _a;
        set {
            _a = value;
            OnPropertyChanged();
        }
    }
    [ListenFor("AddOne")]
    public void AddOne() {
        a++;
    }
}