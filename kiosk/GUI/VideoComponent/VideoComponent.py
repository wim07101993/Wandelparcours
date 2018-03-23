from kivy.uix.relativelayout import RelativeLayout
from kivy.uix.videoplayer import VideoPlayer
from kivy.uix.video import Video

from RestService.Rest import Rest


class VideoComponentLayout(RelativeLayout):
    def build(self):
        self.videoplayer=VideoPlayer(source='http://localhost:5000/api/v1/media/5ab4d4731b0d7051046fc06e.mkv',
                                     state='play')
        self.add_widget(self.videoplayer)

    def SetTag(self, tag):
        self.tag = tag

    def Start(self):
        print(self.tag.GetMac())