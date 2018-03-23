from kivy.uix.videoplayer import VideoPlayer

from RestService.Rest import Rest


class VideoComponentLayout():
    def build(self):
        self.videoplayer=VideoPlayer(source='http://localhost:5000/api/v1/media/5aaf961a4b14c404c0442c2a.mp4',state='play')
        self.add_widget(self.videoplayer)

    def SetTag(self, tag):
        self.tag = tag

    def Start(self):
        print(self.tag.GetMac())