from Interfaces.TagInterface import TagInterface
from abc import *
class ScannerInterface(metaclass=ABCMeta):


    @abstractmethod
    def scan(self):
        pass

