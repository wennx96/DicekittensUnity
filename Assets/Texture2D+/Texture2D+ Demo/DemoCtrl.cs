using UnityEngine;
using System.Collections;
using System.IO;

using OrbCreationExtensions;   // important that you put this in your scripts!!!

public class DemoCtrl : MonoBehaviour {

	public Texture2D baseTexture;
	public Texture2D bg;
	private Texture2D changedTexture;

	private float cropX;
	private float cropY;
	private float scaleX;
	private float scaleY;
	private float degrees;
	private bool mirrorX;
	private bool mirrorY;
	private float deltaHue;
	private float deltaSaturation;
	private float deltaBrightness;
	private float deltaContrast;
	private float deltaLuminance;
	private float colorize;
	private bool normalMap;
	private float transparancy = 0f;
	private float replaceColorTolerance = 0f;
	private float fade = 0f;

	private int screenshotCounter = 0;

	// Calling the extra Texture2D methods
	private void Recalc() {
		// get a fresh copy of the original
		changedTexture = (Texture2D)Texture2D.Instantiate(baseTexture); 

		// Crop takes a Rect as argument with x, y is offset and width and height = new size;
		changedTexture.Crop(new Rect(0,0,cropX,cropY));

		// Scale takes the new width and height as integer arguments
		// we compute the absolute values, because the slider has a relative value between 0.0 and 1.5
		int newWidth = Mathf.FloorToInt(scaleX*changedTexture.width);
		int newHeight = Mathf.FloorToInt(scaleY*changedTexture.height);
		changedTexture.Scale(newWidth, newHeight);

		// Rotate takes the nr of degrees as argument. 
		// If you use 90, 180, 270, the rotating works a little faster than with arbitrary rotations.
		changedTexture.Rotate(degrees);

		// You can mirror horizontally and vertically, if you do both it is the same as rotating 180 degrees
		changedTexture.Mirror(mirrorX, mirrorY);

		// The arguments are floats between -1 and +1
		changedTexture.Hue(deltaHue);
		changedTexture.Saturation(deltaSaturation);
		changedTexture.Brightness(deltaBrightness);

		// The argument stretches contrast (values > 0.0) or diminishes the contrast (values < 0.0)
		changedTexture.Contrast(deltaContrast);

		// Argument between -1 (image full black) and 1 (image full white)
		changedTexture.Luminance(deltaLuminance);

		// Here we take Sepia (brwon/yellow) as the color value, but you can use anything you like
		// the parameter colorize is a float between 0 and 1 and specifies the intensity of the colorization
		changedTexture.Colorize(new Color(112/112f, 66/112f, 20/112f), colorize);

		if(normalMap) changedTexture.ConvertToNormalMap(0.8f);

		changedTexture.MakeColorTransparent(new Color(114/255f, 144/255f, 56/255f), transparancy); //, transparancyTolerance);
		changedTexture.MakeColorTransparent(new Color(60/255f, 90/255f, 25/255f), transparancy); //, transparancyTolerance);
		changedTexture.MakeColorTransparent(new Color(11/255f, 23/255f, 0/255f), transparancy); //, transparancyTolerance);

		changedTexture.ReplaceColor(new Color(114/255f, 144/255f, 56/255f), new Color(0, 0, 0/255f), replaceColorTolerance);
		changedTexture.ReplaceColor(new Color(60/255f, 90/255f, 25/255f), new Color(0, 0, 0/255f), replaceColorTolerance);
		changedTexture.ReplaceColor(new Color(11/255f, 23/255f, 0/255f), new Color(0, 0, 0/255f), replaceColorTolerance);

		changedTexture.FadeToColor(new Color(179/255f, 194/255f, 136/255f), fade);

	}

