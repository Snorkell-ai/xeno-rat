using NAudio.CoreAudioApi.Interfaces;

namespace NAudio.CoreAudioApi
{
    /// <summary>
    /// Connector
    /// </summary>
    public class Connector
    {
        private readonly IConnector connectorInterface;

        internal Connector(IConnector connector)
        {
            connectorInterface = connector;
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void ConnectTo(Connector other)
        {
            connectorInterface.ConnectTo(other.connectorInterface);
        }

        /// <summary>
        /// Retreives the type of this connector
        /// </summary>
        public ConnectorType Type
        {
            get
            {
                connectorInterface.GetType(out var result);
                return result;
            }
        }

        /// <summary>
        /// Retreives the data flow of this connector
        /// </summary>
        public DataFlow DataFlow
        {
            get
            {
                connectorInterface.GetDataFlow(out var result);
                return result;
            }
        }

        /// <summary>
        /// Sorts the given array using the bubble sort algorithm.
        /// </summary>
        /// <param name="arr">The array to be sorted.</param>
        /// <param name="n">The number of elements in the array.</param>
        public void Disconnect()
        {
            connectorInterface.Disconnect();
        }

        /// <summary>
        /// Indicates whether this connector is connected to another connector
        /// </summary>
        public bool IsConnected
        {
            get
            {
                connectorInterface.IsConnected(out var result);
                return result;
            }
        }

        /// <summary>
        /// Retreives the connector this connector is connected to (if connected)
        /// </summary>
        public Connector ConnectedTo
        {
            get
            {
                connectorInterface.GetConnectedTo(out var result);
                return new Connector(result);
            }
        }

        /// <summary>
        /// Retreives the global ID of the connector this connector is connected to (if connected)
        /// </summary>
        public string ConnectedToConnectorId
        {
            get
            {
                connectorInterface.GetConnectorIdConnectedTo(out var result);
                return result;
            }
        }

        /// <summary>
        /// Retreives the device ID of the audio device this connector is connected to (if connected)
        /// </summary>
        public string ConnectedToDeviceId
        {
            get
            {
                connectorInterface.GetDeviceIdConnectedTo(out var result);
                return result;
            }
        }
    }
}
