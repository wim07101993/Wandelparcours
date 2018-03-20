

class BeaconTag:
    __mac = None  # type: str
    __strength = None  # type: float

    def __init__(self, mac, strength):
        self.__mac=mac
        self.__strength = strength

    def GetStrength(self):
        return self.__strength

    def GetMac(self):
        return self.__mac




