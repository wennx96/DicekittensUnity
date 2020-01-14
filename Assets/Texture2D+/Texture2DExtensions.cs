/* 
Texture2D+ by Orbcreation BV
Version 1.01

Use these functions by putting "using Texture2DExtensions" in the top section of your scripts
*/

using UnityEngine;
using System;
using System.Collections;

namespace OrbCreationExtensions
{
	public static class Texture2DExtensions
    {
		public static void Crop(this Texture2D tex, Rect cropRect) {
			// Make sure the crop rectangle stays within the original Texture dimensions
			cropRect.x = Mathf.Clamp(cropRect.x, 0, tex.width);
			cropRect.width = Mathf.Clamp(cropRect.width, 0, tex.width - cropRect.x);
			cropRect.y = Mathf.Clamp(cropRect.y, 0, tex.height);
			cropRect.height = Mathf.Clamp(cropRect.height, 0, tex.height - cropRect.y);
			if(cropRect.height<tex.height || cropRect.width<tex.width) { // only crop if needed
				if(cropRect.height>0 || cropRect.width>0) { // dont create a Texture with size 0
					MakeFormatWritable(tex);
					Color[] pixels = tex.GetPixels((int)cropRect.x, (int)cropRect.y, (int)cropRect.width, (int)cropRect.height, 0);
					if(tex.Resize(Mathf.FloorToInt(cropRect.width), Mathf.FloorToInt(cropRect.height), tex.format, tex.mipmapCount>1)) {
						tex.SetPixels(pixels);
						tex.Apply((tex.mipmapCount > 1), false);
					}
				}
			}
		}

		public static void Scale(this Texture2D tex, int width, int height) {
			if(width<=0 || height<=0 || (width==tex.width && height==tex.height)) return;
			MakeFormatWritable(tex);
			Color[] newPixels = ScaledPixels(tex.GetPixels(0), tex.width, tex.height, width, height);
			if(tex.Resize(width, height, tex.format, tex.mipmapCount>1)) {
				tex.SetPixels(newPixels,0);
				tex.Apply((tex.mipmapCount > 1), false);
			}
		}

