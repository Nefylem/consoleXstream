using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        // Sockets & endpoints
        private Socket gimxSocket;
        private IPEndPoint wakeEndPoint;
        private IPEndPoint controllerEndPoint;



        public GimxRemote(BaseClass baseClass) { _class = baseClass; }

        public void initGimxRemote()
        {
            IPAddress serverAddr;

            gimxMap = new GimxMaps();

            // TODO Get addresses & ports from the config file
            gimxSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverAddr = IPAddress.Parse("192.168.11.251");
            wakeEndPoint = new IPEndPoint(serverAddr, 51913);
            controllerEndPoint = new IPEndPoint(serverAddr, 51914);

            // TODO send wakeup packet to the control daemon
            // Must code a C daemon to start & stop the gimx binary on the remote box
        }

        public void CheckControllerInput()
        {
            //Update gamepad status
            _controls = GamePad.GetState(PlayerIndex.One);

            // TODO Generate different message buffer based on target controller
            // Right now only PS3 controller target is supported
            gimxMap.mapToPS3(_controls);


            gimxSocket.SendTo(gimxMap.buffer, controllerEndPoint);
        }
    }
}
