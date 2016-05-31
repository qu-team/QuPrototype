using UnityEngine;

public interface ColorGenerator {

    float MaxRadius { get; }
    float InitialMinRadius { get; }

    Vector3 Center { get; }

    float Radius { get; set; }
    float MinRadius { get; set; }

    Vector3 Position { get; set; }

    Color Generate();
}