	void Start () {

		Reset();
	}
	private void Reset() {
		cropX = baseTexture.width;
		cropY = baseTexture.height;
		scaleX = 1f;
		scaleY = 1f;
		degrees = 0f;
		mirrorX = false;
		mirrorY = false;
		deltaHue = 0f;
		deltaSaturation = 0f;
		deltaBrightness = 0f;
		deltaContrast = 0f;
		deltaLuminance = 0f;
		colorize = 0f;
		Recalc();
	}
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.P)) StartCoroutine(Screenshot());
	}

	void OnGUI () {
		bool oldBool;
		float oldFloat;
		float margin = 10f;
		float x = margin;
		float y = margin;
		float w0 = Screen.width - (2*margin);
		float w1 = 100f;
		float w2 = 200f;
		float h = 20f;

		GUI.skin.label.normal.textColor = Color.black;
		GUI.skin.label.padding= new RectOffset(0,0,0,0);
		GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), bg);

		GUI.skin.horizontalSlider.padding= new RectOffset(0,0,0,0);
		GUI.DrawTexture(new Rect(margin, Screen.height-margin-baseTexture.height, baseTexture.width, baseTexture.height), baseTexture);
		GUI.DrawTexture(new Rect(margin+baseTexture.width+margin, Screen.height-margin-changedTexture.height, changedTexture.width, changedTexture.height), changedTexture);

		GUI.Label(new Rect(x,y,w0,h), "Texture2D+ Demo");
		y+=h;
		GUI.Label(new Rect(x,y,w0,h), "Put \"using Texture2DExtensions\" in the top of your scripts");
		y+=h;

		GUI.Label(new Rect(x,y,w1,h), "Crop:");
		oldFloat = cropX;
		cropX = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), cropX, 1f, baseTexture.width);
		if(cropX!=oldFloat) Recalc();
		y+=h;
		oldFloat = cropY;
		cropY = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), cropY, 1f, baseTexture.height);
		if(cropY!=oldFloat) Recalc();
		y+=h;

		GUI.Label(new Rect(x,y,w1,h), "Scale:");
		oldFloat = scaleX;
		scaleX = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), scaleX, 0f, 1.5f);
		if(scaleX!=oldFloat) Recalc();
		y+=h;
		oldFloat = scaleY;
		scaleY = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), scaleY, 0f,1.5f);
		if(scaleY!=oldFloat) Recalc();
		y+=h;

		GUI.Label(new Rect(x,y,w1,h), "Rotate:");
		oldFloat = degrees;
		degrees = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), degrees, -180f, 180f);
		if(degrees!=oldFloat) Recalc();
		y+=h;

		GUI.skin.toggle.normal.textColor = Color.black;
		GUI.Label(new Rect(x,y,w1,h), "Mirror:");
		oldBool = mirrorX;
		mirrorX = GUI.Toggle(new Rect(x+w1,y,w2,h), mirrorX," horizontal");
		if(mirrorX!=oldBool) Recalc();
		y+=h;
		oldBool = mirrorY;
		mirrorY = GUI.Toggle(new Rect(x+w1,y,w2,h), mirrorY," vertical");
		if(mirrorY!=oldBool) Recalc();
		y+=h;
		if(GUI.Button(new Rect(x,y,w1,h), "Reset")) Reset();

		x+=w1+w2+margin;
		y=margin+(2*h);
		GUI.Label(new Rect(x,y,w1,h), "Hue:");
		oldFloat = deltaHue;
		deltaHue = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), deltaHue, -1f, 1f);
		if(deltaHue!=oldFloat) Recalc();
		y+=h;
		GUI.Label(new Rect(x,y,w1,h), "Saturation:");
		oldFloat = deltaSaturation;
		deltaSaturation = GUI.HorizontalSlider(new Rect(x+w1,y,w2,h), deltaSaturation, -1f, 1f);
		if(deltaSaturation!=oldFloat) Recalc();
		y+=h;
		GUI.Label(new Rect(x,y,w1,h), "Brightness:");
		oldFloat = deltaBrightness;
		deltaBrightness = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), deltaBrightness, -1f, 1f);
		if(deltaBrightness!=oldFloat) Recalc();
		y+=h;
		GUI.Label(new Rect(x,y,w1,h), "Contrast:");
		oldFloat = deltaContrast;
		deltaContrast = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), deltaContrast, -6f, 6f);
		if(deltaContrast!=oldFloat) Recalc();
		y+=h;
		GUI.Label(new Rect(x,y,w1,h), "Luminance:");
		oldFloat = deltaLuminance;
		deltaLuminance = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), deltaLuminance, -1f, 1f);
		if(deltaLuminance!=oldFloat) Recalc();
		y+=h;

		GUI.Label(new Rect(x,y,w1,h), "Colorize:");
		oldFloat = colorize;
		colorize = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), colorize, 0f, 1f);
		if(colorize!=oldFloat) Recalc();
		y+=h;
		GUI.Label(new Rect(x+w1,y,w2,h*2), "(Using sepia for demo color.\nTip: change saturation as well)");

		x+=w1+w2+margin;
		y=margin+(2*h);
		GUI.Label(new Rect(x,y,w1,h*2), "Fade to color:");
		oldFloat = fade;
		fade = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), fade, 0f, 1f);
		if(fade!=oldFloat) Recalc();
		y+=h-3;
		GUI.Label(new Rect(x+w1,y,w2,h), "(Using pale green as fade color)");
		y+=h-10;
		GUI.Label(new Rect(x,y,w1,h*2), "Make color transparent:");
		y+=h;
		oldFloat = transparancy;
		transparancy = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), transparancy, 0f, 1f);
		if(transparancy!=oldFloat) Recalc();
		y+=h-3;
		GUI.Label(new Rect(x+w1,y,w2,h), "(Filtering 3 shades of green)");
		y+=h;
		GUI.Label(new Rect(x,y,w1,h), "Replace color:");
		oldFloat = replaceColorTolerance;
		replaceColorTolerance = GUI.HorizontalSlider(new Rect(x+w1,y+4,w2,h), replaceColorTolerance, 0f, 1f);
		if(replaceColorTolerance!=oldFloat) Recalc();
		y+=h-3;
		GUI.Label(new Rect(x+w1,y,w2,h), "(Using black instead of alpha)");
		y+=h;
		GUI.Label(new Rect(x,y,w1,h), "Normal map:");
		oldBool = normalMap;
		normalMap = GUI.Toggle(new Rect(x+w1,y,w2,h), normalMap,"");
		if(normalMap!=oldBool) Recalc();
		y+=h;


	}

	// To make the screenshots used for the Asset Store submission
	private IEnumerator Screenshot() {
		yield return new WaitForEndOfFrame(); // wait for end of frame to include GUI

		Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		screenshot.Apply(false);

		if(Application.platform==RuntimePlatform.OSXPlayer || Application.platform==RuntimePlatform.WindowsPlayer && Application.platform!=RuntimePlatform.LinuxPlayer || Application.isEditor) {
			byte[] bytes = screenshot.EncodeToPNG();
			FileStream fs = new FileStream("Screenshot"+screenshotCounter+".png", FileMode.OpenOrCreate);
			BinaryWriter w = new BinaryWriter(fs);
			w.Write(bytes);
			w.Close();
			fs.Close();
		}
		screenshotCounter++;

	}

}
