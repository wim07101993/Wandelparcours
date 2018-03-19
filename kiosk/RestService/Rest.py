from Interfaces.RestInterface import RestInterface
import requests

class Rest(RestInterface):

    __url=""
    def __init__(self):
        self.__url="http://10.9.4.40:5000"

    def GetImages(self,id):
        url = self.__url+"/API/v1/residents/"+id+"/images"
        r = requests.get(url)
        return r.json()

    def GetVideos(self):
        url = self.__url + "/API/v1/residents/" + id + "/Video"
        r = requests.get(url)
        return r.json()

    def GetUrlForMedia(self, id, type="media"):
        return self.__url+"/API/v1/"+type+"/"+id
