from Interfaces.TagInterface import TagInterface
from abc import *
class ScannerInterface(metaclass=ABCMeta):
    def __init__(self):
        pass

    @abstractmethod
    def scan(self):
        pass

