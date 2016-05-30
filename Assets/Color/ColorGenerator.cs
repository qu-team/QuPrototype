using UnityEngine;

public interface ColorGenerator {

    float MaxRadius { get; }

    Vector3 Center { get; }

    float Radius { get; set; }

    Vector3 Position { get; set; }

    Color Generate();
}
