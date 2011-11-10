﻿#region usings
using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D9;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.PluginInterfaces.V2.EX9;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using VVVV.Utils.SlimDX;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

#endregion usings

//here you can change the vertex type
using VertexType = VVVV.Utils.SlimDX.TexturedVertex;
using System.Collections.Generic;
using Emgu.CV.CvEnum;
using System.Drawing;

namespace VVVV.Nodes.EmguCV
{
	#region PluginInfo
	[PluginInfo(Name = "AsTexture",
				Category = "EmguCV",
				Version = "",
				Help = "Converts IPLImage to Texture",
				Tags = "")]
	#endregion PluginInfo
	public class AsTextureNode : DXTextureOutPluginBase, IPluginEvaluate
	{
		#region fields & pins
		[Input("Image")]
		ISpread<CVImageLink> FPinInImage;

		[Import]
		ILogger FLogger;

		private Spread<AsTextureImageInstance> FImageInstances = new Spread<AsTextureImageInstance>(0);

		#endregion fields & pins

		// import host and hand it to base constructor
		[ImportingConstructor()]
		public AsTextureNode(IPluginHost host)
			: base(host)
		{
		}

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			CheckSpread(SpreadMax);

			Update();
		}

		private void CheckSpread(int count)
		{
			bool needsInit = false;

			if (FPinInImage[0] == null)
			{
				FImageInstances.SliceCount = 0;
				SetSliceCount(0);
				Reinitialize();
				return;
			}

			if (FImageInstances.SliceCount != count)
			{
				FImageInstances.SliceCount = count;
				needsInit = true;
			}

			for (int i = 0; i < count; i++)
			{
				if (FPinInImage[i] == null)
				{
					FImageInstances[i] = null;
					continue;
				}
				
				if (FImageInstances[i] == null)
				{
					FImageInstances[i] = new AsTextureImageInstance();
					needsInit = true;
				}

				if (FImageInstances[i].ImageAttributesChanged)
					needsInit = true;

				FImageInstances[i].Initialise(FPinInImage[i]);
			}

			SetSliceCount(count);

			if (needsInit)
				Reinitialize();
		}

		//this method gets called, when Reinitialize() was called in evaluate,
		//or a graphics device asks for its data
		protected override Texture CreateTexture(int Slice, Device device)
		{
			if (FImageInstances.SliceCount > Slice)
				if (FImageInstances[Slice] != null) // we do have a connected input for this slice
					return FImageInstances[Slice].CreateTexture(device);
			
			return TextureUtils.CreateTexture(device, 1, 1);
				
		}

		//this method gets called, when Update() was called in evaluate,
		//or a graphics device asks for its texture, here you fill the texture with the actual data
		//this is called for each renderer, careful here with multiscreen setups, in that case
		//calculate the pixels in evaluate and just copy the data to the device texture here
		protected unsafe override void UpdateTexture(int Slice, Texture texture)
		{
			if (FImageInstances.SliceCount < Slice)
				return;

			FImageInstances[Slice].UpdateTexture(texture);
		}
	}
}
