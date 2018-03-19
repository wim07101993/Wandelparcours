from Interfaces.RestInterface import RestInterface

class Rest(RestInterface):

    __url=""
    def __init__(self):
        self.__url="10.9.4.40"

    def GetImages(self):


        return [{
            "url": None,
            "id": "5aaa8f124d0ef51740eaeccb"
        },
            {
                "url": None,
                "id": "5aaa910240e6f93d60bb08eb"
            }
        ]

    def GetVideos(self):
        return [{
            "url": None,
            "id": "5aaa8f124d0ef51740eaeccb"
        },
            {
                "url": None,
                "id": "5aaa910240e6f93d60bb08eb"
            }
        ]

    def GetUrlForMedia(self, id, type="media"):
        if type == "image":
            return "http://imaging.nikon.com/lineup/lens/zoom/normalzoom/af-s_dx_18-140mmf_35-56g_ed_vr/img/sample/sample1_l.jpg"
        elif type == "video":
            return "https://www.w3schools.com/html/mov_bbb.mp4"