		public static void Rotate(this Texture2D tex, float degrees) {
			MakeFormatWritable(tex);
			float angle = degrees * Mathf.Deg2Rad;
			float x2x = Mathf.Cos(angle);
			float y2x = Mathf.Sin(angle) * -1.0f;
			float x2y = Mathf.Sin(angle);
			float y2y = Mathf.Cos(angle);
			float centerX = tex.width /  2.0f;
			float centerY = tex.height /  2.0f;
			int oldSizeX = tex.width;
			int oldSizeY = tex.height;
			int newSizeX = 0;
			int newSizeY = 0;
			Color[] originalPixels = tex.GetPixels(0);
			Color[] newPixels;

			if(Mathf.RoundToInt(degrees)%90 == 0) {  // use fast 90 degrees rotation method
				int x2xi = Mathf.RoundToInt(Mathf.Cos(angle));
				int y2xi = Mathf.RoundToInt(Mathf.Sin(angle)) * -1;
				int x2yi = Mathf.RoundToInt(Mathf.Sin(angle));
				int y2yi = Mathf.RoundToInt(Mathf.Cos(angle));
				newSizeX = Mathf.Abs((x2xi * oldSizeX) + (y2xi * oldSizeY));
				newSizeY = Mathf.Abs((x2yi * oldSizeX) + (y2yi * oldSizeY));

				newPixels = new Color[newSizeX * newSizeY];
				for (int y = 0; y < oldSizeY; y++) {
					for (int x = 0; x < oldSizeX; x++) {
						int newX = (x2xi * x) + (y2xi * y);
						int newY = (x2yi * x) + (y2yi * y);
						if((y2xi+x2xi)<0) newX+=newSizeX-1;
						if((x2yi+y2yi)<0) newY+=newSizeY-1;
						newPixels[(newY * newSizeX) + newX] = originalPixels[(y * oldSizeX) + x];
					}
				}
			} else { // use slower arbitrary rotation method

				// rotate 4 corners to determine the size of the new texture
				float maxX = 0f;
				float maxY = 0f;
				float oldX = oldSizeX - centerX;
				float oldY = oldSizeY - centerY;
				maxX = Mathf.Max(maxX,Mathf.Abs((x2x * oldX) + (y2x * oldY)));
				maxY = Mathf.Max(maxY,Mathf.Abs((x2y * oldX) + (y2y * oldY)));

				oldX = centerX - oldSizeX;
				oldY = oldSizeY - centerY;
				maxX = Mathf.Max(maxX,Mathf.Abs((x2x * oldX) + (y2x * oldY)));
				maxY = Mathf.Max(maxY,Mathf.Abs((x2y * oldX) + (y2y * oldY)));

				oldX = oldSizeX - centerX;
				oldY = centerY - oldSizeY;
				maxX = Mathf.Max(maxX,Mathf.Abs((x2x * oldX) + (y2x * oldY)));
				maxY = Mathf.Max(maxY,Mathf.Abs((x2y * oldX) + (y2y * oldY)));

				oldX = centerX - oldSizeX;
				oldY = centerY - oldSizeY;
				maxX = Mathf.Max(maxX,Mathf.Abs((x2x * oldX) + (y2x * oldY)));
				maxY = Mathf.Max(maxY,Mathf.Abs((x2y * oldX) + (y2y * oldY)));
				newSizeX = Mathf.RoundToInt(maxX * 2f);
				newSizeY = Mathf.RoundToInt(maxY * 2f);

				newPixels = new Color[newSizeX * newSizeY];
				float[] newColors = new float[newSizeX * newSizeY * 5]; // array of rgba colors and sum of weights

				for (int y = 0; y < oldSizeY; y++) {
					for (int x = 0; x < oldSizeX; x++) {
						Color originalPixel = originalPixels[(y * oldSizeX) + x];
						oldX = (float)x - centerX;
						oldY = (float)y - centerY;
						float newX = ((x2x * oldX) + (y2x * oldY)) + (newSizeX / 2f);
						float newY = ((x2y * oldX) + (y2y * oldY)) + (newSizeY / 2f);
						int yLow = Mathf.FloorToInt(newY);
						int yHigh = Mathf.CeilToInt(newY);
						int xLow = Mathf.FloorToInt(newX);
						int xHigh = Mathf.CeilToInt(newX);
						if(yLow >= 0 && yHigh < newSizeY && xLow >= 0 && xHigh < newSizeX) {
							float weight = (1.0f - (newX-xLow)) * (1.0f - (newY-yLow));
							int idx = ((yLow * newSizeX) + xLow) * 5;
							newColors[idx] += originalPixel.r * weight;
							newColors[idx+1] += originalPixel.g * weight;
							newColors[idx+2] += originalPixel.b * weight;
							newColors[idx+3] += originalPixel.a * weight;
							newColors[idx+4] += weight;

							weight = (1.0f - (newX-xLow)) * (1.0f - (yHigh-newY));
							idx = ((yHigh * newSizeX) + xLow) * 5;
							newColors[idx] += originalPixel.r * weight;
							newColors[idx+1] += originalPixel.g * weight;
							newColors[idx+2] += originalPixel.b * weight;
							newColors[idx+3] += originalPixel.a * weight;
							newColors[idx+4] += weight;

							weight = (1.0f - (xHigh-newX)) * (1.0f - (newY-yLow));
							idx = ((yLow * newSizeX) + xHigh) * 5;
							newColors[idx] += originalPixel.r * weight;
							newColors[idx+1] += originalPixel.g * weight;
							newColors[idx+2] += originalPixel.b * weight;
							newColors[idx+3] += originalPixel.a * weight;
							newColors[idx+4] += weight;

							weight = (1.0f - (xHigh-newX)) * (1.0f - (yHigh-newY));
							idx = ((yHigh * newSizeX) + xHigh) * 5;
							newColors[idx] += originalPixel.r * weight;
							newColors[idx+1] += originalPixel.g * weight;
							newColors[idx+2] += originalPixel.b * weight;
							newColors[idx+3] += originalPixel.a * weight;
							newColors[idx+4] += weight;
						}
					}
				}

				// convert float values into colors
				for (int i = 0; i < newPixels.Length; i++) {
					int idx = i*5;
					float weight = newColors[idx+4];
					if(weight<=0f) weight = 1f; 
					newPixels[i].r = newColors[idx] / weight;
					newPixels[i].g = newColors[idx+1] / weight;
					newPixels[i].b = newColors[idx+2] / weight;
					newPixels[i].a = newColors[idx+3] / weight;
				}
			}
			if(tex.Resize(newSizeX, newSizeY, tex.format, tex.mipmapCount>1)) {
				tex.SetPixels(newPixels, 0);
				tex.Apply((tex.mipmapCount > 1), false);
			}
		}

