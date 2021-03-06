﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VVVV.Nodes.EmguCV
{
	public class CVImageOutput : IDisposable
	{
		CVImageLink FLink = new CVImageLink();
		public CVImageLink Link { get { return FLink; } }

		public CVImage Image = new CVImage();

		/// <summary>
		/// Sends the internal image
		/// </summary>
		public void Send()
		{
			Link.Send(Image);
		}

		/// <summary>
		/// Sends an image to the link, ignoring the internal buffer
		/// </summary>
		public void Send(CVImage image)
		{
			Link.Send(image);
		}

		public CVImageOutput()
		{
			// we shouldn't put an image here to start with
			// the action of assigning an image to here is important
		}

		public IntPtr Data
		{
			get
			{
				return Image.Data;
			}
		}

		public IntPtr CvMat
		{
			get
			{
				return Image.CvMat;
			}
		}

		public void Dispose()
		{
			FLink.Dispose();
		}
	}
}
