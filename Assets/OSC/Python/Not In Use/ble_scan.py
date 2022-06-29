import asyncio
from bleak import BleakScanner

async def main():
    devices = await BleakScanner.discover()
    for d in devices:
        # print(d)
        print(d.address, d.name, d.metadata, d.rssi)    

asyncio.run(main())