		public static void Mirror(this Texture2D tex, bool horizontal, bool vertical) {
			Color[] originalPixels = tex.GetPixels(0);
			Color[] newPixels = new Color[tex.width * tex.height];
			if((!horizontal) && (!vertical)) return;
			MakeFormatWritable(tex);
			for (int y = 0; y < tex.height; y++) {
				for (int x = 0; x < tex.width; x++) {
					int newX = horizontal ? (tex.width-1-x) : x;
					int newY = vertical ? (tex.height-1-y) : y;
					newPixels[(newY * tex.width) + newX] = originalPixels[(y * tex.width) + x];
				}
			}
			tex.SetPixels(newPixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}


		public static void Hue(this Texture2D tex, float deltaHue) {
			tex.ChangeHSB(deltaHue, 0f, 0f);
		}
		public static void Saturation(this Texture2D tex, float deltaSaturation) {
			tex.ChangeHSB(0f, deltaSaturation, 0f);
		}
		public static void Brightness(this Texture2D tex, float deltaBrightness) {
			tex.ChangeHSB(0f, 0f, deltaBrightness);
		}
		public static void ChangeHSB(this Texture2D tex, float deltaHue, float deltaSaturation, float deltaBrightness) {
			MakeFormatWritable(tex);
			Color[] pixels = tex.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				Vector3 hsb = MakeHSB(pixels[i]);
				hsb.x += deltaHue;
				hsb.y += deltaSaturation;
				hsb.z += deltaBrightness;
				Color c = MakeColor(hsb);
				pixels[i].r = c.r;
				pixels[i].g = c.g;
				pixels[i].b = c.b;
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}

		public static void InsertTexture(this Texture2D targetTex, Texture2D tex, int offsetX, int offsetY) {
			MakeFormatWritable(targetTex);
			Color[] targetPixels = targetTex.GetPixels(0);
			Color[] pixels = tex.GetPixels(0);
			for (int y = 0; y < tex.height; y++) {
				int targetY = y + offsetY;
				for (int x = 0; x < tex.width; x++) {
					int targetX = x + offsetX;
					targetPixels[(targetY * targetTex.width) + targetX] = pixels[(y * tex.width) + x];
				}
			}
			targetTex.SetPixels(targetPixels, 0);
			targetTex.Apply((targetTex.mipmapCount > 1), false);
		}


		public static void Luminance(this Texture2D tex, float deltaLuminance) {  
			if(deltaLuminance==0f) return;
			MakeFormatWritable(tex);
			Color[] pixels = tex.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				pixels[i].r+=deltaLuminance;
				pixels[i].g+=deltaLuminance;
				pixels[i].b+=deltaLuminance;
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}

		public static void Contrast(this Texture2D tex, float deltaContrast) {  
			if(deltaContrast==0f) return;
			MakeFormatWritable(tex);
			float multiplier = 1f;
			if(deltaContrast<0f) multiplier = 1f / (1f - deltaContrast);
			else multiplier = 1f + deltaContrast;
			Color[] pixels = tex.GetPixels(0);

			for (int i = 0; i < pixels.Length; i++) {
				Color c = pixels[i];
				float deltaR = c.r - 0.5f;
				float deltaG = c.g - 0.5f;
				float deltaB = c.b - 0.5f;
				deltaR *= multiplier;
				deltaG *= multiplier;
				deltaB *= multiplier;
				pixels[i] = new Color(0.5f + deltaR, 
					0.5f + deltaG, 
					0.5f + deltaB, 
					c.a);
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}

		public static void Colorize(this Texture2D tex, Color color, float amount) {
			amount = Mathf.Clamp01(amount);
			if(amount==0f) return;
			MakeFormatWritable(tex);

			Color[] pixels = tex.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				float b = (pixels[i].r + pixels[i].g + pixels[i].b ) / 3f;
				pixels[i].r = (pixels[i].r * (1f - amount)) + (b * color.r * amount);
				pixels[i].g = (pixels[i].g * (1f - amount)) + (b * color.g * amount);
				pixels[i].b = (pixels[i].b * (1f - amount)) + (b * color.b * amount);
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}

		public static void MakeColorTransparent(this Texture2D tex, Color color, float amount) {
			if(amount==0f) return;
			MakeFormatWritable(tex);
			MakeColorTransparent(tex, color, Mathf.Sqrt(amount), amount*0.5f);
		}
		public static void MakeColorTransparent(this Texture2D tex, Color color, float amount, float tolerance) {
			amount = Mathf.Clamp01(amount);
			if(amount==0f) return;
			MakeFormatWritable(tex);
			float power = (tolerance * 0.75f) + 0.25f;
			Color[] pixels = tex.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				float d = pixels[i].GetHSBColorDistance(color, power);
				pixels[i].a = pixels[i].a - (amount * Mathf.Clamp01(1f - d));
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}
		public static void ReplaceColor(this Texture2D tex, Color color, Color newColor, float tolerance) {
			if(tolerance<=0f) return;
			MakeFormatWritable(tex);
			float power = (tolerance * 0.25f) + 0.15f;
			Color[] pixels = tex.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				float d = Mathf.Clamp01(pixels[i].GetHSBColorDistance(color, power));
				pixels[i].r = (pixels[i].r * d) + (newColor.r * (1f - d));
				pixels[i].g = (pixels[i].g * d) + (newColor.g * (1f - d));
				pixels[i].b = (pixels[i].b * d) + (newColor.b * (1f - d));
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}
		public static void FadeToColor(this Texture2D tex, Color color, float amount) {
			if(amount<=0f) return;
			MakeFormatWritable(tex);
			amount = Mathf.Clamp01(amount);
			Color[] pixels = tex.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++) {
				pixels[i].r = (pixels[i].r * (1f - amount)) + (color.r * amount);
				pixels[i].g = (pixels[i].g * (1f - amount)) + (color.g * amount);
				pixels[i].b = (pixels[i].b * (1f - amount)) + (color.b * amount);
			}
			tex.SetPixels(pixels, 0);
			tex.Apply((tex.mipmapCount > 1), false);
		}

/* ------------------------------------------------------------------------------------*/
/* ------------------ copy functions --------------------------------------------------*/
/* ------------------------------------------------------------------------------------*/
		public static Texture2D GetCopy(this Texture2D tex) {
			return GetCopy(tex, 0, 0, tex.width, tex.height, (tex.mipmapCount > 1));
		}
		public static Texture2D GetCopy(this Texture2D tex, int x, int y, int w, int h) {
			return GetCopy(tex, x, y, w, h, (tex.mipmapCount > 1));
		}
		public static Texture2D GetSection(this Texture2D tex, int x, int y, int w, int h) {
			return GetCopy(tex, x, y, w, h, (tex.mipmapCount > 1));
		}
		public static Texture2D GetCopy(this Texture2D tex, int x, int y, int w, int h, bool mipMaps) {
			Texture2D newTex = new Texture2D(w, h, GetWritableFormat(tex.format), mipMaps);
			newTex.SetPixels(tex.GetPixels(x, y, w, h, 0), 0);
			newTex.Apply(mipMaps, false);
			return newTex;
		}
		public static Texture2D ScaledCopy(this Texture2D tex, int width, int height, bool mipMaps) {
			if(width<=0 || height<=0) return null;
			if(width==tex.width && height==tex.height) return GetCopy(tex, 0, 0, tex.width, tex.height, mipMaps);
			Color[] newPixels = ScaledPixels(tex.GetPixels(0), tex.width, tex.height, width, height);
			Texture2D newTex = new Texture2D(width, height, GetWritableFormat(tex.format), mipMaps);
			newTex.SetPixels(newPixels,0);
			newTex.Apply(mipMaps, false);
			return newTex;
		}

		
		public static void RevertTo(this Texture2D tex, Texture2D original) {
			CopyFrom(tex, original);
		}
		public static void MakeEqual(this Texture2D tex, Texture2D original) {
			CopyFrom(tex, original);
		}
		public static void CopyFrom(this Texture2D tex, Texture2D original) {
			CopyFrom(tex, original, original.mipmapCount>1);
		}
		public static void CopyFrom(this Texture2D tex, Texture2D original, bool mipMaps) {
			if(tex.Resize(original.width, original.height, GetWritableFormat(original.format), mipMaps)) {
				tex.SetPixels(original.GetPixels(0),0);
				tex.Apply(mipMaps, false);
			}
		}
		public static void CopyFrom(this Texture2D tex, Texture2D fromTex, int toX, int toY, int fromX, int fromY, int width, int height) {
			MakeFormatWritable(tex);
			int fullWidth = tex.width;
			Color[] pixels = tex.GetPixels(0);
			Color[] fromPixels = fromTex.GetPixels(fromX, fromY, width, height, 0);
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					pixels[((y + toY) * fullWidth) + x + toX] = fromPixels[(y * width) + x];
				}
			}
			tex.SetPixels(pixels,0);
			tex.Apply((tex.mipmapCount > 1), false);
		}

