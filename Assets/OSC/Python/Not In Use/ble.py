# """
# Connect by BLEDevice
# """

import asyncio
import platform
import sys
import logging

from bleak import BleakClient, BleakScanner
from bleak.exc import BleakError

BLE_MAC_ADDR    = "D8:A0:1D:59:BA:DE"  # M5Stick-C
BLE_IDENTIFIER  = "B6F47DBC-FF10-71C6-DF22-8A7042F8D035" # M5Stick-C
CHAR_UUID       = "BEB5483E-36E1-4688-B7F5-EA07361B26A8"
SERVICE_UUID    = "4FAFC201-1FB5-459E-8FCC-C5C9C331914B"  

ADDRESS = (
    BLE_MAC_ADDR
    if platform.system() != "Darwin"
    else BLE_IDENTIFIER
)


async def main(ble_address: str):
    log = logging.getLogger(__name__)
    device = await BleakScanner.find_device_by_address(ble_address, timeout=20.0)
    if not device:
        raise BleakError(f"A device with address {ble_address} could not be found.")
    async with BleakClient(device) as client:
        svcs = await client.get_services()
        print("Services:")
        for service in svcs:
            print(service)

    # async with BleakClient(address, loop=loop) as client:
        x = await client.is_connected()
        log.info("BLE Connected: {0}".format(x))

        # await client.start_notify(SENSOR_UUID, callback)
        print("Start notification")

        while await client.is_connected():
            # read sensor value from BLE
            # value = await client.read_gatt_char(SENSOR_UUID)
            # value_converted = int.from_bytes(bytes(value), byteorder="big")
            # print("BLE sensor: " + str(value_converted))
            # print("I/O Data Post-Write Value: {0}".format(value))

            # global receive_motor
            # global motor_state

            # send 3 bytes: [command] [dutyCycle (-100% to 100%)] [time in e-2]
            command = bytes("s", "ascii")
            dutyCycle = bytes(100)
            time = bytes(120)

            # commandByte = str.encode(command)

            messageOut = [command, dutyCycle, time]

            user_input = str(input("> send BLE: "))
            # messages need to be in bytes, 03FF -> OK, 3FF -> not OK
            # b = bytearray.fromhex("03")
            # b = bytes("a",'ascii')
            b = bytearray(user_input.encode())
            # b = command

            print(b)
            await client.write_gatt_char(CHAR_UUID, b, response=True) # response=True needed for mac M1
            print("BLE write: " + str(b))
            # receive_motor = False

        # await client.stop_notify(SENSOR_UUID)
        print("Stop notification")
        time.sleep(0.5)


if __name__ == "__main__":
    asyncio.run(main(sys.argv[1] if len(sys.argv) == 2 else ADDRESS))
