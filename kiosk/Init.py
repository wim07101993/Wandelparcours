from RestService.Rest import Rest
from BeaconScanner.BeaconScanner import  BeaconScanner
from BeaconScanner.BeaconTag import  BeaconTag
from MockUp import  ScannerMock
from Algorithms import TagAlgorithm
from kivy.app import App
from GUI.ComponentHandler import ComponentHandler
'''r= Rest()
r.GetImages("1")

s = BeaconScanner()
l =s.scan()

for o in l:
    print("adress: %s  signaal: %s" % (o.GetMac(), o.GetStrength()))
d= TagAlgorithm.TagAlgorithm.getClosestBeacon(l)

print("closest")
print("adress: %s  signaal: %s" % (d.GetMac(), d.GetStrength()))'''

#initializer
class Init(App):
    def build(self):
        componentHandler=ComponentHandler("image")
        tag= BeaconTag("1",123)
        componentHandler.SetTag(tag)
        componentHandler.Start()
        component=componentHandler.GetComponent()
        return component



Init().run()