		public static Color GetAverageColor(this Texture2D tex) {
    		Vector4 c = Vector4.zero;
    		float tot = 0f;
    		Color[] pxls = tex.GetPixels(0);
    		for(int i=0;i<pxls.Length;i++) {
        		c += ((Vector4)pxls[i]) * pxls[i].a;
        		tot += pxls[i].a;
        	}
        	if(tot < 1f) tot = 1f;
        	c.w = tot;
        	return (Color)(c / tot);
        }
		public static Color GetAverageColor(this Texture2D tex, Color useThisColorForAlpha) {
    		Vector4 c = Vector4.zero;
    		float tot = 0f;
    		Color[] pxls = tex.GetPixels(0);
    		for(int i=0;i<pxls.Length;i++) {
        		c += ((Vector4)pxls[i]) * pxls[i].a;
        		c += ((Vector4)useThisColorForAlpha) * (1f - pxls[i].a);
        		tot += 1f;
        	}
        	if(tot < 1f) tot = 1f;
        	c.w = tot;
        	return (Color)(c / tot);
        }

/* ------------------------------------------------------------------------------------*/
/* ------------------ normal map functions --------------------------------------------*/
/* ------------------------------------------------------------------------------------*/
	    public static bool IsNormalMap(this Texture2D aTexture) {
	        Color avgColor = aTexture.GetAverageColor();
	        float rg = (avgColor.r + avgColor.g) / 2f;
	        if(rg <= 0f) rg = 1f;
	        float relativeBlue = avgColor.b / rg;
//	          Debug.Log("avgColor:"+avgColor+" blue/rg "+relativeBlue);
	        //  tested the average color of few normalmaps and images with a lot of blue in it
	        //  normalmap1  avgColor:RGBA(0.407, 0.405, 0.796, 1.000) blue/rg 1.958531
	        //  normalmap2  avgColor:RGBA(0.498, 0.498, 0.995, 1.000) blue/rg 1.999087
	        //  normalmap3  avgColor:RGBA(0.502, 0.502, 0.994, 1.000) blue/rg 1.98113
	        //  blueimage1  avgColor:RGBA(0.360, 0.577, 0.770, 1.000) blue/rg 1.643365
	        //  blueimage2  avgColor:RGBA(0.234, 0.340, 0.561, 1.000) blue/rg 1.953209
	        //  blueimage3  avgColor:RGBA(0.371, 0.659, 0.869, 1.000) blue/rg 1.688144
	        //  blueimage4  avgColor:RGBA(0.529, 0.681, 0.967, 1.000) blue/rg 1.59855
	        //  typically normal maps have lots of blue, when compared to red and green
	        //  red and green have almost identical values
	        //  So I will use 2 criteria:
	        //          1: relative blue value > 1.8 (blue / avg(red and green))
	        //          2: difference between red and green < 0.1
	        if(relativeBlue > 1.8f && Mathf.Abs(avgColor.r - avgColor.g) < 0.1f) return true;
	        return false;
	    }

