using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wasapi.CoreAudioApi.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NAudio.Wasapi.CoreAudioApi
{
    internal class ActivateAudioInterfaceCompletionHandler :
    IActivateAudioInterfaceCompletionHandler, IAgileObject
    {
        private Action<IAudioClient2> initializeAction;
        private TaskCompletionSource<IAudioClient2> tcs = new TaskCompletionSource<IAudioClient2>();

        public ActivateAudioInterfaceCompletionHandler(
            Action<IAudioClient2> initializeAction)
        {
            this.initializeAction = initializeAction;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void ActivateCompleted(IActivateAudioInterfaceAsyncOperation activateOperation)
        {
            // First get the activation results, and see if anything bad happened then
            activateOperation.GetActivateResult(out int hr, out object unk);
            if (hr != 0)
            {
                tcs.TrySetException(Marshal.GetExceptionForHR(hr, new IntPtr(-1)));
                return;
            }

            var pAudioClient = (IAudioClient2)unk;

            // Next try to call the client's (synchronous, blocking) initialization method.
            try
            {
                initializeAction(pAudioClient);
                tcs.SetResult(pAudioClient);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }


        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public TaskAwaiter<IAudioClient2> GetAwaiter()
        {
            return tcs.Task.GetAwaiter();
        }
    }
}
