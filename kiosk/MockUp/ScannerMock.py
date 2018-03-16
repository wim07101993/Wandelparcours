from Interfaces.ScannerInterface import ScannerInterface
from InjectionHandler import InjectionHandler
from MockUp.TagMock import TagMock
class ScannerMock(ScannerInterface):

    def scan(self):
        list = dict()
        list["testmac"] = TagMock("testmac",5)
        return list

scanner = ScannerMock()
scanner.scan()