class TagAlgorithm:
    @staticmethod
    def getClosestBeacon(beacons):

        beacon=None
        for b in beacons:
            if beacon==None:
                beacon=b
            else:
                if(beacon.GetStrength()<b.GetStrength()):
                    beacon=b
        return beacon