# """
# Connect by BLEDevice
# """

import asyncio
import platform
import sys
import logging
import threading
import time

from bleak import BleakClient, BleakScanner
from bleak.exc import BleakError

from pythonosc import osc_server
from pythonosc import udp_client
from pythonosc import dispatcher

# OSC server address
# ================== #
ip = "127.0.0.1"
port = 5007

# BLE settings
# ======================================================= #
BLE_MAC_ADDR    = "82:c5:df:f8:20:42"  # "33:c6:64:e4:fa:3f" # XIAO BLE Sense
BLE_IDENTIFIER  = "72EC3D0E-B93D-2B51-A0B6-308D8DC0931C" # XIAO BLE Sense
GET_CHAR_UUID   = "BEB5483E-36E1-4688-B7F5-EA07361B26A8"
SEND_CHAR_UUID  = "8628FE7C-A4E9-4056-91BD-FD6AA7817E39"
SERVICE_UUID    = "4FAFC201-1FB5-459E-8FCC-C5C9C331914B"  
# ======================================================= #

# OSC server
# ======================================================= #
OSC_IP = "0.0.0.0"  # local device's OSC_IP on LAN
OSC_PORT = 5006
# ======================================================= #

# debug
ble = True

# global variable
client = udp_client.SimpleUDPClient(ip, port)
dispatcher = dispatcher.Dispatcher()
send_barrays = []

# Handler function for sending recieved BLE data via OSC
def send_handler(address, args):
    jumpState = args.decode()
    print("BLE receives: " + jumpState)
    client.send_message("/command", "Jump Status = " + jumpState)

async def main(ble_address: str):
    print("connected to OSC server at "+ip+":"+str(port))
    log = logging.getLogger(__name__)
    device = await BleakScanner.find_device_by_address(ble_address, timeout=20.0)
    if not device:
        raise BleakError(f"A device with address {ble_address} could not be found.")
    async with BleakClient(device) as client:
        # Handler function for sending recieved OSC data from the "/command" channel via BLE
        def command_handler(address, *args):
            command = str(args[0])
            command = command[1:-1] # Removes angle brackets surrounding the command
            # Writing to the characteristic puts this command string (minus the final null character) at the beginning of 
            # a 32 character string buffer which may include unnecessary information from previous writes. Because of this, 
            # it is necessary to add an additional null termination character to the command string so the extra null 
            # character can terminate the 32 character buffer at the correct place after the characteristic is written.
            command += "\0" 
            print("OSC/command receives: " + str(command))
            b = bytearray(command.encode())
            send_barrays.append(b) # Add to the list of commands that need to be sent

        # Sets up the handler for recieving OSC information from the "/command" channel
        dispatcher.map("/command", command_handler)

        svcs = await client.get_services()
        print("Services:")
        for service in svcs:
            print(service)

        x = client.is_connected
        log.info("BLE Connected: {0}".format(x))
        
        # BLE notifications
        print("Start notification")
        await client.start_notify(SEND_CHAR_UUID, send_handler)
        while client.is_connected:
            # Send all recieved commands
            while len(send_barrays) != 0:
                b = send_barrays.pop(0)
                await client.write_gatt_char(GET_CHAR_UUID, b, response=True)
                print("Written: " + str(b))
            await asyncio.sleep(0) # Interrupts the current await function to allow the send handler to run
        print("Stop notification")
        await client.stop_notify(SEND_CHAR_UUID)
        print("Exited")


if __name__ == "__main__":
    # start OSC server in a thread to prevent conflicting with BLE
    server = osc_server.ThreadingOSCUDPServer((OSC_IP, OSC_PORT), dispatcher)
    server_thread = threading.Thread(target=server.serve_forever)
    server_thread.daemon = True
    server_thread.start()
    print("open an OSC server at " + OSC_IP + ":" + str(OSC_PORT))

    address = (
        # for windows we need mac address
        BLE_MAC_ADDR
        if platform.system() != "Darwin"
        # for mac we need identifier
        else BLE_IDENTIFIER
    )

    if ble:
        # start BLE, this will run forever
        loop = asyncio.get_event_loop()
        loop.run_until_complete(main(address))