	    public static void ConvertToNormalMap(this Texture2D tex, float strength) {
	        Color[] pixels = tex.GetPixels(0);
	    	if(tex.format != TextureFormat.RGB24) tex.Resize(tex.width, tex.height, TextureFormat.RGB24, tex.mipmapCount>1);
	        Color[] nPixels = new Color[pixels.Length];
	        for (int y=0; y<tex.height; y++) {
	            for (int x=0; x<tex.width; x++) {
	                int x_1 = x-1;
	                if(x_1 < 0) x_1 = tex.width - 1; // use the opposit side to repeat the texture
	                int x1 = x+1;
	                if(x1 >= tex.width) x1 = 0; // use the opposit side to repeat the texture
	                int y_1 = y-1;
	                if(y_1 < 0) y_1 = tex.height - 1; // use the opposit side to repeat the texture
	                int y1 = y+1;
	                if(y1 >= tex.height) y1 = 0; // use the opposit side to repeat the texture
	                float grayX_1 = pixels[(y * tex.width) + x_1].GrayScale();
	                float grayX1 = pixels[(y * tex.width) + x1].GrayScale();
	                float grayY_1 = pixels[(y_1 * tex.width) + x].GrayScale();
	                float grayY1 = pixels[(y1 * tex.width) + x].GrayScale();
	                Vector3 vx = new Vector3(0, 1, (grayX_1 - grayX1) * strength);
	                Vector3 vy = new Vector3(1, 0, (grayY_1 - grayY1) * strength);
	                Vector3 n = Vector3.Cross(vy, vx).normalized;
	                nPixels[(y * tex.width) + x] = (Vector4)((n + Vector3.one) * 0.5f);
	            }
	        }
	        tex.SetPixels(nPixels, 0);
	        tex.Apply(tex.mipmapCount > 1, false);
	    }

