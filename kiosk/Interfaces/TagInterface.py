from abc import *

class TagInterface(metaclass=ABCMeta):


    def __init__(self,mac,strength):
        pass

    @abstractmethod
    def GetStrength(self) -> float:
        pass

    @abstractmethod
    def GetMac(self) -> str:
        pass



