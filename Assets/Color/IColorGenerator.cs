using UnityEngine;

public interface IColorGenerator {
    float Difficulty { set; }
    Color[] Generate(int n);
}
