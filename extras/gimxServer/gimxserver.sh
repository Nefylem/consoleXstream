#!/bin/bash

### BEGIN INIT INFO
# Provides:          gimxserver
# Required-Start:
# Required-Stop:
# Default-Start:     2 3 4 5
# Default-Stop:      0 1 6
# Short-Description: start consoleXstream gimxServer
### END INIT INFO

# Replace this cmd with the one appropiate for your installation
CMD="gimx --src 192.168.32.45:51914 --nograb --type Sixaxis --hci 0 --bdaddr 00:13:63:3A:F3:02"

case "$1" in
    start)
        gimxServer "${CMD}"
        ;;

    stop)
        pkill -9 gimxServer
        ;;

    restart)
        $0 stop
        sleep 1
        $0 start
        ;;

    *)
        echo "Usage: $0 {start | stop | restart}"
        exit 1
        ;;
esac

exit 0