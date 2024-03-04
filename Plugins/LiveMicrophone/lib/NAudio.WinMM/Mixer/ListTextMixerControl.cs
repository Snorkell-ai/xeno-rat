// created on 13/12/2002 at 22:06
using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
	/// <summary>
	/// List text mixer control
	/// </summary>
	public class ListTextMixerControl : MixerControl 
	{
        internal ListTextMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels) 
		{
			this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
			this.nChannels = nChannels;
			this.mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
			
			GetControlDetails();

		}

		/// <summary>
		/// Sorts the given array using the bubble sort algorithm.
		/// </summary>
		/// <param name="arr">The array to be sorted.</param>
		/// <param name="n">The number of elements in the array.</param>
		protected override void GetDetails(IntPtr pDetails) 
		{
		}

		// TODO: provide a way of getting / setting data
	}
}
