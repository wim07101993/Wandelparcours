from Interfaces.ScannerInterface import ScannerInterface
from InjectionHandler import InjectionHandler
from MockUp.TagMock import TagMock
import random
class ScannerMock(ScannerInterface):

    def scan(self):
        list = dict()
        list["testmac"] = TagMock("testmac",random.randint(0,50))
        list["testmac1"] = TagMock("testmac1", random.randint(0,50))
        list["testmac2"] = TagMock("testmac2", random.randint(0,50))
        return list
