from RestService.Rest import Rest
from BeaconScanner.BeaconScanner import  BeaconScanner
from MockUp import  ScannerMock
from Algorithms import TagAlgorithm
'''r= Rest()
r.GetImages("1")'''

s = BeaconScanner()
l =s.scan()

for o in l:
    print("adress: %s  signaal: %s" % (o.GetMac(), o.GetStrength()))
d= TagAlgorithm.TagAlgorithm.getClosestBeacon(l)

print("closest")
print("adress: %s  signaal: %s" % (d.GetMac(), d.GetStrength()))