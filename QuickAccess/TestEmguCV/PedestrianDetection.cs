using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.GPU;
using Emgu.CV.Structure;

namespace QuickAccess.TestEmguCV
{
	public static class PedestrianDetection
	{
		public static bool DetectInImage(string ImagePath, out Image<Bgr, Byte> imageWithRectangles, out bool UsedGpu, out long ElapsedMilliseconds, out string ErrorMessage)
		{
			imageWithRectangles = null;
			UsedGpu = false;
			ElapsedMilliseconds = 0;
			try
			{
				imageWithRectangles = new Image<Bgr, byte>(ImagePath);

				Stopwatch watch;
				Rectangle[] regions;

				UsedGpu = GpuInvoke.HasCuda;
				//check if there is a compatible GPU to run pedestrian detection
				if (GpuInvoke.HasCuda)
				{ 
					//this is the GPU version
					using (GpuHOGDescriptor des = new GpuHOGDescriptor())
					{
						des.SetSVMDetector(GpuHOGDescriptor.GetDefaultPeopleDetector());

						watch = Stopwatch.StartNew();
						using (GpuImage<Bgr, Byte> gpuImg = new GpuImage<Bgr, byte>(imageWithRectangles))
						using (GpuImage<Bgra, Byte> gpuBgra = gpuImg.Convert<Bgra, Byte>())
						{
							regions = des.DetectMultiScale(gpuBgra);
						}
					}
				}
				else
				{  //this is the CPU version
					using (HOGDescriptor des = new HOGDescriptor())
					{
						des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

						watch = Stopwatch.StartNew();
						regions = des.DetectMultiScale(imageWithRectangles);
					}
				}
				watch.Stop();

				ElapsedMilliseconds = watch.ElapsedMilliseconds;

				foreach (Rectangle pedestrain in regions)
				{
					imageWithRectangles.Draw(pedestrain, new Bgr(Color.Red), 1);
				}
				ErrorMessage = null;
				return true;
			}
			catch (Exception exc)
			{
				//UserMessages.ShowErrorMessage("Could not do pedestrian detection: " + exc.Message);
				ErrorMessage = exc.Message;
				return false;
			}
		}

		/// <summary>
		/// Check if both the managed and unmanaged code are compiled for the same architecture
		/// </summary>
		/// <returns>Returns true if both the managed and unmanaged code are compiled for the same architecture</returns>
		public static bool IsPlaformCompatable()
		{
			int clrBitness = Marshal.SizeOf(typeof(IntPtr)) * 8;
			if (clrBitness != CvInvoke.UnmanagedCodeBitness)
			{
				MessageBox.Show(String.Format("Platform mismatched: CLR is {0} bit, C++ code is {1} bit."
				   + " Please consider recompiling the executable with the same platform target as C++ code.",
				   clrBitness, CvInvoke.UnmanagedCodeBitness));
				return false;
			}
			return true;
		}
	}
}
