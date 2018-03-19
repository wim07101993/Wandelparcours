'''[
    {
        "url": null,
        "id": "5aaa8f124d0ef51740eaeccb"
    },
    {
        "url": null,
        "id": "5aaa910240e6f93d60bb08eb"
    }
]'''

from Interfaces.RestInterface import RestInterface


class RestMockup(RestInterface):
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