	    public static Texture2D GetNormalMap(this Texture2D tex, float strength) {
	        Texture2D normalTexture = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, tex.mipmapCount > 1);
	        Color[] pixels = tex.GetPixels(0);
	    	if(tex.format != TextureFormat.RGB24) tex.Resize(tex.width, tex.height, TextureFormat.RGB24, tex.mipmapCount>1);
	        Color[] nPixels = new Color[pixels.Length];
	        for (int y=0; y<tex.height; y++) {
	            for (int x=0; x<tex.width; x++) {
	                int x_1 = x-1;
	                if(x_1 < 0) x_1 = tex.width - 1; // use the opposit side to repeat the texture
	                int x1 = x+1;
	                if(x1 >= tex.width) x1 = 0; // use the opposit side to repeat the texture
	                int y_1 = y-1;
	                if(y_1 < 0) y_1 = tex.height - 1; // use the opposit side to repeat the texture
	                int y1 = y+1;
	                if(y1 >= tex.height) y1 = 0; // use the opposit side to repeat the texture
	                float grayX_1 = pixels[(y * tex.width) + x_1].GrayScale();
	                float grayX1 = pixels[(y * tex.width) + x1].GrayScale();
	                float grayY_1 = pixels[(y_1 * tex.width) + x].GrayScale();
	                float grayY1 = pixels[(y1 * tex.width) + x].GrayScale();
	                Vector3 vx = new Vector3(0, 1, (grayX_1 - grayX1) * strength);
	                Vector3 vy = new Vector3(1, 0, (grayY_1 - grayY1) * strength);
	                Vector3 n = Vector3.Cross(vy, vx).normalized;
	                nPixels[(y * tex.width) + x] = (Vector4)((n + Vector3.one) * 0.5f);
	            }
	        }
	        normalTexture.SetPixels(nPixels, 0);
	        normalTexture.Apply(tex.mipmapCount > 1, false);
	        return normalTexture;
	    }

	    public static Texture2D GetUnityNormalMap(this Texture2D tex) {
	    	return ToUnityNormalMap(tex);
	    }
	    public static Texture2D ToUnityNormalMap(this Texture2D tex) {
	        Texture2D normalTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, tex.mipmapCount > 1);
	        Color[] pixels = tex.GetPixels(0);
	        Color[] nPixels = new Color[pixels.Length];
	        for (int y=0; y<tex.height; y++) {
	            for (int x=0; x<tex.width; x++) {
	                Color p = pixels[(y * tex.width) + x];
	                Color np = new Color(0,0,0,0);
	                np.r = p.g;
	                np.g = p.g; // waste of memory space if you ask me
	                np.b = p.g;
	                np.a = p.r;  
	                nPixels[(y * tex.width) + x] = np;
	            }
	        }
	        normalTexture.SetPixels(nPixels, 0);
	        normalTexture.Apply(tex.mipmapCount > 1, false);
	        return normalTexture;
	    }
	    public static Texture2D FromUnityNormalMap(this Texture2D tex) {
	        Texture2D normalTexture = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, tex.mipmapCount > 1);
	        Color[] pixels = tex.GetPixels(0);
	        Color[] nPixels = new Color[pixels.Length];
	        for (int y=0; y<tex.height; y++) {
	            for (int x=0; x<tex.width; x++) {
	                Color p = pixels[(y * tex.width) + x];
	                Color np = new Color(0,0,0,0);
	                np.g = p.r;
	                np.r = p.a;
	                np.b = 1f;
	                nPixels[(y * tex.width) + x] = np;
	            }
	        }
	        normalTexture.SetPixels(nPixels, 0);
	        normalTexture.Apply(tex.mipmapCount > 1, false);
	        return normalTexture;
	    }

	    public static void Fill(this Texture2D tex, Color aColor) {
			MakeFormatWritable(tex);
	        Color[] pixels = tex.GetPixels(0);
	        for (int i=0; i<pixels.Length; i++) {
                pixels[i] = aColor;
	        }
	        tex.SetPixels(pixels, 0);
	        tex.Apply(tex.mipmapCount > 1, false);
		}

	    public static bool IsReadable(this Texture2D tex) {
	    	try {
		        tex.GetPixels(0, 0, 1, 1, 0);
	        } catch(Exception e) {
	        	return e == null;
	        }
	        return true;
	    }

	    public static bool HasTransparency(this Texture2D tex) {
	    	Color[] pixels;
	    	try {
		        pixels = tex.GetPixels(0);
	        } catch(Exception e) {
	        	Debug.Log(e);
	        	return false;
	        }
	        for (int i=0; i<pixels.Length; i++) {
	        	if(pixels[i].a < 1f) return true;
	        }
	        return false;
	    }

		public static void MakeFormatWritable(this Texture2D tex) {
			TextureFormat oldFormat = tex.format;
			TextureFormat newFormat = GetWritableFormat(tex.format);
			if(newFormat != oldFormat) {
				tex.Resize(tex.width, tex.height, newFormat, tex.mipmapCount>1);
			}
		}

		public static TextureFormat GetWritableFormat(TextureFormat format) {
			if(format != TextureFormat.Alpha8 && format != TextureFormat.RGB24 && format != TextureFormat.ARGB32 && format != TextureFormat.RGBA32) {
				if(format == TextureFormat.RGB24 || format == TextureFormat.DXT1 || format == TextureFormat.PVRTC_RGB2 || format == TextureFormat.PVRTC_RGB4 || format == TextureFormat.ETC_RGB4 || format == TextureFormat.ETC_RGB4 || format == TextureFormat.ETC2_RGBA8 || format == TextureFormat.ETC2_RGB || format == TextureFormat.ASTC_RGB_4x4 || format == TextureFormat.ASTC_RGB_5x5 || format == TextureFormat.ASTC_RGB_5x5 || format == TextureFormat.ASTC_RGB_5x5 || format == TextureFormat.ASTC_RGB_6x6 || format == TextureFormat.ASTC_RGB_10x10 || format == TextureFormat.ASTC_RGB_12x12) format = TextureFormat.RGB24;
				else format = TextureFormat.RGBA32;
			}
			return format;
		}		


