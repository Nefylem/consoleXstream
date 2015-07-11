using consoleXstream.Input;
using System.Net.Sockets;
using System.Net;


namespace consoleXstream.Output.Gimx
{
    public class GimxRemote
    {
        private BaseClass _class;
        private Input.GamePadState _controls;
        private GimxMaps gimxMap;
        private byte[] keepAlivePacket;
        
        // Sockets & endpoints
        private Socket gimxSocket;
        private IPEndPoint wakeEndPoint;
        private IPEndPoint controllerEndPoint;



        public GimxRemote(BaseClass baseClass) { _class = baseClass; }

        public void initGimxRemote()
        {
            IPAddress serverAddr;

            gimxMap = new GimxMaps();
            keepAlivePacket = new byte[4];

            // Setup Gimx Server Address
            _class.System.Debug("Gimx.log", "[3] Using GimxRemote server: " + _class.System.GimxAddress);
            _class.System.Debug("Gimx.log", "[3] GimxRemote KeepAlive: " + _class.System.GimxKeepAlive);
            gimxSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverAddr = IPAddress.Parse(_class.System.GimxAddress);
            wakeEndPoint = new IPEndPoint(serverAddr, 51913);
            controllerEndPoint = new IPEndPoint(serverAddr, 51914);

            // Setup the wakeup/keepalive packet & send it
            keepAlivePacket[0] = 0xDE;
            keepAlivePacket[1] = 0xAD;
            keepAlivePacket[2] = 0xBE;
            keepAlivePacket[3] = 0xEF;
            gimxSocket.SendTo(keepAlivePacket, wakeEndPoint);

        }

        public void CheckControllerInput()
        {
            //Update gamepad status
            _controls = GamePad.GetState(PlayerIndex.One);

            // TODO Generate different message buffer based on target controller
            // Right now only PS3 controller target is supported
            gimxMap.mapToPS3(_controls);


            gimxSocket.SendTo(gimxMap.buffer, controllerEndPoint);

            
            // Check for reconnect cmd
            if (_controls.Buttons.Back && _controls.Buttons.X)
                gimxSocket.SendTo(keepAlivePacket, wakeEndPoint);
        }
    }
}
