from abc import *


class RestInterface(metaclass=ABCMeta):
    @abstractmethod
    def GetImages(self):
        pass

    @abstractmethod
    def GetVideos(self):
        pass

    @abstractmethod
    def GetUrlForMedia(self,id,type="media"):
        pass