/* ------------------------------------------------------------------------------------*/
/* ------------------ private functions -----------------------------------------------*/
/* ------------------------------------------------------------------------------------*/
		public static float GetHSBColorDistance(this Color color1, Color color2, float power) {
			Vector3 v1 = MakeHSB(color1);
			Vector3 v2 = MakeHSB(color2);
			float h = v1.x - v2.x;
			if(h > 0.5f) h -= 1f;
			if(h <= -0.5f) h += 1f;
			return Mathf.Pow(Mathf.Abs(h), power) + 
				(Mathf.Pow(Mathf.Abs(v1.y - v2.y), power) * 0.25f) +
				(Mathf.Pow(Mathf.Abs(v1.z - v2.z), power) * 0.5f);
		}
		public static float GetRGBColorDistance(this Color color1, Color color2, float power) {
			Vector3 v1 = new Vector3(color1.r, color1.g, color1.b);
			Vector3 v2 = new Vector3(color2.r, color2.g, color2.b);
			return Mathf.Pow(Mathf.Abs(v1.x - v2.x), power) + 
				Mathf.Pow(Mathf.Abs(v1.y - v2.y), power) +
				Mathf.Pow(Mathf.Abs(v1.z - v2.z), power);
		}
		public static float GetColorDistance(this Color color1, Color color2) {
			Vector3 v1 = new Vector3(color1.r, color1.g, color1.b);
			Vector3 v2 = new Vector3(color2.r, color2.g, color2.b);
			return Vector3.Distance(v1, v2);
		}
		public static float GrayScale(this Color color) {
			return (color.r + color.g + color.b) / 3f;
		}

		private static Color MakeColor(Vector3 hsb) {
		    // When saturation = 0, then r, g, b represent grey value (= brightness (z)).
		    float r = hsb.z;
		    float g = hsb.z;
		    float b = hsb.z;
		    while(hsb.x>=1f) hsb.x -= 1f;
		    while(hsb.x<0f) hsb.x += 1f;
			if(hsb.y > 0.0f) {  // saturation > 0
		        // Calc sector
		    	float secPos = (hsb.x * 360.0f) / 60.0f;
		    	int secNr = Mathf.FloorToInt(secPos);
		    	float secPortion = secPos - secNr;

		    	// Calc axes p, q and t
		    	float p = hsb.z * (1.0f - hsb.y);
		    	float q = hsb.z * (1.0f - (hsb.y * secPortion));
		    	float t = hsb.z * (1.0f - (hsb.y * (1.0f - secPortion)));

		    	// Calc rgb
		       	if(secNr == 1) {
		            r = q;
		            g = hsb.z;
		            b = p;
		        } else if(secNr == 2) {
		            r = p;
		            g = hsb.z;
		            b = t;
		        } else if(secNr == 3) {
		            r = p;
		            g = q;
		            b = hsb.z;
		        } else if(secNr == 4) {
		            r = t;
		            g = p;
		            b = hsb.z;
		        } else if(secNr == 5) {
		            r = hsb.z;
		            g = p;
		            b = q;
		        } else {
		            r = hsb.z;
		            g = t;
		            b = p;
		        }
		    }
		    return new Color(r, g, b);
		}

		private static Vector3 MakeHSB(Color c) {
		    float minValue = Mathf.Min(c.r, Mathf.Min(c.g, c.b));
		    float maxValue = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
		    float delta = maxValue - minValue;
		    float h = 0f;
		    float s = 0f;
		    float b = maxValue;

		    // Calc hue (in degrees between 0 and 360)
		    if(maxValue == c.r) {
		        if(c.g >= c.b) {
		            if(delta == 0f) h = 0f;
		            else h = 60f * (c.g - c.b) / delta;
		        } else if(c.g < c.b) {
		            h = 60f * (c.g - c.b) / delta + 360f;
		        }
		    } else if(maxValue == c.g) {
		        h = 60f * (c.b - c.r ) / delta + 120f;
		    } else if(maxValue == c.b) {
		        h = 60f * (c.r - c.g) / delta + 240f;
		    }

		    // Calc saturation (0 - 1)
		    if(maxValue == 0f) s = 0f;
		    else s = 1f - (minValue / maxValue);
		    return new Vector3(h / 360f, s, b);
		}


		private static Color[] ScaledPixels(Color[] originalPixels, int oldWidth, int oldHeight, int width, int height) {
			if(width<=0 || height<=0 || (width==oldWidth && height==oldHeight)) return originalPixels;
			float scaleX = (float)width / (float)oldWidth;
			float scaleY = (float)height / (float)oldHeight;
			Color[] newPixels = new Color[width * height];
			for (int y = 0; y < height; y++) {
				float originalY = y / scaleY;
				int originalYLow = Mathf.FloorToInt(originalY);
				int originalYHigh = Mathf.CeilToInt(originalY);
				for (int x = 0; x < width; x++) {
					float originalX = x / scaleX;
					int originalXLow = Mathf.FloorToInt(originalX);
					int originalXHigh = Mathf.CeilToInt(originalX);
					Color pixel = originalPixels[(originalYLow * oldWidth) + originalXLow] * 
							(1.0f - (originalY - originalYLow)) * 
							(1.0f - (originalX - originalXLow));
					if(originalXLow < originalXHigh && originalXHigh<oldWidth) {
						pixel = pixel + originalPixels[(originalYLow * oldWidth) + originalXHigh] * 
							(1.0f - (originalY - originalYLow)) * 
							(1.0f - (originalXHigh - originalX));
					}
					if(originalYLow < originalYHigh && originalYHigh<oldHeight) {
						pixel = pixel + originalPixels[(originalYHigh * oldWidth) + originalXLow] * 
							(1.0f - (originalYHigh - originalY)) * 
							(1.0f - (originalX - originalXLow));
						if(originalXLow < originalXHigh && originalXHigh<oldWidth) {
							pixel = pixel + originalPixels[(originalYHigh * oldWidth) + originalXHigh] * 
								(1.0f - (originalYHigh - originalY)) * 
								(1.0f - (originalXHigh - originalX));
						}
					}
					newPixels[(y * width) + x] = pixel;
				}
			}
			return newPixels;
		}
	}
}


