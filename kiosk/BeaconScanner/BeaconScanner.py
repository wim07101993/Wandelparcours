import time

from beacontools import *
from BeaconScanner.BeaconTag import BeaconTag
from beacontools.scanner import BeaconScanner as BLEscanner
class BeaconScanner:


    def __init__(self):
        self.__list=[]



    def scan(self):
        self.__list  = []
        scanner = BLEscanner(self.callback)
        scanner.start()

        time.sleep(0.5)
        scanner.stop()


        return self.__list

    def callback(self,btn_addr, rssi, packet, additional_info):
        if self.checkTagExistence(packet._minor) == False:
            self.__list.append(BeaconTag(packet._minor,rssi))


    def checkTagExistence(self,minor):
        for tag in self.__list:
            if isinstance(tag,BeaconTag):
                if(tag.GetMac()==minor):
                    return True
        return False

