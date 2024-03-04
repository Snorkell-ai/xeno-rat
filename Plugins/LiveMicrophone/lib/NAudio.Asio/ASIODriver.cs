using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace NAudio.Wave.Asio
{
    /// <summary>
    /// Main AsioDriver Class. To use this class, you need to query first the GetAsioDriverNames() and
    /// then use the GetAsioDriverByName to instantiate the correct AsioDriver.
    /// This is the first AsioDriver binding fully implemented in C#!
    /// 
    /// Contributor: Alexandre Mutel - email: alexandre_mutel at yahoo.fr
    /// </summary>
    public class AsioDriver
    {
        private IntPtr pAsioComObject;
        private IntPtr pinnedcallbacks;
        private AsioDriverVTable asioDriverVTable;

        private AsioDriver()
        {
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static string[] GetAsioDriverNames()
        {
            var regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ASIO");
            var names = new string[0];
            if (regKey != null)
            {
                names = regKey.GetSubKeyNames();
                regKey.Close();
            }
            return names;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static AsioDriver GetAsioDriverByName(String name)
        {
            var regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ASIO\\" + name);
            if (regKey == null)
            {
                throw new ArgumentException($"Driver Name {name} doesn't exist");
            }
            var guid = regKey.GetValue("CLSID").ToString();
            return GetAsioDriverByGuid(new Guid(guid));
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public static AsioDriver GetAsioDriverByGuid(Guid guid)
        {
            var driver = new AsioDriver();
            driver.InitFromGuid(guid);
            return driver;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool Init(IntPtr sysHandle)
        {
            int ret = asioDriverVTable.init(pAsioComObject, sysHandle);
            return ret == 1;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public string GetDriverName() 
        {
            var name = new StringBuilder(256);
            asioDriverVTable.getDriverName(pAsioComObject, name);
            return name.ToString();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public int GetDriverVersion() {
            return asioDriverVTable.getDriverVersion(pAsioComObject);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public string GetErrorMessage()
        {
            var errorMessage = new StringBuilder(256);
            asioDriverVTable.getErrorMessage(pAsioComObject, errorMessage);
            return errorMessage.ToString();
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Start()
        {
            HandleException(asioDriverVTable.start(pAsioComObject),"start");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public AsioError Stop()
        {
            return asioDriverVTable.stop(pAsioComObject);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void GetChannels(out int numInputChannels, out int numOutputChannels)
        {
            HandleException(asioDriverVTable.getChannels(pAsioComObject, out numInputChannels, out numOutputChannels), "getChannels");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public AsioError GetLatencies(out int inputLatency, out int outputLatency)
        {
            return asioDriverVTable.getLatencies(pAsioComObject, out inputLatency, out outputLatency);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void GetBufferSize(out int minSize, out int maxSize, out int preferredSize, out int granularity)
        {
            HandleException(asioDriverVTable.getBufferSize(pAsioComObject, out minSize, out maxSize, out preferredSize, out granularity), "getBufferSize");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public bool CanSampleRate(double sampleRate)
        {
            var error = asioDriverVTable.canSampleRate(pAsioComObject, sampleRate);
            if (error == AsioError.ASE_NoClock)
            {
                return false;
            } 
            if ( error == AsioError.ASE_OK )
            {
                return true;
            }
            HandleException(error, "canSampleRate");
            return false;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public double GetSampleRate()
        {
            double sampleRate;
            HandleException(asioDriverVTable.getSampleRate(pAsioComObject, out sampleRate), "getSampleRate");
            return sampleRate;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetSampleRate(double sampleRate)
        {
            HandleException(asioDriverVTable.setSampleRate(pAsioComObject, sampleRate), "setSampleRate");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void GetClockSources(out long clocks, int numSources)
        {
            HandleException(asioDriverVTable.getClockSources(pAsioComObject, out clocks,numSources), "getClockSources");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void SetClockSource(int reference)
        {
            HandleException(asioDriverVTable.setClockSource(pAsioComObject, reference), "setClockSources");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void GetSamplePosition(out long samplePos, ref Asio64Bit timeStamp)
        {
            HandleException(asioDriverVTable.getSamplePosition(pAsioComObject, out samplePos, ref timeStamp), "getSamplePosition");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public AsioChannelInfo GetChannelInfo(int channelNumber, bool trueForInputInfo)
        {
            var info = new AsioChannelInfo {channel = channelNumber, isInput = trueForInputInfo};
            HandleException(asioDriverVTable.getChannelInfo(pAsioComObject, ref info), "getChannelInfo");
            return info;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void CreateBuffers(IntPtr bufferInfos, int numChannels, int bufferSize, ref AsioCallbacks callbacks)
        {
            // next two lines suggested by droidi on codeplex issue tracker
            pinnedcallbacks = Marshal.AllocHGlobal(Marshal.SizeOf(callbacks));
            Marshal.StructureToPtr(callbacks, pinnedcallbacks, false);
            HandleException(asioDriverVTable.createBuffers(pAsioComObject, bufferInfos, numChannels, bufferSize, pinnedcallbacks), "createBuffers");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public AsioError DisposeBuffers()
        {
            AsioError result = asioDriverVTable.disposeBuffers(pAsioComObject);
            Marshal.FreeHGlobal(pinnedcallbacks);
            return result;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void ControlPanel()
        {
            HandleException(asioDriverVTable.controlPanel(pAsioComObject), "controlPanel");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Future(int selector, IntPtr opt)
        {
            HandleException(asioDriverVTable.future(pAsioComObject, selector, opt), "future");
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public AsioError OutputReady()
        {
            return asioDriverVTable.outputReady(pAsioComObject);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void ReleaseComAsioDriver()
        {
            Marshal.Release(pAsioComObject);
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void HandleException(AsioError error, string methodName)
        {
            if (error != AsioError.ASE_OK && error != AsioError.ASE_SUCCESS)
            {
                var asioException = new AsioException(
                    $"Error code [{AsioException.getErrorName(error)}] while calling ASIO method <{methodName}>, {this.GetErrorMessage()}");
                asioException.Error = error;
                throw asioException;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        private void InitFromGuid(Guid asioGuid)
        {
            const uint CLSCTX_INPROC_SERVER = 1;
            // Start to query the virtual table a index 3 (init method of AsioDriver)
            const int INDEX_VTABLE_FIRST_METHOD = 3;

            // Pointer to the ASIO object
            // USE CoCreateInstance instead of builtin COM-Class instantiation,
            // because the AsioDriver expect to have the ASIOGuid used for both COM Object and COM interface
            // The CoCreateInstance is working only in STAThread mode.
            int hresult = CoCreateInstance(ref asioGuid, IntPtr.Zero, CLSCTX_INPROC_SERVER, ref asioGuid, out pAsioComObject);
            if ( hresult != 0 )
            {
                throw new COMException("Unable to instantiate ASIO. Check if STAThread is set",hresult);
            }

            // The first pointer at the adress of the ASIO Com Object is a pointer to the
            // C++ Virtual table of the object.
            // Gets a pointer to VTable.
            IntPtr pVtable = Marshal.ReadIntPtr(pAsioComObject);

            // Instantiate our Virtual table mapping
            asioDriverVTable = new AsioDriverVTable();

            // This loop is going to retrieve the pointer from the C++ VirtualTable
            // and attach an internal delegate in order to call the method on the COM Object.
            FieldInfo[] fieldInfos =  typeof (AsioDriverVTable).GetFields();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                // Read the method pointer from the VTable
                IntPtr pPointerToMethodInVTable = Marshal.ReadIntPtr(pVtable, (i + INDEX_VTABLE_FIRST_METHOD) * IntPtr.Size);
                // Instantiate a delegate
                object methodDelegate = Marshal.GetDelegateForFunctionPointer(pPointerToMethodInVTable, fieldInfo.FieldType);
                // Store the delegate in our C# VTable
                fieldInfo.SetValue(asioDriverVTable, methodDelegate);
            }
        }

        /// <summary>
        /// Internal VTable structure to store all the delegates to the C++ COM method.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private class AsioDriverVTable
        {
            //3  virtual ASIOBool init(void *sysHandle) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate int ASIOInit(IntPtr _pUnknown, IntPtr sysHandle);
            public ASIOInit init = null;
            //4  virtual void getDriverName(char *name) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate void ASIOgetDriverName(IntPtr _pUnknown, StringBuilder name);
            public ASIOgetDriverName getDriverName = null;
            //5  virtual long getDriverVersion() = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate int ASIOgetDriverVersion(IntPtr _pUnknown);
            public ASIOgetDriverVersion getDriverVersion = null;
            //6  virtual void getErrorMessage(char *string) = 0;	
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate void ASIOgetErrorMessage(IntPtr _pUnknown, StringBuilder errorMessage);
            public ASIOgetErrorMessage getErrorMessage = null;
            //7  virtual ASIOError start() = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOstart(IntPtr _pUnknown);
            public ASIOstart start = null;
            //8  virtual ASIOError stop() = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOstop(IntPtr _pUnknown);
            public ASIOstop stop = null;
            //9  virtual ASIOError getChannels(long *numInputChannels, long *numOutputChannels) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetChannels(IntPtr _pUnknown, out int numInputChannels, out int numOutputChannels);
            public ASIOgetChannels getChannels = null;
            //10  virtual ASIOError getLatencies(long *inputLatency, long *outputLatency) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetLatencies(IntPtr _pUnknown, out int inputLatency, out int outputLatency);
            public ASIOgetLatencies getLatencies = null;
            //11 virtual ASIOError getBufferSize(long *minSize, long *maxSize, long *preferredSize, long *granularity) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetBufferSize(IntPtr _pUnknown, out int minSize, out int maxSize, out int preferredSize, out int granularity);
            public ASIOgetBufferSize getBufferSize = null;
            //12 virtual ASIOError canSampleRate(ASIOSampleRate sampleRate) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOcanSampleRate(IntPtr _pUnknown, double sampleRate);
            public ASIOcanSampleRate canSampleRate = null;
            //13 virtual ASIOError getSampleRate(ASIOSampleRate *sampleRate) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetSampleRate(IntPtr _pUnknown, out double sampleRate);
            public ASIOgetSampleRate getSampleRate = null;
            //14 virtual ASIOError setSampleRate(ASIOSampleRate sampleRate) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOsetSampleRate(IntPtr _pUnknown, double sampleRate);
            public ASIOsetSampleRate setSampleRate = null;
            //15 virtual ASIOError getClockSources(ASIOClockSource *clocks, long *numSources) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetClockSources(IntPtr _pUnknown, out long clocks, int numSources);
            public ASIOgetClockSources getClockSources = null;
            //16 virtual ASIOError setClockSource(long reference) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOsetClockSource(IntPtr _pUnknown, int reference);
            public ASIOsetClockSource setClockSource = null;
            //17 virtual ASIOError getSamplePosition(ASIOSamples *sPos, ASIOTimeStamp *tStamp) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetSamplePosition(IntPtr _pUnknown, out long samplePos, ref Asio64Bit timeStamp);
            public ASIOgetSamplePosition getSamplePosition = null;
            //18 virtual ASIOError getChannelInfo(ASIOChannelInfo *info) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOgetChannelInfo(IntPtr _pUnknown, ref AsioChannelInfo info);
            public ASIOgetChannelInfo getChannelInfo = null;
            //19 virtual ASIOError createBuffers(ASIOBufferInfo *bufferInfos, long numChannels, long bufferSize, ASIOCallbacks *callbacks) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            //            public delegate ASIOError ASIOcreateBuffers(IntPtr _pUnknown, ref ASIOBufferInfo[] bufferInfos, int numChannels, int bufferSize, ref ASIOCallbacks callbacks);
            public delegate AsioError ASIOcreateBuffers(IntPtr _pUnknown, IntPtr bufferInfos, int numChannels, int bufferSize, IntPtr callbacks);
            public ASIOcreateBuffers createBuffers = null;
            //20 virtual ASIOError disposeBuffers() = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOdisposeBuffers(IntPtr _pUnknown);
            public ASIOdisposeBuffers disposeBuffers = null;
            //21 virtual ASIOError controlPanel() = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOcontrolPanel(IntPtr _pUnknown);
            public ASIOcontrolPanel controlPanel = null;
            //22 virtual ASIOError future(long selector,void *opt) = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOfuture(IntPtr _pUnknown, int selector, IntPtr opt);
            public ASIOfuture future = null;
            //23 virtual ASIOError outputReady() = 0;
            [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
            public delegate AsioError ASIOoutputReady(IntPtr _pUnknown);
            public ASIOoutputReady outputReady = null;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        [DllImport("ole32.Dll")]
        private static extern int CoCreateInstance(ref Guid clsid,
           IntPtr inner,
           uint context,
           ref Guid uuid,
           out IntPtr rReturnedComObject);
    }
}
