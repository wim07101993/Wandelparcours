from Interfaces.ScannerInterface import ScannerInterface
from InjectionHandler import InjectionHandler
from MockUp.TagMock import TagMock
import random
class ScannerMock(ScannerInterface):

    def scan(self):
        list = dict()
        list["1"] = TagMock("1",random.randint(0,50))
        list["2"] = TagMock("2", random.randint(0,50))
        list["3"] = TagMock("3", random.randint(0,50))
        list["4"] = TagMock("4", random.randint(0, 50))
        return list
