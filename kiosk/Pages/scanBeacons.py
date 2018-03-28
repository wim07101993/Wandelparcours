import random
import sys
import json
from beacontools import *
from beacontools.scanner import BeaconScanner as BLEscanner
import time



class Scanner():
    def __init__(self):
        self.list = []
        scanner = BLEscanner(self.callback)
        scanner.start()
        time.sleep(0.5)
        scanner.stop()
    def checkTagExistence(self,minor):
        for tag in self.list:
            if tag["id"]==minor:
               return True
        return False


    def callback(self,btn_addr,rssi,packet,additional_info):
        if self.checkTagExistence(packet._minor)==False:
            self.list.append(dict(id=packet._minor, rssi=rssi))
scanner=Scanner()
print(json.dumps(scanner.list))
sys.stdout.flush()
'''
list = []
list.append(dict(id="1", rssi=random.randint(0, 50)))
list.append(dict(id="2", rssi=random.randint(0, 50)))
list.append(dict(id="3", rssi=random.randint(0, 50)))
list.append(dict(id="4", rssi=random.randint(0, 50)))
print(json.dumps(list))
sys.stdout.flush()'''