from socket import *
import binascii
port = 56700
s = socket(AF_INET, SOCK_DGRAM)
s.bind(('', port))
b = socket(AF_INET, SOCK_DGRAM)
b.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
b.setsockopt(SOL_SOCKET, SO_BROADCAST, 1)
while True:
    m = s.recvfrom(1024)
    print(str(binascii.hexlify(m[0])).split('\'')[1], m[1])