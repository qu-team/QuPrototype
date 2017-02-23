using UnityEngine;

public class CustomHSLColorGenerator: IColorGenerator{

	public Color[] Generate(int n){
		var startCoordinate = Random.Range(0f,360f);
		Color[] colors = new Color[3];
		colors[0] = getRGB ( startCoordinate );
		colors[1] = getRGB ( startCoordinate + arcAmplitude );
		colors[2] = getRGB ( startCoordinate - arcAmplitude );
		return colors;
	}

	float magic(float coordinate){
		coordinate = Mathf.Repeat(coordinate,360f);
		return Mathf.Max( Mathf.Sin(coordinate /360f * Mathf.PI/2f),0f);
	}

	Color getRGB(float coordinate){
		var r = magic(coordinate);
		var g = magic(coordinate + 120f);
		var b = magic(coordinate + 240f);
		return new Color(r,g,b,1f);
	}
	float arcAmplitude=240f;
	float minAngle;
	public float Difficulty{
		set{
			arcAmplitude = 240f/(value+1f);
		}
	}


}
