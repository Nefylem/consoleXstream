# gimxServer

gimxServer is a small UDP daemon which listens for magic packets in order to run gimx.

### Installation

To install gimxServer you must build and copy some files:
```
$ gcc gimxServer.c -o gimxServer
# cp gimxServer /usr/bin
# cp gimxserver.sh /etc/init.d
```

Under debian systems you can also configure the init script to automatically start on boot
```
# update-rc.d gimxserver.sh defaults
```

### Usage

You can manually launch gimxServer

```
# ./gimxServer -d "gimx --src 192.168.32.45:51914 --nograb --type Sixaxis --hci 0 --bdaddr 00:13:63:3A:F3:02"
```

Or you can run it as a daemon using the init script included.
You **must** change the line contaning the `CMD` variable with the propper gimx command for your setup.

```
# /etc/init.d/gimxserver.sh start
```