/***************************************************************************
 *            gimxServer.c
 *
 *  Simple daemon which will listen for a magic packet and run a cmd
 *  This source is part of consoleXstream extras
 *  https://github.com/Nefylem/consoleXstream
 *
 *  Fri Jul 10 23:24:25 CEST 2015
 *  Copyright  2015  Jaime Peñalba Estebanez
 *  jpenalbae@gmail.com
 ****************************************************************************/

/*
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <errno.h>
#include <fcntl.h>

#include <sys/types.h>
#include <sys/stat.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>


static void daemonize(void)  
{  
    pid_t pid, sid;  
    int fd;   
  
    pid = fork();  
    if (pid < 0)    
        exit(EXIT_FAILURE);  
    if (pid > 0)    
        exit(EXIT_SUCCESS); /*Killing the Parent Process*/  
  
  
    /* Create a new SID for the child process */  
    sid = setsid();  
    if (sid < 0)    
        exit(EXIT_FAILURE);  
  
    fd = open("/dev/null", O_RDWR, 0);  
    if (fd != -1)  
    {  
        dup2 (fd, STDIN_FILENO);  
        dup2 (fd, STDOUT_FILENO);  
        dup2 (fd, STDERR_FILENO);  
  
        if (fd > 2)  
            close (fd);
    }  
}  


/*
 * Closing the socket and opening it for every read is a lazy hack
 * in order to avoid getting a full socket buffer after system() finishes
 *
 * Do not attemp this at home.
 */
void udp_listen(char *cmd)
{
    int sock, bytes;
    char buffer[1024];
    struct sockaddr_in servaddr;

    if ((sock = socket(AF_INET, SOCK_DGRAM, 0)) < 0) {
        perror("Error creating socket");
        return;
    }

    memset(&servaddr, 0, sizeof(servaddr));
    servaddr.sin_family = AF_INET;
    servaddr.sin_port = htons(51913);
    servaddr.sin_addr.s_addr = htonl(INADDR_ANY);

    if (bind(sock, (struct sockaddr *) &servaddr, sizeof(servaddr)) < 0) {
        perror("Error while binding");
        return;
    }

read_packet:
    bzero(&buffer, 1024);
    bytes = read(sock, &buffer, 1024);

    if (bytes == 4 && (*(unsigned int *)&buffer[0] == 0xEFBEADDE)) {
        printf("[+] Wakeup/KeepAlive received\n");
        close(sock);
        system(cmd);
        return;
    }

    goto read_packet;
}

void print_help(char *cmd)
{
    printf("Usage: %s [-d] \"cmd_to_execute\"\n", cmd);
    printf("   -d: Show debug messages (Do not detach)\n");
    printf("   -h: Show this help\n\n");
}

int main(int argc, char *argv[])
{
    
    int c;
    int debug = 0;

    while ((c = getopt(argc, argv, "dh")) != -1) {
        switch (c) {
            case 'd':
                debug = 1;
                break;
            default:
                print_help(argv[0]);
                return 0;
                break;
        }
    }

    if (optind >= argc) {
        fprintf(stderr, "ERROR: Expected argument after options\n");
        print_help(argv[0]);
        return 1;
    }

    if (!debug)
        daemonize();

    for (;;) {
        printf("[+] Listening for magick packet\n");
        udp_listen(argv[optind]);
    }

    return 0;
}