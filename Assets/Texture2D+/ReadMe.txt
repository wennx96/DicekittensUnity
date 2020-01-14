Texture2D+
----------
The Texture2D class in Unity lacks convenient functions to manipulate the image. Of course you can use GetPixels and SetPixels, but it would be a lot easier if simply had functions to properly Scale, Crop and Rotate as well as change Hue, Saturation, Brightness, Luminance, Contrast and Colorize.

You can test these extra Texture2D methods if you run the Demo scene that is included. 

Make sure you include the line
  using Texture2DExtensions;
in your scripts!


Documentation
-------------
The full documentation is available online at http://orbcreation.com/orbcreation/docu.orb
The file at Texture2D+ / Documentation.txt contains a plain text version of this documentation


Package Contents
----------------
Texture2D+ / ReadMe.txt   (this file)
Texture2D+ / Documentation.txt   (documentation in plain text format. better use http://orbcreation.com/orbcreation/docu.orb)
Texture2D+ / Texture2DExtensions.cs   (the actual Texture2D extensions are all you need)
Texture2D+ / Texture2D+Demo / Demo.unity   (the demo scene)
Texture2D+ / Texture2D+Demo / DSC03519.jpg (sample image)


Minimal needed in your project
------------------------------
The Texture2DExtensions.cs script needs to be somewhere in your project folders. All the rest can go.


C# and Javascript
-----------------
If you want to create a Javascript that uses the Texture2D+ package, you will have to place the Texture2DExtensions.cs script in the "Standard Assets", "Pro Standard Assets" or "Plugins" folder and your Javascripts outside of these folders. The code inside the "Standard Assets", "Pro Standard Assets" or "Plugins" is compiled first and the code outside is compiled in a later step making the Types defined in the compilation step (the C# scripts) available to later compilation steps (your Javascript scripts).


