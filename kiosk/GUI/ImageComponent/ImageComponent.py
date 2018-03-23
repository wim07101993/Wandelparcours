from kivy.uix.boxlayout import BoxLayout
from kivy.uix.image import AsyncImage
from kivy.uix.carousel import Carousel
from kivy.clock import Clock
from kivy.uix.relativelayout import RelativeLayout
from kivy.uix.button import Button
from RestService.Rest import Rest


class ImageComponentLayout(RelativeLayout):
    def build(self):
        self.carousel=Carousel(direction='right', loop=True)
        self.add_widget(self.carousel)
        Clock.schedule_interval(self.changer, 8)

    def SetTag(self, tag):
        self.tag=tag

    def Start(self):
        print(self.tag.GetMac())
        tagid = self.tag.GetMac()
        rest = Rest()
        request = rest.GetImages(tagid)
        for r in request:
            #src="http://10.9.4.40:5000/API/v1/media/5ab11d2eb9cb951e3de4ea72.jpg"
            src= rest.GetUrlForMedia(id=r['id'])+".jpg"
            image = AsyncImage(source=src, allow_stretch=True)
            self.carousel.add_widget(image)

    def changer(self, *args):
        self.carousel.load_next()







